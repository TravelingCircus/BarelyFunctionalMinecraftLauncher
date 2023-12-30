using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using BFML.Core;
using Common;
using Common.Misc;
using Common.Models;

namespace BFML.WPF;

public partial class LogInWindow
{
    private readonly IFileClient _fileClient;
    private readonly LaunchConfiguration _launchConfiguration;
    private readonly ConfigurationVersion _version;

    public LogInWindow() { }

    public LogInWindow(IFileClient fileClient, LaunchConfiguration launchConfiguration, ConfigurationVersion version)
    {
        InitializeComponent();
        _version = version;
        _fileClient = fileClient;
        _launchConfiguration = launchConfiguration;
    }

    private void ShutDown(object sender, RoutedEventArgs e)
    {
        try
        {
            Application.Current.Shutdown();
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
        if (e.LeftButton != MouseButtonState.Pressed) return;
        if (WindowState == WindowState.Maximized)
        {
            WindowState = WindowState.Normal;
            Application.Current.MainWindow.Top = 3;
        }

        DragMove();
    }

    private async void RegisterButtonOnClick(object sender, RoutedEventArgs e)
    {
        string nickname = InputNickname.Text;
        ValidateString(nickname);
        string password = InputPassword.Text;
        ValidateString(password);

        User newUser = new User(nickname, password);
        Result<User> registrationResult = await _fileClient.CreateRecord(newUser);
        if (!registrationResult.IsOk)
        {
            //TODO Display that user exists
            return;
        }

        await TryLogIn(registrationResult.Value);
    }

    private async void LogInButtonOnClick(object sender, RoutedEventArgs e)
    {
        string nickname = InputNickname.Text;
        string password = InputPassword.Text;
        User user = new User(nickname, password);
        
        await TryLogIn(user);
    }

    private async Task TryLogIn(User user)
    {
        Result<User> loginResult = await _fileClient.Authenticate(user);
        if (!loginResult.IsOk)
        {
            //TODO Display that user doesn't exists
            return;
        }

        User authenticatedUser = loginResult.Value;
        LocalPrefs.SaveLocalPrefs(authenticatedUser.Nickname, authenticatedUser.PasswordHash);
        LocalPrefs localPrefs = LocalPrefs.GetLocalPrefs();
        MainWindow mainWindow = new MainWindow(_fileClient, loginResult.Value, localPrefs, _launchConfiguration, _version);
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

    private static void ValidateString(string text)
    {
        if (string.IsNullOrEmpty(text)) throw new NullReferenceException();
        if (!IsLegalUnicode(text)) throw new Exception($"[{text}] isn't valid unicode");
        if (text.Length is < 4 or > 16) throw new Exception("Too long or too short");
    }
    
    private static bool IsLegalUnicode(string str)
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