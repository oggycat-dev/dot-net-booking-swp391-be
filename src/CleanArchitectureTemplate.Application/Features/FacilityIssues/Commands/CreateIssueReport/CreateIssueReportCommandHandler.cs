using CleanArchitectureTemplate.Application.Common.DTOs.FacilityIssue;
using CleanArchitectureTemplate.Application.Common.Exceptions;
using CleanArchitectureTemplate.Application.Common.Interfaces;
using CleanArchitectureTemplate.Domain.Entities;
using CleanArchitectureTemplate.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace CleanArchitectureTemplate.Application.Features.FacilityIssues.Commands.CreateIssueReport;

public class CreateIssueReportCommandHandler : IRequestHandler<CreateIssueReportCommand, FacilityIssueReportDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ICloudinaryService _cloudinaryService;
    private readonly IFirebaseNotificationService _firebaseService;

    public CreateIssueReportCommandHandler(
        IUnitOfWork unitOfWork, 
        ICurrentUserService currentUserService,
        ICloudinaryService cloudinaryService,
        IFirebaseNotificationService firebaseService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _cloudinaryService = cloudinaryService;
        _firebaseService = firebaseService;
    }

    public async Task<FacilityIssueReportDto> Handle(CreateIssueReportCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId 
            ?? throw new UnauthorizedAccessException("User not authenticated");

        // Verify booking exists and belongs to current user or user is the one who checked in
        var booking = await _unitOfWork.Bookings.GetByIdAsync(request.BookingId)
            ?? throw new NotFoundException(nameof(Booking), request.BookingId);

        if (booking.UserId != userId)
        {
            throw new UnauthorizedAccessException("You can only report issues for your own bookings");
        }

        // Verify booking is currently in use
        if (booking.Status != BookingStatus.InUse)
        {
            throw new ValidationException("Can only report issues during active booking (InUse status)");
        }

        // Generate report code
        var reportCount = await _unitOfWork.FacilityIssueReports.GetQueryable()
            .CountAsync(cancellationToken);
        var reportCode = $"ISSUE-{DateTime.UtcNow:yyyyMMdd}-{(reportCount + 1):D4}";

        // Upload images to Cloudinary if provided
        string? imageUrlsJson = null;
        if (request.Images != null && request.Images.Any())
        {
            var uploadedImageUrls = new List<string>();
            foreach (var image in request.Images)
            {
                using var stream = image.OpenReadStream();
                var url = await _cloudinaryService.UploadImageAsync(stream, image.FileName);
                uploadedImageUrls.Add(url);
            }
            // Store as JSON array
            imageUrlsJson = JsonSerializer.Serialize(uploadedImageUrls);
        }

        var report = new FacilityIssueReport
        {
            Id = Guid.NewGuid(),
            ReportCode = reportCode,
            BookingId = request.BookingId,
            ReportedBy = userId,
            IssueTitle = request.IssueTitle,
            IssueDescription = request.IssueDescription,
            Severity = request.Severity,
            Category = request.Category,
            ImageUrls = imageUrlsJson,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.FacilityIssueReports.AddAsync(report);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Reload with related data
        var createdReport = await _unitOfWork.FacilityIssueReports.GetQueryable()
            .Include(r => r.Booking)
                .ThenInclude(b => b.Facility)
            .Include(r => r.ReportedByUser)
            .FirstOrDefaultAsync(r => r.Id == report.Id, cancellationToken);

        // Send Firebase notification to all admins
        try
        {
            var notificationData = new Dictionary<string, string>
            {
                { "type", "facility_issue_report" },
                { "reportId", createdReport!.Id.ToString() },
                { "reportCode", createdReport.ReportCode },
                { "bookingCode", createdReport.Booking.BookingCode },
                { "facilityName", createdReport.Booking.Facility.FacilityName },
                { "severity", createdReport.Severity }
            };

            await _firebaseService.SendToAllAdminsAsync(
                title: "🚨 New Facility Issue Reported",
                body: $"{createdReport.ReportedByUser.FullName} reported: {createdReport.IssueTitle} at {createdReport.Booking.Facility.FacilityName}",
                data: notificationData
            );
        }
        catch
        {
            // Log but don't fail the request if notification fails
            // Notification failure shouldn't block report creation
        }

        // Parse image URLs from JSON
        List<string>? imageUrls = null;
        if (!string.IsNullOrEmpty(createdReport!.ImageUrls))
        {
            try
            {
                imageUrls = JsonSerializer.Deserialize<List<string>>(createdReport.ImageUrls);
            }
            catch
            {
                // If parsing fails, treat as single URL
                imageUrls = new List<string> { createdReport.ImageUrls };
            }
        }

        return new FacilityIssueReportDto(
            createdReport.Id,
            createdReport.ReportCode,
            createdReport.BookingId,
            createdReport.Booking.BookingCode,
            createdReport.Booking.FacilityId,
            createdReport.Booking.Facility.FacilityName,
            createdReport.ReportedByUser.FullName,
            createdReport.ReportedByUser.Email,
            createdReport.IssueTitle,
            createdReport.IssueDescription,
            createdReport.Severity,
            createdReport.Category,
            imageUrls,
            createdReport.Status,
            createdReport.NewFacilityId,
            createdReport.NewFacility?.FacilityName,
            createdReport.AdminResponse,
            createdReport.CreatedAt,
            createdReport.HandledAt,
            createdReport.ResolvedAt
        );
    }
}
