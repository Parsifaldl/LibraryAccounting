using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryAccounting.Models;

[Table("Reader")]
public class Reader
{
    [Key]
    public int Id { get; set; }

    [Column("last_name", TypeName = "varchar(100)")]
    public string LastName { get; set; } = string.Empty;

    [Column("first_name", TypeName = "varchar(100)")]
    public string FirstName { get; set; } = string.Empty;

    [Column("middle_name", TypeName = "varchar(100)")]
    public string? MiddleName { get; set; }

    [Column("birth_date", TypeName = "date")]
    public DateTime? BirthDate { get; set; }

    [Column("category", TypeName = "varchar(50)")]
    public string Category { get; set; } = string.Empty;

    [Column("ticket_number", TypeName = "varchar(20)")]
    public string TicketNumber { get; set; } = string.Empty;

    [Column("contact", TypeName = "varchar(150)")]
    public string? Contact { get; set; }

    [Column("status", TypeName = "varchar(20)")]
    public string Status { get; set; } = string.Empty;

    public List<Loan> Loans { get; set; } = [];

    public string FullName => $"{LastName} {FirstName} {MiddleName}".Trim();

    public string DisplayName => $"{FullName}, билет №{TicketNumber}, {Status}";
}