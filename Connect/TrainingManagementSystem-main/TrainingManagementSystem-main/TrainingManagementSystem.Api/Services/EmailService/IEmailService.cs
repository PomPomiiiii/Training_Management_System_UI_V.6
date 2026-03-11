namespace TrainingManagementSystem.Api.Services.EmailService
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string name, string subject, string message);
    }
}
