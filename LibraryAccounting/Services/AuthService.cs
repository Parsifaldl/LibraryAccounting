using LibraryAccounting.Data;
using LibraryAccounting.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryAccounting.Services;

public class AuthService
{
    public User? Authorize(string login, string password)
    {
        if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
        {
            return null;
        }

        using var context = new LibraryDbContext();
        string passwordHash = PasswordHelper.ComputeHash(password);

        return context.Users
            .Include(u => u.Role)
            .FirstOrDefault(u => u.Login == login.Trim()
                                 && u.Password == passwordHash
                                 && !u.IsBlocked);
    }
}