using System.Threading.Tasks;
using GrubPix.Application.Services.Interfaces;

namespace GrubPix.Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        public async Task SendNotificationAsync(string recipient, string subject, string message)
        {
            // Placeholder for actual notification logic
            await Task.Run(() =>
            {
                Console.WriteLine($"Notification sent to {recipient} with subject '{subject}' and message: {message}");
            });
        }
    }
}
