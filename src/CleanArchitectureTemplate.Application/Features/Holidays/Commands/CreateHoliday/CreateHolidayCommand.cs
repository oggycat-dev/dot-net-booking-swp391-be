using CleanArchitectureTemplate.Application.Common.DTOs.Holiday;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Holidays.Commands.CreateHoliday;

public class CreateHolidayCommand : IRequest<HolidayDto>
{
    public string HolidayName { get; set; } = string.Empty;
    public DateTime HolidayDate { get; set; }
    public bool IsRecurring { get; set; }
    public string? Description { get; set; }
}
