using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using pcHealth.Model;

namespace pcHealth.Hubs
{
    public class PcHealthHub: Hub
    {
		// Отправка сообщений
		public async Task Send(PCInfoMessage pc)
		{
			var ctx = Context.GetHttpContext();
			var userName = ctx.Request.Headers["username"].ToString() ?? "";

			await Clients.Group(userName).SendAsync("PCInfo", pc);
		}

		public async Task Notify(float temp)
		{
			var ctx = Context.GetHttpContext();
			var userName = ctx.Request.Headers.FirstOrDefault(el => el.Key == "username");

			await Clients.Group(userName.Value).SendAsync("notify", $"Температура превысила {temp} °C") ;
		}

		public override async Task OnConnectedAsync()
		{
			//var re = Request.Headers.FirstOrDefault(el => el.Key == "username");
			var ctx = Context.GetHttpContext();

			var userName = ctx.Request.Headers["username"].ToString() ?? "";

			await base.OnConnectedAsync();
			var connectionId = Context.ConnectionId;
			await Groups.AddToGroupAsync(connectionId, userName);
		}
	}
}
