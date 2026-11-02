using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryAccounting.Models;

[Table("Publisher")]
public class Publisher
{
    [Key]
    public int Id { get; set; }

    [Column("name", TypeName = "varchar(150)")]
    public string Name { get; set; } = string.Empty;

    [Column("city", TypeName = "varchar(100)")]
    public string? City { get; set; }
}