using LibraryAccounting.Models;
using LibraryAccounting.ViewModels;
using System.Windows;

namespace LibraryAccounting.Views;

public partial class IssueLoanWindow : Window
{
    private readonly IssueLoanViewModel _viewModel;

    public IssueLoanWindow(User currentUser, int bookId, string bookTitle)
    {
        InitializeComponent();
        _viewModel = new IssueLoanViewModel(currentUser, bookId, bookTitle);
        DataContext = _viewModel;
    }

    private void OnIssueClick(object sender, RoutedEventArgs e)
    {
        string? error = _viewModel.Issue();
        if (error is null)
        {
            MessageBox.Show(
                "Выдача оформлена успешно.",
                "Выдача книги",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
            DialogResult = true;
            return;
        }

        MessageBox.Show(error, "Ошибка выдачи", MessageBoxButton.OK, MessageBoxImage.Warning);
    }
}