using PcHealthClientApp.Properties;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace PcHealthClientApp
{
	/// <summary>
	/// Логика взаимодействия для App.xaml
	/// </summary>
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);
			if (Settings.Default["accesToken"].ToString() != "")
			{
				PcHealthWindow ws = new PcHealthWindow();
				ws.Show();
			}
			else
			{
				LoginWindow ws = new LoginWindow();
				ws.Show();
			}
		}
	}
}
