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
    /// <summary>
    /// Interaction logic for LogInWindow.xaml
    /// </summary>
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
            //string nickname = NicknameBar.Text;
            //string password = PasswordBar.Text;

            //User newUser = new User(nickname, password);
            //RegistrationResponse response = await _fileClient.SendRegistrationRequest(newUser);
            //if (!response.Success)
            //{
            //    //TODO Display that user exists
            //    return;
            //}

            //LocalPrefs.SaveLocalPrefs(nickname, password);
            //LocalPrefs localPrefs = LocalPrefs.GetLocalPrefs();
            //MainWindow mainWindow = new MainWindow(_fileClient, newUser, localPrefs, _launchConfiguration, _version);
        }

        private async void LogInButtonOnClick(object sender, RoutedEventArgs e)
        {
            //string nickname = NicknameBar.Text;
            //string password = PasswordBar.Text;

            //User newUser = new User(nickname, password);
            //LoginResponse response = await _fileClient.SendLoginRequest(newUser);
            //if (!response.Success)
            //{
            //    //TODO Display that user doesn't exists
            //    return;
            //}

            //LocalPrefs.SaveLocalPrefs(nickname, password);
            //LocalPrefs localPrefs = LocalPrefs.GetLocalPrefs();
            //MainWindow mainWindow = new MainWindow(_fileClient, newUser, localPrefs, _launchConfiguration, _version);
        }

        public LogInWindow()
        {
        }

        private void DebugButtonOnClick(object sender, RoutedEventArgs e)
        {

            //string password = PasswordBar.Text;
        }

        private void textNickname_MouseDown(object sender, MouseButtonEventArgs e)
        {
            inputNickname.Focus();
        }

        private void inputNickname_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(inputNickname.Text) && inputNickname.Text.Length > 0)
            {
                textNickname.Visibility = Visibility.Collapsed;
            }
            else
            {
                textNickname.Visibility = Visibility.Visible;
            }
        }

        private void textPassword_MouseDown(object sender, MouseButtonEventArgs e)
        {
            inputPassword.Focus();
        }

        private void inputPassword_PasswordChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(inputPassword.Text) && inputPassword.Text.Length > 0)
            {
                textPassword.Visibility = Visibility.Collapsed;
            }
            else
            {
                textPassword.Visibility = Visibility.Visible;
            }


        }

    }
}
