using System.Net;
using System.Net.Mail;
using CleanArchitectureTemplate.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CleanArchitectureTemplate.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;
    private readonly string _smtpHost;
    private readonly int _smtpPort;
    private readonly string _smtpUsername;
    private readonly string _smtpPassword;
    private readonly string _fromEmail;
    private readonly string _fromName;
    private readonly bool _enableSsl;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
        
        _smtpHost = _configuration["Email:SmtpHost"] ?? "smtp.gmail.com";
        _smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
        _smtpUsername = _configuration["Email:SmtpUsername"] ?? "";
        _smtpPassword = _configuration["Email:SmtpPassword"] ?? "";
        _fromEmail = _configuration["Email:FromEmail"] ?? "";
        _fromName = _configuration["Email:FromName"] ?? "FPT Booking System";
        _enableSsl = bool.Parse(_configuration["Email:EnableSsl"] ?? "true");
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        try
        {
            if (string.IsNullOrEmpty(_smtpUsername) || string.IsNullOrEmpty(_smtpPassword))
            {
                _logger.LogWarning("Email service is not configured. Skipping email to {To}", to);
                return;
            }

            using var client = new SmtpClient(_smtpHost, _smtpPort)
            {
                EnableSsl = _enableSsl,
                Credentials = new NetworkCredential(_smtpUsername, _smtpPassword)
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_fromEmail, _fromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            
            mailMessage.To.Add(to);

            await client.SendMailAsync(mailMessage);
            _logger.LogInformation("Email sent successfully to {To}", to);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {To}", to);
            throw;
        }
    }

    public async Task SendRegistrationApprovedEmailAsync(string to, string fullName)
    {
        var subject = "Your Registration Has Been Approved";
        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <h2>Registration Approved</h2>
                <p>Dear {fullName},</p>
                <p>Your registration has been approved by the administrator.</p>
                <p>You can now log in to the system and start using our services.</p>
                <p>Best regards,<br>FPT Booking System</p>
            </body>
            </html>
        ";
        
        await SendEmailAsync(to, subject, body);
    }

    public async Task SendRegistrationRejectedEmailAsync(string to, string fullName, string reason)
    {
        var subject = "Your Registration Has Been Rejected";
        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <h2>Registration Rejected</h2>
                <p>Dear {fullName},</p>
                <p>Unfortunately, your registration has been rejected by the administrator.</p>
                <p><strong>Reason:</strong> {reason}</p>
                <p>If you have any questions, please contact the administrator.</p>
                <p>Best regards,<br>FPT Booking System</p>
            </body>
            </html>
        ";
        
        await SendEmailAsync(to, subject, body);
    }

    public async Task SendCampusChangeApprovedEmailAsync(string to, string fullName, string newCampusName)
    {
        var subject = "Your Campus Change Request Has Been Approved";
        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <h2>Campus Change Approved</h2>
                <p>Dear {fullName},</p>
                <p>Your campus change request has been approved.</p>
                <p><strong>New Campus:</strong> {newCampusName}</p>
                <p>Your account has been updated with the new campus information.</p>
                <p>Best regards,<br>FPT Booking System</p>
            </body>
            </html>
        ";
        
        await SendEmailAsync(to, subject, body);
    }

    public async Task SendCampusChangeRejectedEmailAsync(string to, string fullName, string reason)
    {
        var subject = "Your Campus Change Request Has Been Rejected";
        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <h2>Campus Change Rejected</h2>
                <p>Dear {fullName},</p>
                <p>Unfortunately, your campus change request has been rejected.</p>
                <p><strong>Reason:</strong> {reason}</p>
                <p>If you have any questions, please contact the administrator.</p>
                <p>Best regards,<br>FPT Booking System</p>
            </body>
            </html>
        ";
        
        await SendEmailAsync(to, subject, body);
    }

    public async Task SendBookingApprovedEmailAsync(string to, string fullName, string facilityName, DateTime startTime, DateTime endTime)
    {
        var subject = "Your Booking Has Been Approved";
        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <h2>Booking Approved</h2>
                <p>Dear {fullName},</p>
                <p>Your booking has been approved.</p>
                <p><strong>Facility:</strong> {facilityName}</p>
                <p><strong>Date & Time:</strong> {startTime:dd/MM/yyyy} from {startTime:HH:mm} to {endTime:HH:mm}</p>
                <p>Please arrive on time for your booking.</p>
                <p>Best regards,<br>FPT Booking System</p>
            </body>
            </html>
        ";
        
        await SendEmailAsync(to, subject, body);
    }

    public async Task SendBookingRejectedEmailAsync(string to, string fullName, string facilityName, string reason)
    {
        var subject = "Your Booking Has Been Rejected";
        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <h2>Booking Rejected</h2>
                <p>Dear {fullName},</p>
                <p>Unfortunately, your booking has been rejected.</p>
                <p><strong>Facility:</strong> {facilityName}</p>
                <p><strong>Reason:</strong> {reason}</p>
                <p>If you have any questions, please contact the administrator.</p>
                <p>Best regards,<br>FPT Booking System</p>
            </body>
            </html>
        ";
        
        await SendEmailAsync(to, subject, body);
    }

    public async Task SendBookingReminderEmailAsync(string to, string fullName, string facilityName, DateTime startTime)
    {
        var subject = "Reminder: Your Booking Starts Soon";
        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <h2>Booking Reminder</h2>
                <p>Dear {fullName},</p>
                <p>This is a reminder that your booking starts soon.</p>
                <p><strong>Facility:</strong> {facilityName}</p>
                <p><strong>Start Time:</strong> {startTime:dd/MM/yyyy HH:mm}</p>
                <p>Please don't forget to check in on time.</p>
                <p>Best regards,<br>FPT Booking System</p>
            </body>
            </html>
        ";
        
        await SendEmailAsync(to, subject, body);
    }

    public async Task SendPasswordResetCodeEmailAsync(string to, string fullName, string verificationCode)
    {
        var subject = "Password Reset Verification Code";
        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <h2>Password Reset Request</h2>
                <p>Dear {fullName},</p>
                <p>You have requested to reset your password.</p>
                <p>Your verification code is:</p>
                <h1 style='color: #007bff; letter-spacing: 5px; font-size: 32px;'>{verificationCode}</h1>
                <p><strong>This code will expire in 15 minutes.</strong></p>
                <p>If you did not request this password reset, please ignore this email.</p>
                <p>Best regards,<br>FPT Booking System</p>
            </body>
            </html>
        ";
        
        await SendEmailAsync(to, subject, body);
    }

    public async Task SendBookingPendingLecturerApprovalEmailAsync(string to, string lecturerName, string studentName, string facilityName, DateTime bookingDate, TimeSpan startTime, TimeSpan endTime, string purpose)
    {
        var subject = "New Booking Request Needs Your Approval";
        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <h2>New Booking Request</h2>
                <p>Dear {lecturerName},</p>
                <p>A student has submitted a booking request that requires your approval:</p>
                <div style='background-color: #f5f5f5; padding: 15px; border-radius: 5px; margin: 15px 0;'>
                    <p><strong>Student:</strong> {studentName}</p>
                    <p><strong>Facility:</strong> {facilityName}</p>
                    <p><strong>Date:</strong> {bookingDate:dddd, MMMM dd, yyyy}</p>
                    <p><strong>Time:</strong> {startTime:hh\\:mm} - {endTime:hh\\:mm}</p>
                    <p><strong>Purpose:</strong> {purpose}</p>
                </div>
                <p>Please log in to the system to review and approve/reject this booking request.</p>
                <p>Best regards,<br>FPT Booking System</p>
            </body>
            </html>
        ";
        
        await SendEmailAsync(to, subject, body);
    }
}

