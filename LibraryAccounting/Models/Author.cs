using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryAccounting.Models;

[Table("Author")]
public class Author
{
    [Key]
    public int Id { get; set; }

    [Column("last_name", TypeName = "varchar(100)")]
    public string LastName { get; set; } = string.Empty;

    [Column("first_name", TypeName = "varchar(100)")]
    public string FirstName { get; set; } = string.Empty;

    [Column("middle_name", TypeName = "varchar(100)")]
    public string? MiddleName { get; set; }

    [Column("country", TypeName = "varchar(100)")]
    public string? Country { get; set; }

    public string ShortName
    {
        get
        {
            var initials = string.Empty;
            if (!string.IsNullOrEmpty(FirstName)) initials += $" {FirstName[0]}.";
            if (!string.IsNullOrEmpty(MiddleName)) initials += $"{MiddleName[0]}.";
            return $"{LastName}{initials}";
        }
    }
}