using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryAccounting.Models;

[Table("WriteOff")]
public class WriteOff
{
    [Key]
    public int Id { get; set; }

    [Column("date", TypeName = "date")]
    public DateTime Date { get; set; }

    [Column("reason", TypeName = "varchar(100)")]
    public string Reason { get; set; } = string.Empty;

    [Column("copy_id")]
    public int CopyId { get; set; }

    public Copy? Copy { get; set; }
}