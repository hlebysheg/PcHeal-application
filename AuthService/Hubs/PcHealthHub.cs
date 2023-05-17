using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using WordBook.Model.dto;
namespace WordBook.Hubs
{
    public class PcHealthHub: Hub
    {
		// Отправка сообщений
		public async Task Send(PCInfoMessage pc)
		{
            var ctx = Context.GetHttpContext();
            var name = ctx?.User?.Identity?.Name;
            name = name == "" || name == null ? "Alien!" : name;

            await Clients.Group(name).SendAsync("PCInfo", pc);
		}

		public async Task Notify(float temp)
		{
            var ctx = Context.GetHttpContext();
            var name = ctx?.User?.Identity?.Name;
            name = name == "" || name == null ? "Alien!" : name;

            await Clients.Group(name).SendAsync("notify", $"Температура превысила {temp} °C") ;
		}

		public override async Task OnConnectedAsync()
		{
			var ctx = Context.GetHttpContext();
			var name = ctx?.User?.Identity?.Name;
			name = name == "" || name == null ? "Alien!" : name;

            await base.OnConnectedAsync();
			var connectionId = Context.ConnectionId;
			await Groups.AddToGroupAsync(connectionId, name);
		}
	}
}
