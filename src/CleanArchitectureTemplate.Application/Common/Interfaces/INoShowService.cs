namespace CleanArchitectureTemplate.Application.Common.Interfaces;

public interface INoShowService
{
    Task CheckAndMarkNoShowBookingsAsync(Guid userId);
}
