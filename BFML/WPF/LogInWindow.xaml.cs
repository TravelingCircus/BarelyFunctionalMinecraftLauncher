using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using BFML.Core;
using CommonData.Models;
using CommonData.Network.Messages.Login;
using CommonData.Network.Messages.Registration;
using TCPFileClient;

namespace BFML.WPF
{
    public partial class LogInWindow : Window
    {
        private readonly FileClient _fileClient;
        private readonly LaunchConfiguration _launchConfiguration;
        private readonly ConfigurationVersion _version;

        public LogInWindow(FileClient fileClient, LaunchConfiguration launchConfiguration, ConfigurationVersion version)
        {
            InitializeComponent();
            _fileClient = fileClient;
            _launchConfiguration = launchConfiguration;
            _version = version;
        }

        private void ShutDown(object sender, RoutedEventArgs e)
        {
            try
            {
                App.Current.Shutdown();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void Minimize(object sender, RoutedEventArgs e)
        {
            try
            {
                this.WindowState = WindowState.Minimized;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void MoveWindow(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (this.WindowState == WindowState.Maximized)
                {
                    this.WindowState = WindowState.Normal;
                    Application.Current.MainWindow.Top = 3;
                }
                this.DragMove();
            }
        }

        private async void RegisterButtonOnClick(object sender, RoutedEventArgs e)
        {
            string nickname = InputNickname.Text;
            string password = InputPassword.Text;

            User newUser = new User(nickname, password);
            RegistrationResponse response = await _fileClient.SendRegistrationRequest(newUser);
            if (!response.Success)
            {
                //TODO Display that user exists
                InputNickname.Text = "";
                InputPassword.Text = "";
                return;
            }

            LocalPrefs.SaveLocalPrefs(nickname, password);
            LocalPrefs localPrefs = LocalPrefs.GetLocalPrefs();
            MainWindow mainWindow = new MainWindow(_fileClient, newUser, localPrefs, _launchConfiguration, _version);
            mainWindow.Show();
            Close();
        }

        private async void LogInButtonOnClick(object sender, RoutedEventArgs e)
        {
            string nickname = InputNickname.Text;
            string password = InputPassword.Text;

            User newUser = new User(nickname, password);
            LoginResponse response = await _fileClient.SendLoginRequest(newUser);
            if (!response.Success)
            {
                //TODO Display that user doesn't exists
                InputNickname.Text = "";
                InputPassword.Text = "";
                return;
            }

            LocalPrefs.SaveLocalPrefs(nickname, password);
            LocalPrefs localPrefs = LocalPrefs.GetLocalPrefs();
            MainWindow mainWindow = new MainWindow(_fileClient, newUser, localPrefs, _launchConfiguration, _version);
            mainWindow.Show();
            Close();
        }

        private void TextNicknameMouseDown(object sender, MouseButtonEventArgs e)
        {
            InputNickname.Focus();
        }

        private void InputNicknameTextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(InputNickname.Text) && InputNickname.Text.Length > 0)
            {
                TextNickname.Visibility = Visibility.Collapsed;
            }
            else
            {
                TextNickname.Visibility = Visibility.Visible;
            }
        }

        private void TextPasswordMouseDown(object sender, MouseButtonEventArgs e)
        {
            InputPassword.Focus();
        }

        private void InputPasswordPasswordChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(InputPassword.Text) && InputPassword.Text.Length > 0)
            {
                TextPassword.Visibility = Visibility.Collapsed;
            }
            else
            {
                TextPassword.Visibility = Visibility.Visible;
            }
        }

    }
}
