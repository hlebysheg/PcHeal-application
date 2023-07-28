using PcHealthClientApp.Model.dto;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
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
	/// Логика взаимодействия для RegisterWindow.xaml
	/// </summary>
	public partial class RegisterWindow : Window
	{
		private static readonly HttpClient client = new HttpClient();

		public RegisterWindow()
		{
			InitializeComponent();
		}
		private bool ValidateRequireForm()
		{
			string email = emailTextBox.Text;
			string login = loginTextBox.Text;
			string password = passwordTextBox.Password;

			bool isRequire = true;

			if (email.Length < 6)
			{
				emailLabel.Content = "email less then 6 character";
				emailLabel.Foreground = System.Windows.Media.Brushes.Red;
				isRequire =  false;
			}
			if (login.Length < 6)
			{
				loginLabel.Content = "login less then 6 character";
				loginLabel.Foreground = System.Windows.Media.Brushes.Red;
				isRequire = false;
			}
			if (password.Length < 6)
			{
				passwordLabel.Content = "password less then 6 character";
				passwordLabel.Foreground = System.Windows.Media.Brushes.Red;
				isRequire = false;
			}

			return isRequire;
		}
		private bool ValidateForm()
		{
			if (!ValidateRequireForm())
			{
				return false;
			}

			string email = emailTextBox.Text;
			string login = loginTextBox.Text;
			string password = passwordTextBox.Password;
			return true;
		}

		private async void registerButton_Click(object sender, RoutedEventArgs e)
		{
			if (!ValidateForm())
			{
				return;
			}

			string queuePath = ConfigurationManager.AppSettings["hostUrl"].ToString() + "/api/UserLogin/reg";
			var body = new RegisterDto
			{
				Email = emailTextBox.Text,
				Name = loginTextBox.Text,
				Password = passwordTextBox.Password
			};
			var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
			try
			{
				var response = await client.PostAsync(queuePath, content);
				if (response.StatusCode == System.Net.HttpStatusCode.OK)
				{
					bool? Result = new MessageBoxCustom("Register succes", MessageType.Confirmation, MessageButtons.Ok).ShowDialog();
				}
				else
				{
					bool? Result = new MessageBoxCustom("Fail register, try another email or name", MessageType.Confirmation, MessageButtons.Ok).ShowDialog();
				}
			}
			catch
			{
				bool? Result = new MessageBoxCustom("Connection error", MessageType.Error, MessageButtons.Ok).ShowDialog();
			}
			
		}
		private void loginButton_Click(object sender, RoutedEventArgs e)
		{
			LoginWindow mnw = new LoginWindow();
			mnw.Owner = this;
			this.Hide(); // not required if using the child events below
			mnw.ShowDialog();
		}

		private void emailTextBox_GotFocus(object sender, RoutedEventArgs e)
		{
			emailLabel.Content = "Email";
			emailLabel.Foreground = System.Windows.Media.Brushes.Black;
		}

		private void loginTextBox_GotFocus(object sender, RoutedEventArgs e)
		{
			loginLabel.Content = "Login";
			loginLabel.Foreground = System.Windows.Media.Brushes.Black;
		}

		private void passwordTextBox_GotFocus(object sender, RoutedEventArgs e)
		{
			passwordLabel.Content = "Password";
			passwordLabel.Foreground = System.Windows.Media.Brushes.Black;
		}


	}
}
