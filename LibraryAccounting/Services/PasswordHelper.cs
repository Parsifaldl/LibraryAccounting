using System.Security.Cryptography;
using System.Text;

namespace LibraryAccounting.Services;

public static class PasswordHelper
{
    public static string ComputeHash(string password)
    {
        byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }
}