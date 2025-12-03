using CleanArchitectureTemplate.Application.Common.DTOs.Booking;
using CleanArchitectureTemplate.Application.Common.Exceptions;
using CleanArchitectureTemplate.Application.Common.Interfaces;
using CleanArchitectureTemplate.Domain.Entities;
using CleanArchitectureTemplate.Domain.Enums;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Bookings.Commands.CreateBooking;

public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, BookingDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public CreateBookingCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<BookingDto> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        // Get current user
        var userId = _currentUserService.UserId 
            ?? throw new ValidationException("User not authenticated");

        var user = await _unitOfWork.Users.GetByIdAsync(userId)
            ?? throw new NotFoundException(nameof(User), userId);

        // Check if user is blocked or inactive
        if (!user.CanBook())
        {
            throw new ValidationException("Your account is blocked or inactive. You cannot make bookings.");
        }

        // Validate facility exists and is available
        var facility = await _unitOfWork.Facilities.GetByIdAsync(request.FacilityId)
            ?? throw new NotFoundException(nameof(Facility), request.FacilityId);

        if (facility.Status != FacilityStatus.Available)
        {
            throw new ValidationException($"Facility is currently {facility.Status}");
        }

        // Validate booking date is not in the past
        var today = DateTime.SpecifyKind(DateTime.UtcNow.Date, DateTimeKind.Utc);
        if (request.BookingDate.Date < today)
        {
            throw new ValidationException("Cannot book facilities for past dates");
        }

        // Validate booking date is within allowed range based on role
        var maxDaysAhead = user.GetMaxBookingDaysAhead();
        var maxDate = DateTime.SpecifyKind(DateTime.UtcNow.Date.AddDays(maxDaysAhead), DateTimeKind.Utc);
        if (request.BookingDate.Date > maxDate)
        {
            throw new ValidationException($"{user.Role} can only book up to {maxDaysAhead} days in advance");
        }

        // Validate time range is within facility working hours
        var campus = await _unitOfWork.Campuses.GetByIdAsync(facility.CampusId)
            ?? throw new NotFoundException(nameof(Campus), facility.CampusId);

        if (!campus.IsTimeRangeValid(request.StartTime, request.EndTime))
        {
            throw new ValidationException($"Booking time must be within campus working hours ({campus.WorkingHoursStart} - {campus.WorkingHoursEnd})");
        }

        // Check for time conflicts
        if (await _unitOfWork.Bookings.HasConflictAsync(request.FacilityId, request.BookingDate, request.StartTime, request.EndTime))
        {
            throw new ValidationException("This time slot is already booked");
        }

        // For Student bookings, lecturer email is required
        if (user.Role == UserRole.Student)
        {
            if (string.IsNullOrWhiteSpace(request.LecturerEmail))
            {
                throw new ValidationException("Lecturer email is required for student bookings");
            }

            // Validate lecturer email format
            if (!request.LecturerEmail.EndsWith("@fpt.edu.vn"))
            {
                throw new ValidationException("Lecturer email must be an @fpt.edu.vn address");
            }

            // Optionally: Verify lecturer exists in system
            var lecturer = await _unitOfWork.Users.GetByEmailAsync(request.LecturerEmail);
            if (lecturer != null && lecturer.Role != UserRole.Lecturer)
            {
                throw new ValidationException("The provided email does not belong to a lecturer");
            }
        }

        // Create booking
        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            BookingCode = Booking.GenerateBookingCode(request.BookingDate),
            FacilityId = request.FacilityId,
            UserId = userId,
            BookingDate = DateTime.SpecifyKind(request.BookingDate.Date, DateTimeKind.Utc),
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            Purpose = request.Purpose,
            NumParticipants = request.NumParticipants,
            EquipmentNeeded = request.EquipmentNeeded,
            Note = request.Note,
            LecturerEmail = user.Role == UserRole.Student ? request.LecturerEmail : null,
            Status = user.Role == UserRole.Student 
                ? BookingStatus.WaitingLecturerApproval 
                : BookingStatus.Pending,  // Lecturer goes straight to Admin
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Bookings.AddAsync(booking);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // TODO: Send notification email
        // If Student -> Send to Lecturer
        // If Lecturer -> Send to Admin

        return new BookingDto(
            booking.Id,
            booking.BookingCode,
            facility.Id,
            facility.FacilityName,
            facility.FacilityCode,
            user.Id,
            user.FullName,
            user.Email,
            user.Role.ToString(),
            booking.BookingDate,
            booking.StartTime,
            booking.EndTime,
            booking.Purpose,
            booking.NumParticipants,
            booking.EquipmentNeeded,
            booking.Note,
            booking.Status.ToString(),
            booking.LecturerEmail,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            booking.CreatedAt
        );
    }
}
