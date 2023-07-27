using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PcHealthClientApp.Infrastructure.Fabrics
{
	public static class HubConnectionFabric
	{
		private static string host = ConfigurationManager.AppSettings["hostUrl"].ToString();//hubHost  hostUrl

		public static HubConnection Create()
		{
			HubConnection connection = new Microsoft.AspNetCore.SignalR.Client.HubConnectionBuilder()
				.WithUrl(host + "/api/pchealh/hub", opt =>
				{
					opt.Headers.Add("access_token", (Properties.Settings.Default["accesToken"].ToString()));
					opt.AccessTokenProvider = () => Task.FromResult(Properties.Settings.Default["accesToken"].ToString());
				})
				.WithAutomaticReconnect()
				.Build();

			return connection;
		}
	}
}
