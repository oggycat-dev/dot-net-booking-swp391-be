using CleanArchitectureTemplate.Application.Common.DTOs.Campus;
using CleanArchitectureTemplate.Application.Common.Exceptions;
using CleanArchitectureTemplate.Application.Common.Interfaces;
using CleanArchitectureTemplate.Domain.Entities;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Campuses.Commands.CreateCampus;

public class CreateCampusCommandHandler : IRequestHandler<CreateCampusCommand, CampusDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateCampusCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CampusDto> Handle(CreateCampusCommand request, CancellationToken cancellationToken)
    {
        // Check if campus code already exists
        if (await _unitOfWork.Campuses.IsCodeExistsAsync(request.CampusCode))
        {
            throw new ValidationException($"Campus with code '{request.CampusCode}' already exists");
        }

        var campus = new Campus
        {
            Id = Guid.NewGuid(),
            CampusCode = request.CampusCode,
            CampusName = request.CampusName,
            Address = request.Address,
            WorkingHoursStart = request.WorkingHoursStart,
            WorkingHoursEnd = request.WorkingHoursEnd,
            ContactPhone = request.ContactPhone,
            ContactEmail = request.ContactEmail,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Campuses.AddAsync(campus);
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
