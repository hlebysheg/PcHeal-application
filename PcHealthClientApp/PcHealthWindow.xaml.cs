

using LibreHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
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

		static Computer c = new Computer()
		{
			//RAMEnabled = true, // uncomment for RAM reports
			//MainboardEnabled = true, // uncomment for Motherboard reports
			//FanControllerEnabled = true, // uncomment for FAN Reports
			//HDDEnabled = true, // uncomment for HDD Report
		};
		//Properties.Settings.Default["accesToken"] = str.AccesToken;
		//		Properties.Settings.Default["refreshToken"] = str.RefreshToken;
		//		Properties.Settings.Default["userName"] = body.Name;
		//		Properties.Settings.Default.Save();
		public PcHealthWindow()
		{
			InitializeComponent();
			nameLabel.Content = Properties.Settings.Default["userName"] ?? "Alien";
			InfoUpdate();
		}
		private async void InfoUpdate()
		{
			while(true)
			{
				InfoListner();
				await Task.Delay(500);
			}
		}

        private void InfoListner()
		{
			c.Open();
			c.Accept(new UpdateVisitor());
            c.IsCpuEnabled = true;
			c.IsGpuEnabled = true;
			foreach (var hardware in c.Hardware)
			{

                if (hardware.HardwareType == HardwareType.Cpu)
				{
					// only fire the update when found
					hardware.Update();
					CPUNameLabel.Content = hardware.Name;
					// loop through the data
					foreach (var sensor in hardware.Sensors)
						if (sensor.SensorType == SensorType.Temperature)
						{
							// store
							hardware.Update();
							CPUTempLabel.Content = ((sensor.Value.GetValueOrDefault())).ToString("#.##");
							// print to console
							System.Diagnostics.Debug.WriteLine("cpuTemp: " + sensor.Value.GetValueOrDefault());

						}
						else if (sensor.SensorType == SensorType.Load && sensor.Name.Contains("CPU Total"))
						{
							// store
							//cpuUsage = sensor.Value.GetValueOrDefault();
							// print to console
							System.Diagnostics.Debug.WriteLine("cpuUsage: " + sensor.Value.GetValueOrDefault());

						}
						else if (sensor.SensorType == SensorType.Power && sensor.Name.Contains("CPU Package"))
						{
							// store
							//cpuPowerDrawPackage = sensor.Value.GetValueOrDefault();
							// print to console
							System.Diagnostics.Debug.WriteLine("CPU Power Draw - Package: " + sensor.Value.GetValueOrDefault());


						}
						else if (sensor.SensorType == SensorType.Clock && sensor.Name.Contains("CPU Core #1"))
						{
							// store
							//cpuFrequency = sensor.Value.GetValueOrDefault();
							// print to console
							System.Diagnostics.Debug.WriteLine("cpuFrequency: " + sensor.Value.GetValueOrDefault());
						}
				}
			}
			//CPUNameLabel.Content = "";
			//GPUNameLabel.Content = "";
			//CPUTempLabel.Content = "";
			//GPUTempLabel.Content = "";
		}

		private async void Button_Click(object sender, RoutedEventArgs e)
		{
			Properties.Settings.Default["accesToken"] = "";
			Properties.Settings.Default["refreshToken"] = "";
			Properties.Settings.Default["userName"] = "";
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

			base.OnClosing(e);
			c.Close();
		}
	}
}
