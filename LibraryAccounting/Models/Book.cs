using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryAccounting.Models;

[Table("Book")]
public class Book
{
    [Key]
    public int Id { get; set; }

    [Column("title", TypeName = "varchar(255)")]
    public string Title { get; set; } = string.Empty;

    [Column("year")]
    public int Year { get; set; }

    [Column("pages")]
    public int? Pages { get; set; }

    [Column("publisher_id")]
    public int PublisherId { get; set; }

    [Column("genre_id")]
    public int GenreId { get; set; }

    public Publisher? Publisher { get; set; }
    public Genre? Genre { get; set; }
    public List<BookAuthor> BookAuthors { get; set; } = [];
    public List<Copy> Copies { get; set; } = [];

    public string AuthorsDisplay => string.Join(", ", BookAuthors.Select(ba => ba.Author?.ShortName));
}