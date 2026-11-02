using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryAccounting.Models;

[Table("LoanExtension")]
public class LoanExtension
{
    [Key]
    public int Id { get; set; }

    [Column("old_due_date", TypeName = "date")]
    public DateTime OldDueDate { get; set; }

    [Column("new_due_date", TypeName = "date")]
    public DateTime NewDueDate { get; set; }

    [Column("extension_date", TypeName = "date")]
    public DateTime ExtensionDate { get; set; }

    [Column("loan_id")]
    public int LoanId { get; set; }

    public Loan? Loan { get; set; }
}