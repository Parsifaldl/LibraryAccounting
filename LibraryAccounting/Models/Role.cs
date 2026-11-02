using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryAccounting.Models;

[Table("Role")]
public class Role
{
    [Key]
    public int Id { get; set; }

    [Column("name", TypeName = "varchar(50)")]
    public string Name { get; set; } = string.Empty;

    [Column("description", TypeName = "varchar(255)")]
    public string? Description { get; set; }

    public List<User> Users { get; set; } = [];
}