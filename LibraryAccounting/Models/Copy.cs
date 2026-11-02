using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryAccounting.Models;

[Table("Copy")]
public class Copy
{
    [Key]
    public int Id { get; set; }

    [Column("inventory_number", TypeName = "varchar(20)")]
    public string InventoryNumber { get; set; } = string.Empty;

    [Column("price", TypeName = "decimal(10,2)")]
    public decimal? Price { get; set; }

    [Column("arrival_date", TypeName = "date")]
    public DateTime ArrivalDate { get; set; }

    [Column("location", TypeName = "varchar(50)")]
    public string Location { get; set; } = string.Empty;

    [Column("book_id")]
    public int BookId { get; set; }

    [Column("status_id")]
    public int StatusId { get; set; }

    public Book? Book { get; set; }
    public CopyStatus? Status { get; set; }

    public string Display => $"{InventoryNumber} ({Location})";
}