using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryAccounting.Models;

[Table("Debt")]
public class Debt
{
    [Key]
    public int Id { get; set; }

    [Column("days_overdue")]
    public int DaysOverdue { get; set; }

    [Column("created_at", TypeName = "date")]
    public DateTime CreatedAt { get; set; }

    [Column("closed_at", TypeName = "date")]
    public DateTime? ClosedAt { get; set; }

    [Column("loan_id")]
    public int LoanId { get; set; }

    public Loan? Loan { get; set; }
}