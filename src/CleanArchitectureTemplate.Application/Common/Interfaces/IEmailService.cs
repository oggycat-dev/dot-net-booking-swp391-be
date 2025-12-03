namespace CleanArchitectureTemplate.Application.Common.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body);
    Task SendRegistrationApprovedEmailAsync(string to, string fullName);
    Task SendRegistrationRejectedEmailAsync(string to, string fullName, string reason);
    Task SendCampusChangeApprovedEmailAsync(string to, string fullName, string newCampusName);
    Task SendCampusChangeRejectedEmailAsync(string to, string fullName, string reason);
    Task SendBookingApprovedEmailAsync(string to, string fullName, string facilityName, DateTime startTime, DateTime endTime);
    Task SendBookingRejectedEmailAsync(string to, string fullName, string facilityName, string reason);
    Task SendBookingReminderEmailAsync(string to, string fullName, string facilityName, DateTime startTime);
    Task SendPasswordResetCodeEmailAsync(string to, string fullName, string verificationCode);
}
