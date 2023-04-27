using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace AuthService.Hubs
{
    public class PcHealthHub: Hub
    {

        // Отправка сообщений
        public async Task SendAsync(string name, string message)
        {
            await Clients.All.SendAsync(name, message);
        }

        // Подключение нового пользователя
        [Authorize]
        public async Task ConnectAsync()
        {
            var user = Context.User?.Identity?.Name;
            if(user == null) { return; }

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
