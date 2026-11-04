using LibraryAccounting.Constants;
using LibraryAccounting.Models;

namespace LibraryAccounting.ViewModels;

public class CatalogRow
{
    public CatalogRow(Book book)
    {
        BookId = book.Id;
        Title = book.Title;
        Authors = book.AuthorsDisplay;
        Genre = book.Genre?.Name ?? string.Empty;
        Publisher = book.Publisher?.Name ?? string.Empty;
        Year = book.Year;
        TotalCopies = book.Copies.Count;
        AvailableCopies = book.Copies.Count(
            c => c.Status != null && c.Status.Name == LibraryConstants.CopyStatusAvailable);
    }

    public int BookId { get; }
    public string Title { get; }
    public string Authors { get; }
    public string Genre { get; }
    public string Publisher { get; }
    public int Year { get; }
    public int TotalCopies { get; }
    public int AvailableCopies { get; }

    public string Availability => $"{AvailableCopies} из {TotalCopies}";
}