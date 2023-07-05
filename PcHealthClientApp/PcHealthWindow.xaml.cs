

using LibreHardwareMonitor.Hardware;
using Microsoft.AspNetCore.SignalR.Client;
using PcHealthClientApp.Model.dto;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PcHealthClientApp
{
    public class UpdateVisitor : IVisitor
    {
        public void VisitComputer(IComputer computer)
        {
            computer.Traverse(this);
        }
        public void VisitHardware(IHardware hardware)
        {
            hardware.Update();
            foreach (IHardware subHardware in hardware.SubHardware) subHardware.Accept(this);
        }
        public void VisitSensor(ISensor sensor) { }
        public void VisitParameter(IParameter parameter) { }
    }
    /// <summary>
    /// Логика взаимодействия для PcHealthWindow.xaml
    /// </summary>
    public partial class PcHealthWindow : Window
	{
		private static readonly HttpClient client = new HttpClient();

        private static bool canNotifty = true;

		private readonly int criticalTemp = 86;

        private static System.Timers.Timer aTimer;

        private CancellationTokenSource cancelTokenSource = new CancellationTokenSource();

		private bool isOpenConnection = true;
		
		private HubConnection connection;

		static Computer c = new Computer();
        private static void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            canNotifty = true;
        }
        public PcHealthWindow()
		{
			InitializeComponent();
			nameLabel.Content = Properties.Settings.Default["userName"] ?? "Alien";
			string host = ConfigurationManager.AppSettings["hostUrl"].ToString();//hubHost  hostUrl
			connection = new Microsoft.AspNetCore.SignalR.Client.HubConnectionBuilder()
				.WithUrl(host + "/api/pchealh/hub", opt =>
				{
					opt.Headers.Add("access_token", (Properties.Settings.Default["accesToken"].ToString()));
					opt.AccessTokenProvider = () => Task.FromResult(Properties.Settings.Default["accesToken"].ToString());
				})
				.WithAutomaticReconnect()
				.Build();
			InfoUpdate();
			connection.On<PCInfoMessage>("Receive", (info) =>
			{
				Dispatcher.Invoke(() =>
				{
					System.Diagnostics.Debug.WriteLine(info.CPUName);
				});
			});
			connection.On<string>("notify", (info) =>
			{
				Dispatcher.Invoke(() =>
				{
					MessageBox.Show(info);
				});
			});
            
            aTimer = new System.Timers.Timer();
            aTimer.Elapsed += OnTimedEvent;
            aTimer.Interval = 10000 * 6 * 10;
            aTimer.Enabled = true;
        }

        private void ResetNotifyTimer()
		{
			aTimer.Stop();
			aTimer.Start();
		}
		private async void InfoUpdate()
		{
			//CancellationToken token = cancelTokenSource.Token;
			//Task tsk = new Task(async () =>
			//{
			//	while (true)
			//	{
			//		await InfoListner();
			//		await Task.Delay(500);
			//	}
			//}, token);
			//tsk.Start();
			while (isOpenConnection)
			{
				await InfoListner();
				await Task.Delay(500);
			}
		}

		private async Task InfoListner()
		{
			c.Open();
			c.Accept(new UpdateVisitor());
            c.IsCpuEnabled = true;
			c.IsGpuEnabled = true;
			c.IsMotherboardEnabled = true;

			PCInfoMessage message = new PCInfoMessage();

			foreach (var hardware in c.Hardware)
			{

                if (hardware.HardwareType == HardwareType.Cpu)
				{
					// only fire the update when found
					CPUNameLabel.Content = hardware.Name;
					message.CPUName = hardware.Name;
					// loop through the data
					foreach (var sensor in hardware.Sensors)
						if (sensor.SensorType == SensorType.Temperature)
						{
							// store
							CPUTempLabel.Content = ((sensor.Value.GetValueOrDefault())).ToString("#.##") + "°C";
							// print to console
							message.CPUTemp = sensor.Value.GetValueOrDefault();
							//System.Diagnostics.Debug.WriteLine("cpuTemp: " + sensor.Value.GetValueOrDefault());

						}
						else if (sensor.SensorType == SensorType.Load && sensor.Name.Contains("CPU Total"))
						{
							// store
							//cpuUsage = sensor.Value.GetValueOrDefault();
							// print to console
							CPUUsageLabel.Content = sensor.Value.GetValueOrDefault().ToString("#.##") + "%";
							message.CPULoad = sensor.Value.GetValueOrDefault();
							//System.Diagnostics.Debug.WriteLine("cpuUsage: " + sensor.Value.GetValueOrDefault());

						}
						else if (sensor.SensorType == SensorType.Power && sensor.Name.Contains("CPU Package"))
						{
							// store
							//cpuPowerDrawPackage = sensor.Value.GetValueOrDefault();
							// print to console
							//System.Diagnostics.Debug.WriteLine("CPU Power Draw - Package: " + sensor.Value.GetValueOrDefault());


						}
						else if (sensor.SensorType == SensorType.Clock && sensor.Name.Contains("CPU Core #1"))
						{
							// store
							//cpuFrequency = sensor.Value.GetValueOrDefault();
							// print to console
							CPUFrenqLabel.Content = sensor.Value.GetValueOrDefault().ToString("#.##") + "GHz";
							message.CPUFrenq = sensor.Value.GetValueOrDefault();
							//System.Diagnostics.Debug.WriteLine("cpuFrequency: " + sensor.Value.GetValueOrDefault());
						}
				}

                if (hardware.HardwareType == HardwareType.GpuNvidia || hardware.HardwareType == HardwareType.GpuAmd || hardware.HardwareType == HardwareType.GpuIntel)
                {
                    // only fire the update when found
                    hardware.Update();
                    GPUNameLabel.Content = hardware.Name;
					message.GPUName = hardware.Name;
					// loop through the data
					foreach (var sensor in hardware.Sensors)
                        if (sensor.SensorType == SensorType.Temperature)
                        {
                            // store
                            hardware.Update();
                            GPUTempLabel.Content = ((sensor.Value.GetValueOrDefault())).ToString("#.##");
							// print to console
							message.GPUTemp = sensor.Value.GetValueOrDefault();

							//System.Diagnostics.Debug.WriteLine("cpuTemp: " + sensor.Value.GetValueOrDefault());

                        }
                        else if (sensor.SensorType == SensorType.Load && sensor.Name.Contains("CPU Total"))
                        {
							// store
							//cpuUsage = sensor.Value.GetValueOrDefault();
							// print to console
							GPUUsageLabel.Content = sensor.Value.GetValueOrDefault();
							message.GPULoad = sensor.Value.GetValueOrDefault();
							//System.Diagnostics.Debug.WriteLine("cpuUsage: " + sensor.Value.GetValueOrDefault());

                        }
                        else if (sensor.SensorType == SensorType.Power && sensor.Name.Contains("CPU Package"))
                        {
                            // store
                            //cpuPowerDrawPackage = sensor.Value.GetValueOrDefault();
                            // print to console
                            //System.Diagnostics.Debug.WriteLine("CPU Power Draw - Package: " + sensor.Value.GetValueOrDefault());


                        }
                        else if (sensor.SensorType == SensorType.Clock && sensor.Name.Contains("CPU Core #1"))
                        {
                            // store
                            //cpuFrequency = sensor.Value.GetValueOrDefault();
                            // print to console
                            //System.Diagnostics.Debug.WriteLine("cpuFrequency: " + sensor.Value.GetValueOrDefault());
                        }
                }
            }
			SendInfo(message);
		}

		private async void SendInfo(PCInfoMessage msg)
		{
			try
			{
				// отправка сообщения
				await connection.InvokeAsync("Send", msg);
				if(msg.CPUTemp > criticalTemp && canNotifty)
				{
					await connection.InvokeAsync("Notify", msg.CPUTemp);
					ResetNotifyTimer();
					canNotifty =false;

                }
				ConnectionStatus.IsChecked = true;
				ConnectionStatus.Content = "Подключено";
			}
			catch (Exception ex)
			{
				ConnectionStatus.IsChecked = false;
				ConnectionStatus.Content = "Не подключено";
			}
		}

		private async void Window_Loaded(object sender, RoutedEventArgs e)
		{
			await conect();
		}
		private async Task conect()
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
		private async void Button_Click(object sender, RoutedEventArgs e)
		{
			Properties.Settings.Default["accesToken"] = "";
			Properties.Settings.Default["refreshToken"] = "";
			Properties.Settings.Default["userName"] = "";
			Properties.Settings.Default.Save();
			isOpenConnection = false;
			await connection.StopAsync();
			await Task.Delay(1000);
			openLoginWindow();
		}

		private void openLoginWindow()
		{
			LoginWindow mnw = new LoginWindow();
			mnw.Owner = this;
			this.Hide(); // not required if using the child events below
			mnw.ShowDialog();
		}

		protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
		{
			//do my stuff before closing
			c.Close();
			cancelTokenSource.Cancel();
			isOpenConnection = false;
			base.OnClosing(e);
			
		}

		private async void Button_Click_1(object sender, RoutedEventArgs e)
		{
			try
			{
				await connection.InvokeAsync("Notify", 100);
			}
			catch (Exception ex)
			{
			}
		}

		private async void Button_Click_2(object sender, RoutedEventArgs e)
		{
			await conect();
		}
	}
}
