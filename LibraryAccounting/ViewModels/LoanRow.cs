using LibraryAccounting.Models;

namespace LibraryAccounting.ViewModels;

public class LoanRow
{
    public LoanRow(Loan loan)
    {
        LoanId = loan.Id;
        InventoryNumber = loan.Copy?.InventoryNumber ?? string.Empty;
        Title = loan.Copy?.Book?.Title ?? string.Empty;
        ReaderName = loan.Reader?.FullName ?? string.Empty;
        IssueDate = loan.IssueDate.ToString("dd.MM.yyyy");
        DueDate = loan.DueDate.ToString("dd.MM.yyyy");
        FormName = loan.Form?.Name ?? string.Empty;

        int overdueDays = (DateTime.Today - loan.DueDate).Days;
        StatusText = overdueDays > 0 ? $"Просрочено {overdueDays} дн." : "В срок";
        IsOverdue = overdueDays > 0;
    }

    public int LoanId { get; }
    public string InventoryNumber { get; }
    public string Title { get; }
    public string ReaderName { get; }
    public string IssueDate { get; }
    public string DueDate { get; }
    public string FormName { get; }
    public string StatusText { get; }
    public bool IsOverdue { get; }
}