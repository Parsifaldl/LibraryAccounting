using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryAccounting.Models;

[Table("BookAuthor")]
public class BookAuthor
{
    [Column("book_id")]
    public int BookId { get; set; }

    [Column("author_id")]
    public int AuthorId { get; set; }

    public Book? Book { get; set; }
    public Author? Author { get; set; }
}