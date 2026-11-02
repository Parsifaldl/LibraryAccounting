using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryAccounting.Models;

[Table("Genre")]
public class Genre
{
    [Key]
    public int Id { get; set; }

    [Column("name", TypeName = "varchar(100)")]
    public string Name { get; set; } = string.Empty;
}