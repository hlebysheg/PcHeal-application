using PcHealthClientApp.Model.dto;
using PcHealthClientApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
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
	/// <summary>
	/// Логика взаимодействия для LoginWindow.xaml
	/// </summary>
	public partial class LoginWindow : Window
	{
		private static readonly HttpClient client = new HttpClient();

		public LoginWindow()
		{
			InitializeComponent();
			
		}

		private async void loginButton_Click(object sender, RoutedEventArgs e)
		{
			string queuePath = ConfigurationManager.AppSettings["hostUrl"].ToString() + "/api/UserLogin/log";
			var body = new RegisterDto
			{
				Name = loginTextBox.Text,
				Password = passwordTextBox.Password,
				Email = ""
			};
			var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
			var response = await client.PostAsync(queuePath, content);
			if (response.StatusCode != System.Net.HttpStatusCode.OK)
			{
				MessageBox.Show("fail login");
				return;
			}
			var encoding = ASCIIEncoding.ASCII;

			using (var reader = new System.IO.StreamReader(await response.Content.ReadAsStreamAsync(), encoding))
			{
				string responseText = reader.ReadToEnd();
				var str = JsonSerializer.Deserialize<RefreshTokenDto>(responseText);
				
				Properties.Settings.Default["accesToken"] = str.AccesToken;
				Properties.Settings.Default["refreshToken"] = str.RefreshToken;
				Properties.Settings.Default["userName"] = body.Name;
				Properties.Settings.Default.Save();
			}
			MessageBox.Show("succes login");
			await Task.Delay(1000);
			openMainWindow();

		}
		private void openMainWindow()
		{
			PcHealthWindow mnw = new PcHealthWindow();
			mnw.Owner = this;
			this.Hide(); // not required if using the child events below
			mnw.ShowDialog();
		}

		private void registerButton_Click(object sender, RoutedEventArgs e)
		{
			RegisterWindow mnw = new RegisterWindow();
			mnw.Owner = this;
			this.Hide(); // not required if using the child events below
			mnw.ShowDialog();
		}
	}
}
