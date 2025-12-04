using CleanArchitectureTemplate.Application.Common.Exceptions;
using CleanArchitectureTemplate.Application.Common.Interfaces;
using CleanArchitectureTemplate.Domain.Entities;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Holidays.Commands.DeleteHoliday;

public class DeleteHolidayCommandHandler : IRequestHandler<DeleteHolidayCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteHolidayCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(DeleteHolidayCommand request, CancellationToken cancellationToken)
    {
        var holiday = await _unitOfWork.Holidays.GetByIdAsync(request.HolidayId)
            ?? throw new NotFoundException(nameof(Holiday), request.HolidayId);

        await _unitOfWork.Holidays.DeleteAsync(holiday);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
