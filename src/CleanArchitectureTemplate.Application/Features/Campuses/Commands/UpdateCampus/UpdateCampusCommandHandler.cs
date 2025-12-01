using CleanArchitectureTemplate.Application.Common.DTOs.Campus;
using CleanArchitectureTemplate.Application.Common.Exceptions;
using CleanArchitectureTemplate.Application.Common.Interfaces;
using CleanArchitectureTemplate.Domain.Entities;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Campuses.Commands.UpdateCampus;

public class UpdateCampusCommandHandler : IRequestHandler<UpdateCampusCommand, CampusDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCampusCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CampusDto> Handle(UpdateCampusCommand request, CancellationToken cancellationToken)
    {
        var campus = await _unitOfWork.Campuses.GetByIdAsync(request.Id)
            ?? throw new NotFoundException(nameof(Campus), request.Id);

        if (campus.IsDeleted)
        {
            throw new NotFoundException(nameof(Campus), request.Id);
        }

        campus.CampusName = request.CampusName;
        campus.Address = request.Address;
        campus.WorkingHoursStart = request.WorkingHoursStart;
        campus.WorkingHoursEnd = request.WorkingHoursEnd;
        campus.ContactPhone = request.ContactPhone;
        campus.ContactEmail = request.ContactEmail;
        campus.IsActive = request.IsActive;
        campus.ModifiedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CampusDto(
            campus.Id,
            campus.CampusCode,
            campus.CampusName,
            campus.Address,
            campus.WorkingHoursStart,
            campus.WorkingHoursEnd,
            campus.ContactPhone,
            campus.ContactEmail,
            campus.IsActive
        );
    }
}
