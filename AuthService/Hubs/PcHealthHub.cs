﻿
using AuthService.Infrastructure.Service.PcStat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using WordBook.Model.dto;

namespace WordBook.Hubs
{
    public class PcHealthHub: Hub
    {
		private readonly IPcStatService _statService;
		public PcHealthHub(IPcStatService statService) 
		{
			_statService = statService;
		}
		[Authorize]
		public async Task Send(PCInfoMessage pc)
		{
            var ctx = Context.GetHttpContext();
            var name = ctx?.User?.Identity?.Name;

            await Clients.Group(name).SendAsync("PCInfo", pc);
		}
		[Authorize]
		public async Task Save(PCInfoMessage pc)
		{
			var ctx = Context.GetHttpContext();
			var name = ctx?.User?.Identity?.Name;
			_statService.SaveStat(pc, name);
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
