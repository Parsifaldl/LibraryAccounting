using LibraryAccounting.Constants;
using LibraryAccounting.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryAccounting.Data;

public class LibraryDbContext : DbContext
{
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Genre> Genres => Set<Genre>();
    public DbSet<Publisher> Publishers => Set<Publisher>();
    public DbSet<Author> Authors => Set<Author>();
    public DbSet<Book> Books => Set<Book>();
    public DbSet<BookAuthor> BookAuthors => Set<BookAuthor>();
    public DbSet<CopyStatus> CopyStatuses => Set<CopyStatus>();
    public DbSet<Copy> Copies => Set<Copy>();
    public DbSet<Reader> Readers => Set<Reader>();
    public DbSet<LoanForm> LoanForms => Set<LoanForm>();
    public DbSet<Acquisition> Acquisitions => Set<Acquisition>();
    public DbSet<WriteOff> WriteOffs => Set<WriteOff>();
    public DbSet<Loan> Loans => Set<Loan>();
    public DbSet<LoanExtension> LoanExtensions => Set<LoanExtension>();
    public DbSet<Debt> Debts => Set<Debt>();
    public DbSet<OperationLog> OperationLogs => Set<OperationLog>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(LibraryConstants.ConnectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BookAuthor>()
            .HasKey(ba => new { ba.BookId, ba.AuthorId });

        modelBuilder.Entity<Loan>()
            .HasOne(l => l.Reader)
            .WithMany(r => r.Loans)
            .HasForeignKey(l => l.ReaderId);
    }
}