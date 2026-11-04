using System;
using System.Collections.Generic;
using System.Linq;
using LibraryAccounting.Constants;
using LibraryAccounting.Models;
using LibraryAccounting.Services;
using Xunit;

namespace LibraryAccounting.Tests;

[Collection("LibraryTestDatabase")]
public class CatalogServiceTests
{
    private readonly TestDatabaseFixture _fixture;

    public CatalogServiceTests(TestDatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void SearchBooks_ByTitleFragment_ReturnsMatchingBooks()
    {
        var service = new CatalogService();

        List<Book> results = service.SearchBooks("мир", null);

        Assert.Single(results);
        Assert.Equal("Война и мир", results[0].Title);
    }

    [Fact]
    public void SearchBooks_ByAuthorAndInventoryNumber()
    {
        var service = new CatalogService();

        List<Book> byAuthor = service.SearchBooks("Достоевский", null);
        List<Book> byInventory = service.SearchBooks("ИН-000107", null);

        Book book = byAuthor.Single();
        Assert.Equal("Преступление и наказание", book.Title);
        Assert.Equal("Преступление и наказание", byInventory.Single().Title);
    }

    [Fact]
    public void SearchBooks_ByGenre_ReturnsOnlyGenreBooks()
    {
        using var context = _fixture.CreateContext();
        int classicId = context.Genres.First(g => g.Name == "Русская классика").Id;
        var service = new CatalogService();

        List<Book> results = service.SearchBooks(null, classicId);

        Assert.Equal(6, results.Count);
        Assert.All(results, b => Assert.Equal("Русская классика", b.Genre!.Name));
    }

    [Fact]
    public void SearchBooks_NoMatches_ReturnsEmptyList()
    {
        var service = new CatalogService();

        List<Book> results = service.SearchBooks("Гарри Поттер", null);

        Assert.Empty(results);
    }

    [Fact]
    public void SearchReaders_ByLastNameAndTicket_ReturnsMatchingReader()
    {
        var service = new CatalogService();

        List<Reader> byName = service.SearchReaders("Петрова");
        List<Reader> byTicket = service.SearchReaders("БН-2024015");

        Reader petrova = byName.Single();
        Assert.Equal("БН-2021004", petrova.TicketNumber);
        Assert.Equal("Сидоров Кирилл Константинович", byTicket.Single().FullName);
    }

    [Fact]
    public void GetAvailableCopies_ReturnsOnlyAvailableCopies()
    {
        using var context = _fixture.CreateContext();
        int bookId = context.Books.First(b => b.Title == "Преступление и наказание").Id;
        var service = new CatalogService();

        List<Copy> copies = service.GetAvailableCopies(bookId);

        Assert.Equal(2, copies.Count);
        Assert.All(copies, c => Assert.Equal(LibraryConstants.CopyStatusAvailable, c.Status!.Name));
        Assert.Contains(copies, c => c.InventoryNumber == "ИН-000107");
        Assert.Contains(copies, c => c.InventoryNumber == "ИН-000108");
    }
}