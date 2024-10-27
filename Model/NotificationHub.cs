using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace BackendNETAPI.Model
{
    public class NotificationHub:Hub
    {
        public async Task SendNotification(string message)
        {
            await Clients.All.SendAsync("ReceiveNotification", message);
        }
        public async Task SendOrderCountUpdate(int count)
        {
            await Clients.All.SendAsync("ReceiveOrderCountUpdate", count);
        }
    }
}
