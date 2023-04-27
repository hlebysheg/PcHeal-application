using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace pcHealth.Hubs
{
    public class PcHealthHub: Hub
    {
        private readonly ILogger _logger;
        public PcHealthHub (ILogger log)
        {
            _logger = log;
        }

        // Отправка сообщений
        public async Task SendAsync(string name)
        {
            await Clients.All.SendAsync(name, name);
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            _logger.LogWarning("connect");
            var user = Context.User?.Identity?.Name;
            if (user == null) { 
                await base.OnDisconnectedAsync(new Exception("not such user"));
                return;
            }

            var connectionId = Context.ConnectionId;
            await Groups.AddToGroupAsync(connectionId, user);
        }

        [Authorize]
        public async Task? LeaveRoom(string roomName)
        {
            var user = Context.User?.Identity?.Name;
            if (user == null) { return ; }

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, user);
        }
    }
}
