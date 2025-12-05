using CleanArchitectureTemplate.Application.Common.DTOs.FacilityIssue;
using CleanArchitectureTemplate.Application.Common.Exceptions;
using CleanArchitectureTemplate.Application.Common.Interfaces;
using CleanArchitectureTemplate.Domain.Entities;
using CleanArchitectureTemplate.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Text.Json;

namespace CleanArchitectureTemplate.Application.Features.FacilityIssues.Commands.ChangeRoomForIssue;

public class ChangeRoomForIssueCommandHandler : IRequestHandler<ChangeRoomForIssueCommand, FacilityIssueReportDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IConfiguration _configuration;

    public ChangeRoomForIssueCommandHandler(
        IUnitOfWork unitOfWork, 
        ICurrentUserService currentUserService,
        IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _configuration = configuration;
    }

    public async Task<FacilityIssueReportDto> Handle(ChangeRoomForIssueCommand request, CancellationToken cancellationToken)
    {
        var adminId = _currentUserService.UserId 
            ?? throw new UnauthorizedAccessException("User not authenticated");

        // Get the issue report
        var report = await _unitOfWork.FacilityIssueReports.GetQueryable()
            .Include(r => r.Booking)
                .ThenInclude(b => b.Facility)
            .Include(r => r.Booking)
                .ThenInclude(b => b.User)
            .Include(r => r.ReportedByUser)
            .FirstOrDefaultAsync(r => r.Id == request.ReportId, cancellationToken)
            ?? throw new NotFoundException(nameof(FacilityIssueReport), request.ReportId);

        if (report.Status == "RoomChanged" || report.Status == "Resolved")
        {
            throw new ValidationException("This issue has already been resolved");
        }

        // Get the new facility
        var newFacility = await _unitOfWork.Facilities.GetByIdAsync(request.NewFacilityId)
            ?? throw new NotFoundException(nameof(Facility), request.NewFacilityId);

        if (!newFacility.IsActive || newFacility.Status != FacilityStatus.Available)
        {
            throw new ValidationException("Selected facility is not available");
        }

        var booking = report.Booking;
        var now = DateTime.UtcNow;
        
        // Check if new facility is available for the remaining time
        // From now until the original booking end time
        var endDateTime = booking.BookingDate.Date.Add(booking.EndTime);
        
        var conflictingBookings = await _unitOfWork.Bookings.GetQueryable()
            .Where(b => b.FacilityId == request.NewFacilityId &&
                       b.BookingDate.Date == booking.BookingDate.Date &&
                       !b.IsDeleted &&
                       b.Id != booking.Id &&
                       (b.Status == BookingStatus.Approved || 
                        b.Status == BookingStatus.InUse ||
                        b.Status == BookingStatus.Pending ||
                        b.Status == BookingStatus.WaitingLecturerApproval))
            .ToListAsync(cancellationToken);

        var currentTime = now.TimeOfDay;
        var hasConflict = conflictingBookings.Any(b => 
            b.StartTime < booking.EndTime && b.EndTime > currentTime);

        if (hasConflict)
        {
            throw new ValidationException("Selected facility has conflicting bookings for the remaining time");
        }

        // Update the booking with new facility and adjusted start time
        var oldFacilityName = booking.Facility.FacilityName;
        booking.FacilityId = request.NewFacilityId;
        booking.StartTime = currentTime; // Start from now
        // Keep the original end time
        booking.ModifiedAt = now;

        // Update the report
        report.NewFacilityId = request.NewFacilityId;
        report.HandledBy = adminId;
        report.AdminResponse = request.AdminResponse;
        report.HandledAt = now;
        report.ResolvedAt = now;
        report.Status = "RoomChanged";
        report.ModifiedAt = now;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Send email notification to user
        await SendRoomChangeEmailAsync(
            booking.User.Email,
            booking.User.FullName,
            booking.BookingCode,
            oldFacilityName,
            newFacility.FacilityName,
            newFacility.Building ?? "",
            newFacility.Floor ?? "",
            newFacility.RoomNumber ?? "",
            booking.BookingDate,
            booking.StartTime,
            booking.EndTime,
            report.IssueTitle,
            request.AdminResponse
        );

        // Reload with updated data
        var updatedReport = await _unitOfWork.FacilityIssueReports.GetQueryable()
            .Include(r => r.Booking)
                .ThenInclude(b => b.Facility)
            .Include(r => r.ReportedByUser)
            .Include(r => r.NewFacility)
            .FirstOrDefaultAsync(r => r.Id == report.Id, cancellationToken);

        // Parse image URLs from JSON
        List<string>? imageUrls = null;
        if (!string.IsNullOrEmpty(updatedReport!.ImageUrls))
        {
            try
            {
                imageUrls = JsonSerializer.Deserialize<List<string>>(updatedReport.ImageUrls);
            }
            catch
            {
                imageUrls = new List<string> { updatedReport.ImageUrls };
            }
        }

        return new FacilityIssueReportDto(
            updatedReport.Id,
            updatedReport.ReportCode,
            updatedReport.BookingId,
            updatedReport.Booking.BookingCode,
            updatedReport.Booking.FacilityId,
            updatedReport.Booking.Facility.FacilityName,
            updatedReport.ReportedByUser.FullName,
            updatedReport.ReportedByUser.Email,
            updatedReport.IssueTitle,
            updatedReport.IssueDescription,
            updatedReport.Severity,
            updatedReport.Category,
            imageUrls,
            updatedReport.Status,
            updatedReport.NewFacilityId,
            updatedReport.NewFacility?.FacilityName,
            updatedReport.AdminResponse,
            updatedReport.CreatedAt,
            updatedReport.HandledAt,
            updatedReport.ResolvedAt
        );
    }

    private async Task SendRoomChangeEmailAsync(
        string userEmail,
        string userName,
        string bookingCode,
        string oldFacilityName,
        string newFacilityName,
        string building,
        string floor,
        string roomNumber,
        DateTime bookingDate,
        TimeSpan startTime,
        TimeSpan endTime,
        string issueTitle,
        string adminResponse)
    {
        try
        {
            var smtpHost = _configuration["Email:SmtpHost"];
            var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
            var smtpUser = _configuration["Email:SmtpUser"];
            var smtpPass = _configuration["Email:SmtpPassword"];
            var fromEmail = _configuration["Email:FromEmail"];

            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(smtpUser, smtpPass)
            };

            var location = $"{building} - Floor {floor} - Room {roomNumber}";
            
            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail ?? smtpUser!),
                Subject = $"Room Changed for Booking {bookingCode}",
                Body = $@"
Dear {userName},

Your room has been changed due to the facility issue you reported.

Issue Reported: {issueTitle}

Original Room: {oldFacilityName}
New Room: {newFacilityName}
Location: {location}

Booking Details:
- Booking Code: {bookingCode}
- Date: {bookingDate:yyyy-MM-dd}
- Time: {startTime:hh\\:mm} - {endTime:hh\\:mm}

Admin Response:
{adminResponse}

Please proceed to the new room for your booking.

Best regards,
FPT Booking System
",
                IsBodyHtml = false
            };

            mailMessage.To.Add(userEmail);
            await client.SendMailAsync(mailMessage);
        }
        catch (Exception ex)
        {
            // Log email error but don't fail the operation
            Console.WriteLine($"Failed to send room change email: {ex.Message}");
        }
    }
}
