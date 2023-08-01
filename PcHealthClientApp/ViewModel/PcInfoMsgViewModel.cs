using LibreHardwareMonitor.Hardware;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.AspNetCore.SignalR.Client;
using PcHealthClientApp.Infrastructure.Fabrics;
using PcHealthClientApp.Model.Domain;
using PcHealthClientApp.Model.dto;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace PcHealthClientApp.ViewModel
{
	public class PcInfoMsgViewModel: INotifyPropertyChanged
	{
		
		private readonly DispatcherTimer _timer;
		private readonly DispatcherTimer _saveTimer;
		private CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
		private PcInfo _currentInfo;
		private bool _connectionStatus;
		private bool isOpenConnection = true;
		public ICommand ButtonCommand { get; set; }

		public ICommand ButtonNotifyCommand { get; set; }

		private HubConnection connection;
		public PcInfoMsgViewModel()
		{
			ButtonCommand = new RelayCommand(async o => await conect("ConnectButton"));
			ButtonNotifyCommand = new RelayCommand(async o => await Notify("ConnectButton"));
			connection = HubConnectionFabric.Create();
			conect("");
			_timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
			_saveTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(60) };
			_timer.Start();
			_saveTimer.Start();
			_saveTimer.Tick += async (o, e) => await SaveListner();
			_timer.Tick += async (o, e) => await InfoListner();
		}
		public PcInfo CurrentInfo
		{
			get { return _currentInfo; }
			set
			{
				_currentInfo = value;
				OnPropertyChanged("CurrentInfo");
			}
		}

		public bool ConnectionStatus
		{
			get { return _connectionStatus; }
			set
			{
				_connectionStatus = value;
				OnPropertyChanged("ConnectionStatus");
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged([CallerMemberName] string prop = "")
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(prop));
		}

		private async Task InfoListner()
		{
			//PCInfoMessage message = new PCInfoMessage();
			PcInfo info = PcInfoFabric.Create();
			CurrentInfo = info;
			SendInfo(info);
		}

		private async Task SaveListner()
		{
			//PCInfoMessage message = new PCInfoMessage();
			PcInfo info = PcInfoFabric.Create();
			CurrentInfo = info;
			try
			{
				await connection.InvokeAsync("Save", new PCInfoMessage(info));
				ConnectionStatus = true;
			}
			catch (Exception ex)
			{
				ConnectionStatus = false;
			}
		}

		private async void SendInfo(PcInfo info)
		{
			try
			{
				await connection.InvokeAsync("Send", new PCInfoMessage(info));
				ConnectionStatus = true;
			}
			catch (Exception ex)
			{
				ConnectionStatus = false;
			}
		}

		private async Task conect(object sender)
		{
			try
			{
				// подключемся к хабу
				await connection.StartAsync();
			}
			catch (Exception ex)
			{
			}
		}

		private async Task Notify(object sender)
		{
			try
			{
				await connection.InvokeAsync("Notify", 100);
			}
			catch (Exception ex)
			{
			}
		}
	}
}
