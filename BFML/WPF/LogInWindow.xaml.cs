using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using BFML.Core;
using BFML.Repository;
using Common.Misc;
using Common.Models;
using Utils;
using Utils.Async;

namespace BFML.WPF;

public partial class LogInWindow
{
    private readonly CentralizedModeRepo _repo;

    internal LogInWindow() { }

    internal LogInWindow(CentralizedModeRepo repo)
    {
        InitializeComponent();
        _repo = repo;
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
        string password = InputPassword.Text;
        ValidateString(nickname);
        ValidateString(password);

        User newUser = new User(nickname, password);
        Result<User> registrationResult = await _repo.CreateRecord(newUser);
        if (!registrationResult.IsOk)
        {
            //TODO Display that user exists
            return;
        }

        User registeredUser = registrationResult.Value;
        TryLogIn(registeredUser).FireAndForget();
    }

    private void LogInButtonOnClick(object sender, RoutedEventArgs e)
    {
        string nickname = InputNickname.Text;
        string password = InputPassword.Text;
        ValidateString(nickname);
        ValidateString(password);
        
        User user = new User(nickname, password);
        TryLogIn(user).FireAndForget();
    }

    private async Task TryLogIn(User user)
    {
        Result<User> loginResult = await _repo.Authenticate(user);
        if (!loginResult.IsOk)
        {
            //TODO Display that user doesn't exists
            return;
        }

        User authenticatedUser = loginResult.Value;
        LocalPrefs localPrefs = _repo.LocalPrefs;
        localPrefs.Nickname = authenticatedUser.Nickname;
        localPrefs.PasswordHash = authenticatedUser.PasswordHash;
        await _repo.SaveLocalPrefs(localPrefs);
        //MainWindow mainWindow = new MainWindow(_repo, loginResult.Value); //TODO CentrilizedModeWindow
        //mainWindow.Show();
        Close();
    }

    private void TextNicknameMouseDown(object sender, MouseButtonEventArgs e)
    {
        InputNickname.Focus();
    }

    private void TextPasswordMouseDown(object sender, MouseButtonEventArgs e)
    {
        InputPassword.Focus();
    }

    private void InputNicknameTextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) 
    {
        bool hasText = !string.IsNullOrEmpty(InputNickname.Text) && InputNickname.Text.Length > 0;
        TextNickname.Visibility = hasText 
            ? Visibility.Collapsed
            : Visibility.Visible;
    }

    private void InputPasswordPasswordChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
        bool hasText = !string.IsNullOrEmpty(InputPassword.Text) && InputPassword.Text.Length > 0;
        TextPassword.Visibility = hasText 
            ? Visibility.Collapsed
            : Visibility.Visible;
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