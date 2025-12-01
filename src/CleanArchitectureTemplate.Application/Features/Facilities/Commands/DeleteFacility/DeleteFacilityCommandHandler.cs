using CleanArchitectureTemplate.Application.Common.Exceptions;
using CleanArchitectureTemplate.Application.Common.Interfaces;
using CleanArchitectureTemplate.Domain.Commons;
using CleanArchitectureTemplate.Domain.Entities;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Facilities.Commands.DeleteFacility;

public class DeleteFacilityCommandHandler : IRequestHandler<DeleteFacilityCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteFacilityCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteFacilityCommand request, CancellationToken cancellationToken)
    {
        var facility = await _unitOfWork.Facilities.GetByIdAsync(request.Id)
            ?? throw new NotFoundException(nameof(Facility), request.Id);

        if (facility.IsDeleted)
        {
            throw new NotFoundException(nameof(Facility), request.Id);
        }

        facility.MarkAsDeleted();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
