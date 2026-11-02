using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryAccounting.Models;

[Table("CopyStatus")]
public class CopyStatus
{
    [Key]
    public int Id { get; set; }

    [Column("name", TypeName = "varchar(50)")]
    public string Name { get; set; } = string.Empty;
}