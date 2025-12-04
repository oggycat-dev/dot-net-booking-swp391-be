using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Holidays.Commands.DeleteHoliday;

public class DeleteHolidayCommand : IRequest<Unit>
{
    public Guid HolidayId { get; set; }
}
