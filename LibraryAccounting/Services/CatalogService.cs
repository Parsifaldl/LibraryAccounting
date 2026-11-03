using LibraryAccounting.Constants;
using LibraryAccounting.Data;
using LibraryAccounting.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryAccounting.Services;

public class CatalogService
{
    public List<Book> SearchBooks(string searchText, int? genreId)
    {
        using var context = new LibraryDbContext();

        IQueryable<Book> query = context.Books
            .Include(b => b.Genre)
            .Include(b => b.Publisher)
            .Include(b => b.BookAuthors).ThenInclude(ba => ba.Author)
            .Include(b => b.Copies).ThenInclude(c => c.Status);

        if (!string.IsNullOrWhiteSpace(searchText))
        {
            string clean = searchText.Trim();
            query = query.Where(b =>
                b.Title.Contains(clean)
                || b.BookAuthors.Any(ba => (ba.Author!.LastName + " " + ba.Author.FirstName).Contains(clean))
                || b.Copies.Any(c => c.InventoryNumber.Contains(clean)));
        }

        if (genreId.HasValue)
        {
            query = query.Where(b => b.GenreId == genreId.Value);
        }

        return query.OrderBy(b => b.Title).ToList();
    }

    public List<Genre> GetGenres()
    {
        using var context = new LibraryDbContext();
        return context.Genres.OrderBy(g => g.Name).ToList();
    }

    public List<Reader> SearchReaders(string searchText)
    {
        using var context = new LibraryDbContext();

        IQueryable<Reader> query = context.Readers;

        if (!string.IsNullOrWhiteSpace(searchText))
        {
            string clean = searchText.Trim();
            query = query.Where(r =>
                r.LastName.Contains(clean)
                || r.FirstName.Contains(clean)
                || r.TicketNumber.Contains(clean));
        }

        return query.OrderBy(r => r.LastName).ThenBy(r => r.FirstName).ToList();
    }

    public List<Copy> GetAvailableCopies(int bookId)
    {
        using var context = new LibraryDbContext();
        return context.Copies
            .Include(c => c.Status)
            .Where(c => c.BookId == bookId
                        && c.Status!.Name == LibraryConstants.CopyStatusAvailable)
            .OrderBy(c => c.InventoryNumber)
            .ToList();
    }

    public List<LoanForm> GetLoanForms()
    {
        using var context = new LibraryDbContext();
        return context.LoanForms.OrderBy(f => f.Id).ToList();
    }
}