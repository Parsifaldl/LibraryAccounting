using LibraryAccounting.Models;
using LibraryAccounting.Services;
using LibraryAccounting.ViewModels;
using System.Windows;

namespace LibraryAccounting.Views;

public partial class MainWindow : Window
{
    private readonly User _currentUser;
    private readonly LoanService _loanService = new();

    private MainViewModel ViewModel => (MainViewModel)DataContext;

    public MainWindow(User currentUser)
    {
        _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
        InitializeComponent();
        DataContext = new MainViewModel(_currentUser);
    }

    private void OnIssueClick(object sender, RoutedEventArgs e)
    {
        var row = ViewModel.SelectedCatalogRow;
        if (row is null)
        {
            MessageBox.Show(
                "Выберите издание в каталоге.",
                "Выдача книги",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
            return;
        }

        var window = new IssueLoanWindow(_currentUser, row.BookId, row.Title) { Owner = this };
        if (window.ShowDialog() == true)
        {
            ViewModel.LoadCatalog();
            ViewModel.LoadLoans();
        }
    }

    private void OnReturnClick(object sender, RoutedEventArgs e)
    {
        var row = ViewModel.SelectedLoanRow;
        if (row is null)
        {
            MessageBox.Show(
                "Выберите выдачу в списке.",
                "Возврат книги",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
            return;
        }

        var answer = MessageBox.Show(
            $"Оформить возврат экземпляра №{row.InventoryNumber} " +
            $"от читателя {row.ReaderName}?",
            "Возврат книги",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);
        if (answer != MessageBoxResult.Yes)
        {
            return;
        }

        int overdueDays = _loanService.ReturnCopy(row.LoanId);
        if (overdueDays < 0)
        {
            MessageBox.Show(
                "Не удалось оформить возврат: выдача не найдена или книга уже возвращена.",
                "Возврат книги",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
            return;
        }

        string note = overdueDays > 0
            ? $" Зафиксирована просрочка {overdueDays} дн., создана запись о задолженности."
            : " Книга возвращена в срок.";
        MessageBox.Show(
            "Возврат оформлен." + note,
            "Возврат книги",
            MessageBoxButton.OK,
            MessageBoxImage.Information);

        ViewModel.LoadCatalog();
        ViewModel.LoadLoans();
    }

    private void OnExtendClick(object sender, RoutedEventArgs e)
    {
        var row = ViewModel.SelectedLoanRow;
        if (row is null)
        {
            MessageBox.Show(
                "Выберите выдачу в списке.",
                "Продление срока",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
            return;
        }

        string? error = _loanService.ExtendLoan(row.LoanId);
        if (error is not null)
        {
            MessageBox.Show(error, "Продление срока", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        MessageBox.Show(
            "Срок возврата продлён на 14 дней.",
            "Продление срока",
            MessageBoxButton.OK,
            MessageBoxImage.Information);

        ViewModel.LoadLoans();
    }
}