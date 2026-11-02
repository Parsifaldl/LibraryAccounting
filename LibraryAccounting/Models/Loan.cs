using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryAccounting.Models;

[Table("Loan")]
public class Loan
{
    [Key]
    public int Id { get; set; }

    [Column("issue_date", TypeName = "date")]
    public DateTime IssueDate { get; set; }

    [Column("due_date", TypeName = "date")]
    public DateTime DueDate { get; set; }

    [Column("return_date", TypeName = "date")]
    public DateTime? ReturnDate { get; set; }

    [Column("is_overdue")]
    public bool? IsOverdue { get; set; }

    [Column("reader_id")]
    public int ReaderId { get; set; }

    [Column("copy_id")]
    public int CopyId { get; set; }

    [Column("form_id")]
    public int FormId { get; set; }

    [Column("user_id")]
    public int UserId { get; set; }

    public Reader? Reader { get; set; }
    public Copy? Copy { get; set; }
    public LoanForm? Form { get; set; }
    public User? User { get; set; }
}