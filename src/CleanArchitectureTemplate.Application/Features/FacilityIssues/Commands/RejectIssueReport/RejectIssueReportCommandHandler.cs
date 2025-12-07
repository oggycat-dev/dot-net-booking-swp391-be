using CleanArchitectureTemplate.Application.Common.DTOs.FacilityIssue;
using CleanArchitectureTemplate.Application.Common.Exceptions;
using CleanArchitectureTemplate.Application.Common.Interfaces;
using CleanArchitectureTemplate.Domain.Entities;
using CleanArchitectureTemplate.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Text.Json;

namespace CleanArchitectureTemplate.Application.Features.FacilityIssues.Commands.RejectIssueReport;

public class RejectIssueReportCommandHandler : IRequestHandler<RejectIssueReportCommand, FacilityIssueReportDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IConfiguration _configuration;

    public RejectIssueReportCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _configuration = configuration;
    }

    public async Task<FacilityIssueReportDto> Handle(RejectIssueReportCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId
            ?? throw new UnauthorizedAccessException("User not authenticated");

        // Get report with all relationships
        var report = await _unitOfWork.FacilityIssueReports.GetByIdAsync(request.ReportId)
            ?? throw new NotFoundException(nameof(FacilityIssueReport), request.ReportId);

        if (report.Status != "Pending")
        {
            throw new ValidationException($"Cannot reject report with status {report.Status}. Only pending reports can be rejected.");
        }

        // Get booking and user
        var booking = await _unitOfWork.Bookings.GetByIdAsync(report.BookingId)
            ?? throw new NotFoundException(nameof(Booking), report.BookingId);

        var facility = await _unitOfWork.Facilities.GetByIdAsync(booking.FacilityId)
            ?? throw new NotFoundException(nameof(Facility), booking.FacilityId);

        var user = await _unitOfWork.Users.GetByIdAsync(booking.UserId)
            ?? throw new NotFoundException(nameof(User), booking.UserId);

        // Update report status
        report.Status = "Rejected";
        report.HandledBy = userId;
        report.HandledAt = DateTime.UtcNow;
        report.AdminResponse = request.RejectionReason;

        await _unitOfWork.FacilityIssueReports.UpdateAsync(report);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Send email notification to user
        await SendRejectionEmailAsync(user, report, booking);

        // Parse image URLs
        List<string>? imageUrls = null;
        if (!string.IsNullOrEmpty(report.ImageUrls))
        {
            try
            {
                imageUrls = JsonSerializer.Deserialize<List<string>>(report.ImageUrls);
            }
            catch
            {
                imageUrls = new List<string> { report.ImageUrls };
            }
        }

        return new FacilityIssueReportDto(
            report.Id,
            report.ReportCode,
            report.BookingId,
            booking.BookingCode,
            facility.Id,
            facility.FacilityName,
            user.FullName,
            user.Email,
            report.IssueTitle,
            report.IssueDescription,
            report.Severity,
            report.Category,
            imageUrls,
            report.Status,
            report.NewFacilityId,
            null,
            report.AdminResponse,
            report.CreatedAt,
            report.HandledAt,
            report.ResolvedAt
        );
    }

    private async Task SendRejectionEmailAsync(User user, FacilityIssueReport report, Booking booking)
    {
        try
        {
            var smtpHost = _configuration["Email:SmtpHost"];
            var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
            var smtpUsername = _configuration["Email:SmtpUsername"];
            var smtpPassword = _configuration["Email:SmtpPassword"];
            var fromEmail = _configuration["Email:FromEmail"];
            var fromName = _configuration["Email:FromName"];

            using var smtpClient = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUsername, smtpPassword),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail!, fromName),
                Subject = $"Issue Report Rejected - {report.ReportCode}",
                Body = $@"
                    <html>
                    <body>
                        <h2>Issue Report Rejected</h2>
                        <p>Dear {user.FullName},</p>
                        <p>Your facility issue report has been rejected by the admin.</p>
                        
                        <h3>Report Details:</h3>
                        <ul>
                            <li><strong>Report Code:</strong> {report.ReportCode}</li>
                            <li><strong>Issue:</strong> {report.IssueTitle}</li>
                            <li><strong>Severity:</strong> {report.Severity}</li>
                            <li><strong>Booking Date:</strong> {booking.BookingDate:dd/MM/yyyy}</li>
                            <li><strong>Time:</strong> {booking.StartTime} - {booking.EndTime}</li>
                        </ul>
                        
                        <h3>Rejection Reason:</h3>
                        <p>{report.AdminResponse}</p>
                        
                        <p>If you have any questions, please contact the administration.</p>
                        
                        <p>Best regards,<br>FPT Booking System</p>
                    </body>
                    </html>",
                IsBodyHtml = true
            };

            mailMessage.To.Add(user.Email);
            await smtpClient.SendMailAsync(mailMessage);
        }
        catch (Exception)
        {
            // Log error but don't throw - email failure shouldn't fail the rejection
        }
    }
}
