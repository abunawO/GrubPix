namespace GrubPix.Application.Services.Interfaces
{
    public interface INotificationService
    {
        Task SendNotificationAsync(string recipient, string subject, string message);
    }
}
