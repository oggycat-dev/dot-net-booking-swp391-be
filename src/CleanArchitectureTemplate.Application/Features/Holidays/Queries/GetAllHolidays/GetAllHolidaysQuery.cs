using CleanArchitectureTemplate.Application.Common.DTOs.Holiday;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Holidays.Queries.GetAllHolidays;

public class GetAllHolidaysQuery : IRequest<List<HolidayDto>>
{
}
