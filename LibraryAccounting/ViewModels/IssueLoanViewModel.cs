using LibraryAccounting.Constants;
using LibraryAccounting.Models;
using LibraryAccounting.Services;
using System.Collections.ObjectModel;

namespace LibraryAccounting.ViewModels;

public class IssueLoanViewModel : ViewModelBase
{
    private readonly LoanService _loanService = new();
    private readonly CatalogService _catalogService = new();
    private readonly int _currentUserId;
    private readonly int _bookId;

    public IssueLoanViewModel(User currentUser, int bookId, string bookTitle)
    {
        _currentUserId = currentUser.Id;
        _bookId = bookId;
        BookTitle = bookTitle;

        ReaderSearchCommand = new RelayCommand(_ => LoadReaders());

        LoadReaders();
        LoadCopies();
    }

    public string BookTitle { get; }
    public string WindowTitle => $"Выдача книги: {BookTitle}";

    public ObservableCollection<Reader> Readers { get; } = [];
    public ObservableCollection<Copy> Copies { get; } = [];
    public List<LoanForm> Forms { get; private set; } = [];

    private string _readerSearch = string.Empty;
    public string ReaderSearch
    {
        get => _readerSearch;
        set { _readerSearch = value; OnPropertyChanged(); }
    }

    private Reader? _selectedReader;
    public Reader? SelectedReader
    {
        get => _selectedReader;
        set { _selectedReader = value; OnPropertyChanged(); }
    }

    private Copy? _selectedCopy;
    public Copy? SelectedCopy
    {
        get => _selectedCopy;
        set { _selectedCopy = value; OnPropertyChanged(); }
    }

    private LoanForm? _selectedForm;
    public LoanForm? SelectedForm
    {
        get => _selectedForm;
        set
        {
            _selectedForm = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(DueDateInfo));
        }
    }

    public string DueDateInfo => SelectedForm switch
    {
        null => "Форма выдачи не выбрана.",
        _ when SelectedForm.Name == LibraryConstants.LoanFormHome =>
            $"Срок возврата: {DateTime.Today.AddDays(LibraryConstants.HomeLoanDays):dd.MM.yyyy} " +
            $"({LibraryConstants.HomeLoanDays} дней)",
        _ => "Возврат до конца рабочего дня (читальный зал)"
    };

    public RelayCommand ReaderSearchCommand { get; }

    private void LoadReaders()
    {
        Readers.Clear();
        foreach (var reader in _catalogService.SearchReaders(ReaderSearch))
        {
            Readers.Add(reader);
        }
    }

    private void LoadCopies()
    {
        Copies.Clear();
        foreach (var copy in _catalogService.GetAvailableCopies(_bookId))
        {
            Copies.Add(copy);
        }

        Forms = _catalogService.GetLoanForms();
        OnPropertyChanged(nameof(Forms));

        SelectedCopy = Copies.FirstOrDefault();
        SelectedForm = Forms.FirstOrDefault(f => f.Name == LibraryConstants.LoanFormHome);
    }

    public string? Issue()
    {
        return _loanService.IssueCopy(
            SelectedReader?.Id,
            SelectedCopy?.InventoryNumber,
            SelectedForm?.Id,
            _currentUserId);
    }
}