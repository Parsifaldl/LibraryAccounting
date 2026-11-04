using LibraryAccounting.Models;
using LibraryAccounting.Services;
using System.Collections.ObjectModel;

namespace LibraryAccounting.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly User _currentUser;
    private readonly CatalogService _catalogService = new();
    private readonly LoanService _loanService = new();

    public MainViewModel(User currentUser)
    {
        _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));

        SearchCommand = new RelayCommand(_ => LoadCatalog());
        RefreshLoansCommand = new RelayCommand(_ => LoadLoans());

        Genres = _catalogService.GetGenres();
        LoadCatalog();
        LoadLoans();
    }

    public string CurrentUserInfo =>
        $"{_currentUser.DisplayName} - {_currentUser.Role?.Name}";

    public ObservableCollection<CatalogRow> CatalogRows { get; } = [];
    public ObservableCollection<LoanRow> LoanRows { get; } = [];
    public List<Genre> Genres { get; }

    private string _searchText = string.Empty;
    public string SearchText
    {
        get => _searchText;
        set { _searchText = value; OnPropertyChanged(); }
    }

    private Genre? _selectedGenre;
    public Genre? SelectedGenre
    {
        get => _selectedGenre;
        set { _selectedGenre = value; OnPropertyChanged(); }
    }

    private CatalogRow? _selectedCatalogRow;
    public CatalogRow? SelectedCatalogRow
    {
        get => _selectedCatalogRow;
        set { _selectedCatalogRow = value; OnPropertyChanged(); }
    }

    private LoanRow? _selectedLoanRow;
    public LoanRow? SelectedLoanRow
    {
        get => _selectedLoanRow;
        set { _selectedLoanRow = value; OnPropertyChanged(); }
    }

    public RelayCommand SearchCommand { get; }
    public RelayCommand RefreshLoansCommand { get; }

    public void LoadCatalog()
    {
        CatalogRows.Clear();
        foreach (var book in _catalogService.SearchBooks(SearchText, SelectedGenre?.Id))
        {
            CatalogRows.Add(new CatalogRow(book));
        }
    }

    public void LoadLoans()
    {
        LoanRows.Clear();
        foreach (var loan in _loanService.GetActiveLoans())
        {
            LoanRows.Add(new LoanRow(loan));
        }
    }
}