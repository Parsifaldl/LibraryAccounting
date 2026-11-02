using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryAccounting.Models;

[Table("LoanForm")]
public class LoanForm
{
    [Key]
    public int Id { get; set; }

    [Column("name", TypeName = "varchar(50)")]
    public string Name { get; set; } = string.Empty;
}