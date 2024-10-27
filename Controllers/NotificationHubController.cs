using BackendNETAPI.Model; // Adjust this if necessary
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace BackendNETAPI.Controllers
{
    [Route("api/[controller]")] // Move this attribute to the class level
    [ApiController]
    public class NotificationController : ControllerBase // Remove nested class
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationController(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        [HttpPost("send")] // This attribute is fine as is
        public async Task<IActionResult> SendNotification([FromBody] string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return BadRequest(new { Status = "Message cannot be empty." });
            }

            await _hubContext.Clients.All.SendAsync("ReceiveNotification", message);
            return Ok(new { Status = "Notification sent!" });
        }
    }
}
