using System;
using System.Collections.Generic;
using System.Linq;
using LibraryAccounting.Models;
using LibraryAccounting.Services;
using Xunit;

namespace LibraryAccounting.Tests;

[Collection("LibraryTestDatabase")]
public class AuthServiceTests
{
    private readonly TestDatabaseFixture _fixture;

    public AuthServiceTests(TestDatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Theory]
    [InlineData("bibliotekar", "123")]
    [InlineData("student01", "12345")]
    [InlineData("", "")]
    [InlineData(" ", "12345")]
    public void Authorize_InvalidCredentials_ReturnsNull(string login, string password)
    {
        var service = new AuthService();

        User? user = service.Authorize(login, password);

        Assert.Null(user);
    }

    [Fact]
    public void Authorize_ValidCredentials_ReturnsLibrarian()
    {
        var service = new AuthService();

        User? user = service.Authorize("bibliotekar", "12345");

        Assert.NotNull(user);
        Assert.Equal("Смирнова", user!.LastName);
        Assert.Equal("Библиотекарь абонемента", user.Role!.Name);
        Assert.False(user.IsBlocked);
    }

    [Fact]
    public void Authorize_TrimsLoginWhitespace_ReturnsUser()
    {
        var service = new AuthService();

        User? user = service.Authorize("  bibliotekar  ", "12345");

        Assert.NotNull(user);
        Assert.Equal("bibliotekar", user!.Login);
    }

    [Fact]
    public void Authorize_BlockedAccount_ReturnsNull()
    {
        var service = new AuthService();

        using (var context = _fixture.CreateContext())
        {
            User account = context.Users.First(u => u.Login == "komplekt");
            account.IsBlocked = true;
            context.SaveChanges();
        }

        try
        {
            Assert.Null(service.Authorize("komplekt", "12345"));
        }
        finally
        {
            using var context = _fixture.CreateContext();
            User account = context.Users.First(u => u.Login == "komplekt");
            account.IsBlocked = false;
            context.SaveChanges();
        }
    }
}