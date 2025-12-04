using CleanArchitectureTemplate.Application.Common.DTOs.Holiday;
using CleanArchitectureTemplate.Application.Common.Interfaces;
using CleanArchitectureTemplate.Domain.Entities;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Holidays.Commands.CreateHoliday;

public class CreateHolidayCommandHandler : IRequestHandler<CreateHolidayCommand, HolidayDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateHolidayCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<HolidayDto> Handle(CreateHolidayCommand request, CancellationToken cancellationToken)
    {
        var holiday = new Holiday
        {
            Id = Guid.NewGuid(),
            HolidayName = request.HolidayName,
            HolidayDate = DateTime.SpecifyKind(request.HolidayDate.Date, DateTimeKind.Utc),
            IsRecurring = request.IsRecurring,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Holidays.AddAsync(holiday);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new HolidayDto(
            holiday.Id,
            holiday.HolidayName,
            holiday.HolidayDate,
            holiday.IsRecurring,
            holiday.Description,
            holiday.CreatedAt
        );
    }
}
