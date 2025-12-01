using CleanArchitectureTemplate.Application.Common.Exceptions;
using CleanArchitectureTemplate.Application.Common.Interfaces;
using CleanArchitectureTemplate.Domain.Entities;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Campuses.Commands.DeleteCampus;

public class DeleteCampusCommandHandler : IRequestHandler<DeleteCampusCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCampusCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(DeleteCampusCommand request, CancellationToken cancellationToken)
    {
        var campus = await _unitOfWork.Campuses.GetByIdAsync(request.Id)
            ?? throw new NotFoundException(nameof(Campus), request.Id);

        if (campus.IsDeleted)
        {
            throw new NotFoundException(nameof(Campus), request.Id);
        }

        // Soft delete
        campus.IsDeleted = true;
        campus.DeletedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
