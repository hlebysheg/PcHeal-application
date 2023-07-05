using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using WordBook.Model.dto;
namespace WordBook.Hubs
{
    public class PcHealthHub: Hub
    {
		[Authorize]
		public async Task Send(PCInfoMessage pc)
		{
            var ctx = Context.GetHttpContext();
            var name = ctx?.User?.Identity?.Name;

            await Clients.Group(name).SendAsync("PCInfo", pc);
		}
		[Authorize]
		public async Task Notify(float temp)
		{
            var ctx = Context.GetHttpContext();
            var name = ctx?.User?.Identity?.Name;

            await Clients.Group(name).SendAsync("notify", $"Температура превысила {temp} °C") ;
		}
		[Authorize]
		public override async Task OnConnectedAsync()
		{
			var ctx = Context.GetHttpContext();
			var name = ctx?.User?.Identity?.Name;

            await base.OnConnectedAsync();
			var connectionId = Context.ConnectionId;
			await Groups.AddToGroupAsync(connectionId, name);
		}
	}
}
