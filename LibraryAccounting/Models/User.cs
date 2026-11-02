using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryAccounting.Models;

[Table("User")]
public class User
{
    [Key]
    public int Id { get; set; }

    [Column("login", TypeName = "varchar(50)")]
    public string Login { get; set; } = string.Empty;

    [Column("password", TypeName = "varchar(50)")]
    public string Password { get; set; } = string.Empty;

    [Column("last_name", TypeName = "varchar(100)")]
    public string LastName { get; set; } = string.Empty;

    [Column("first_name", TypeName = "varchar(100)")]
    public string FirstName { get; set; } = string.Empty;

    [Column("middle_name", TypeName = "varchar(100)")]
    public string? MiddleName { get; set; }

    [Column("is_blocked")]
    public bool IsBlocked { get; set; }

    [Column("role_id")]
    public int RoleId { get; set; }

    public Role? Role { get; set; }

    public string DisplayName
    {
        get
        {
            var initials = string.Empty;
            if (!string.IsNullOrEmpty(FirstName)) initials += $" {FirstName[0]}.";
            if (!string.IsNullOrEmpty(MiddleName)) initials += $" {MiddleName[0]}.";
            return $"{LastName},{initials}";
        }
    }
}