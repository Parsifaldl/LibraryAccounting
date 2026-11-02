using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryAccounting.Models;

[Table("OperationLog")]
public class OperationLog
{
    [Key]
    public int Id { get; set; }

    [Column("operation", TypeName = "varchar(100)")]
    public string Operation { get; set; } = string.Empty;

    [Column("description", TypeName = "varchar(500)")]
    public string? Description { get; set; }

    [Column("created_at", TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [Column("user_id")]
    public int UserId { get; set; }

    public User? User { get; set; }
}