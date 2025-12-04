using CleanArchitectureTemplate.Application.Common.DTOs.Holiday;
using CleanArchitectureTemplate.Application.Common.Interfaces;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Holidays.Queries.GetAllHolidays;

public class GetAllHolidaysQueryHandler : IRequestHandler<GetAllHolidaysQuery, List<HolidayDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllHolidaysQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<HolidayDto>> Handle(GetAllHolidaysQuery request, CancellationToken cancellationToken)
    {
        var holidays = await _unitOfWork.Holidays.GetAllAsync();

        return holidays
            .OrderBy(h => h.HolidayDate)
            .Select(h => new HolidayDto(
                h.Id,
                h.HolidayName,
                h.HolidayDate,
                h.IsRecurring,
                h.Description,
                h.CreatedAt
            ))
            .ToList();
    }
}
