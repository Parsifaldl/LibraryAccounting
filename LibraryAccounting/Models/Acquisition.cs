using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryAccounting.Models;

[Table("Acquisition")]
public class Acquisition
{
    [Key]
    public int Id { get; set; }

    [Column("date", TypeName = "date")]
    public DateTime Date { get; set; }

    [Column("quantity")]
    public int Quantity { get; set; }

    [Column("price", TypeName = "decimal(10,2)")]
    public decimal? Price { get; set; }

    [Column("source", TypeName = "varchar(50)")]
    public string Source { get; set; } = string.Empty;

    [Column("book_id")]
    public int BookId { get; set; }

    public Book? Book { get; set; }
}