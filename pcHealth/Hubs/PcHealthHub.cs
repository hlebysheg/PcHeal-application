using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace pcHealth.Hubs
{
    public class PcHealthHub: Hub
    {
		// Отправка сообщений
		public async Task Send(string username, string message)
		{
			await this.Clients.All.SendAsync("Receive", username, message);
		}

		public override async Task OnConnectedAsync()
		{
			var user = Context.User?.Identity?.Name;
			if (user == null)
			{
				await base.OnDisconnectedAsync(new Exception("not such user"));
				return;
			}

			await base.OnConnectedAsync();
			var connectionId = Context.ConnectionId;
			await Groups.AddToGroupAsync(connectionId, user);
		}
	}
}
