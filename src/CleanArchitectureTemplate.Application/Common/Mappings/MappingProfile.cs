using AutoMapper;
using CleanArchitectureTemplate.Domain.Entities;
using CleanArchitectureTemplate.Domain.Enums;
using CleanArchitectureTemplate.Application.Common.DTOs.Users;
using CleanArchitectureTemplate.Application.Common.DTOs.Booking;

namespace CleanArchitectureTemplate.Application.Common.Mappings;

/// <summary>
/// Mapping profile for AutoMapper
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User mappings
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.UserCode, opt => opt.MapFrom(src => src.UserCode))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()))
            .ForMember(dest => dest.Department, opt => opt.MapFrom(src => src.Department))
            .ForMember(dest => dest.Major, opt => opt.MapFrom(src => src.Major))
            .ForMember(dest => dest.CampusId, opt => opt.MapFrom(src => src.CampusId))
            .ForMember(dest => dest.CampusName, opt => opt.MapFrom(src => src.Campus != null ? src.Campus.CampusName : null))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
            .ForMember(dest => dest.IsApproved, opt => opt.MapFrom(src => src.IsApproved))
            .ForMember(dest => dest.IsBlocked, opt => opt.MapFrom(src => src.IsBlocked))
            .ForMember(dest => dest.BlockedReason, opt => opt.MapFrom(src => src.BlockedReason))
            .ForMember(dest => dest.BlockedUntil, opt => opt.MapFrom(src => src.BlockedUntil))
            .ForMember(dest => dest.NoShowCount, opt => opt.MapFrom(src => src.NoShowCount))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));
            
        CreateMap<CreateUserRequest, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedAt, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.EmailConfirmed, opt => opt.MapFrom(src => false))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => Enum.Parse<UserRole>(src.Role)));

        // Booking mappings
        CreateMap<Booking, BookingDto>()
            .ConstructUsing(src => new BookingDto(
                src.Id,
                src.BookingCode,
                src.FacilityId,
                src.Facility != null ? src.Facility.FacilityName : string.Empty,
                src.Facility != null ? src.Facility.FacilityCode : string.Empty,
                src.UserId,
                src.User != null ? src.User.FullName : string.Empty,
                src.User != null ? src.User.Email : string.Empty,
                src.User != null ? src.User.Role.ToString() : string.Empty,
                src.BookingDate,
                src.StartTime,
                src.EndTime,
                src.Purpose,
                src.NumParticipants,
                src.EquipmentNeeded,
                src.Note,
                src.Status.ToString(),
                src.LecturerEmail,
                src.LecturerApprovedBy,
                src.LecturerApprover != null ? src.LecturerApprover.FullName : null,
                src.LecturerApprovedAt,
                src.LecturerRejectReason,
                src.ApprovedBy,
                src.Approver != null ? src.Approver.FullName : null,
                src.ApprovedAt,
                src.RejectReason,
                src.CheckedInAt,
                src.CheckedOutAt,
                src.Rating,
                src.Comment,
                src.CreatedAt
            ));

        CreateMap<Booking, BookingListDto>()
            .ConstructUsing(src => new BookingListDto(
                src.Id,
                src.BookingCode,
                src.Facility != null ? src.Facility.FacilityName : string.Empty,
                src.User != null ? src.User.FullName : string.Empty,
                src.User != null ? src.User.Role.ToString() : string.Empty,
                src.BookingDate,
                src.StartTime,
                src.EndTime,
                src.Status.ToString(),
                src.LecturerEmail,
                src.CreatedAt
            ));
    }
}
