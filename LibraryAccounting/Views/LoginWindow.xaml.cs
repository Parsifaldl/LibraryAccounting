using LibraryAccounting.Services;
using System.Windows;

namespace LibraryAccounting.Views;

public partial class LoginWindow : Window
{
    private readonly AuthService _authService = new();

    public LoginWindow()
    {
        InitializeComponent();
    }

    private void OnLoginClick(object sender, RoutedEventArgs e)
    {
        var user = _authService.Authorize(LoginBox.Text, PasswordBox.Password);
        if (user is null)
        {
            MessageBox.Show(
                "Неверный логин или пароль либо учётная запись заблокирована.",
                "Авторизация",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
            return;
        }

        var main = new MainWindow(user) { Owner = null };
        main.Show();
        Close();
    }

    private void OnExitClick(object sender, RoutedEventArgs e)
    {
        Close();
    }
}