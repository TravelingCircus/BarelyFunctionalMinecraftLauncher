using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using BFML.Core;
using Common.Models;
using Common.Network.Messages.Login;
using Common.Network.Messages.Registration;
using TCPFileClient;

namespace BFML.WPF;

public partial class LogInWindow : Window
{
    private readonly FileClient _fileClient;
    private readonly LaunchConfiguration _launchConfiguration;
    private readonly ConfigurationVersion _version;

    public LogInWindow()
    {
    }

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
            WindowState = WindowState.Minimized;
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
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
                Application.Current.MainWindow.Top = 3;
            }

            DragMove();
        }
    }

    private async void RegisterButtonOnClick(object sender, RoutedEventArgs e)
    {
        string nickname = InputNickname.Text;
        ValidateString(nickname);
        string password = InputPassword.Text;
        ValidateString(password);

        User newUser = new User(nickname, password);
        RegistrationResponse response = await _fileClient.SendRegistrationRequest(newUser);
        if (!response.Success)
        {
            //TODO Display that user exists
            return;
        }

        await TryLogIn(nickname, password);
    }

    private async void LogInButtonOnClick(object sender, RoutedEventArgs e)
    {
        string nickname = InputNickname.Text;
        ValidateString(nickname);
        string password = InputPassword.Text;
        ValidateString(password);

        await TryLogIn(nickname, password);
    }

    private async Task TryLogIn(string nickname, string password)
    {
        User newUser = new User(nickname, password);
        LoginResponse response = await _fileClient.SendLoginRequest(newUser);
        newUser = response.User;
        if (!response.Success)
        {
            //TODO Display that user doesn't exists
            return;
        }

        LocalPrefs.SaveLocalPrefs(nickname, password);
        LocalPrefs localPrefs = LocalPrefs.GetLocalPrefs();
        MainWindow mainWindow = new MainWindow(_fileClient, newUser, localPrefs, _launchConfiguration, _version);
        mainWindow.Show();
        Close();
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
    
    private void TextNicknameMouseDown(object sender, MouseButtonEventArgs e)
    {
        InputNickname.Focus();
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

    private void ValidateString(string text)
    {
        if (String.IsNullOrEmpty(text)) throw new NullReferenceException();
        if (!IsLegalUnicode(text)) throw new Exception($"[{text}] isn't valid unicode");
        if (text.Length < 4 || text.Length > 16) throw new Exception("Too long or too short");
    }
    
    private bool IsLegalUnicode(string str)
    {
        for (int i = 0; i < str.Length; i++)
        {
            UnicodeCategory uc = char.GetUnicodeCategory(str, i);

            if (uc == UnicodeCategory.Surrogate)
            {
                return false;
            }
            if (uc == UnicodeCategory.OtherNotAssigned)
            {
                return false;
            }
            if (char.IsHighSurrogate(str, i))
            {
                i++;
            }
        }

        return true;
    }
}