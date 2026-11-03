using LibraryAccounting.Constants;
using LibraryAccounting.Data;
using LibraryAccounting.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryAccounting.Services;

public class LoanService
{
    public List<Loan> GetActiveLoans()
    {
        using var context = new LibraryDbContext();
        return context.Loans
            .Include(l => l.Reader)
            .Include(l => l.Copy!).ThenInclude(c => c.Book)
            .Include(l => l.Form)
            .Where(l => l.ReturnDate == null)
            .OrderBy(l => l.DueDate)
            .ToList();
    }

    public string? IssueCopy(int? readerId, string? inventoryNumber, int? formId, int userId)
    {
        var validation = new ValidationResult();
        if (readerId is null)
        {
            validation.AddError("Не выбран читатель.");
        }
        if (string.IsNullOrWhiteSpace(inventoryNumber))
        {
            validation.AddError("Не указан инвентарный номер экземпляра.");
        }
        if (formId is null)
        {
            validation.AddError("Не выбрана форма выдачи.");
        }
        if (!validation.IsValid)
        {
            return validation.ErrorMessage;
        }

        using var context = new LibraryDbContext();

        var reader = context.Readers.FirstOrDefault(r => r.Id == readerId);
        var copy = context.Copies.Include(c => c.Status)
            .FirstOrDefault(c => c.InventoryNumber == inventoryNumber!.Trim());
        var form = context.LoanForms.FirstOrDefault(f => f.Id == formId);

        var issueValidation = ValidateIssue(context, reader, copy, form);
        if (!issueValidation.IsValid)
        {
            return issueValidation.ErrorMessage;
        }

        DateTime dueDate = form!.Name == LibraryConstants.LoanFormHome
            ? DateTime.Today.AddDays(LibraryConstants.HomeLoanDays)
            : DateTime.Today;

        var loan = new Loan
        {
            IssueDate = DateTime.Today,
            DueDate = dueDate,
            ReaderId = reader!.Id,
            CopyId = copy!.Id,
            FormId = form.Id,
            UserId = userId,
        };

        copy.Status = context.CopyStatuses
            .First(s => s.Name == LibraryConstants.CopyStatusIssued);

        context.Loans.Add(loan);
        context.SaveChanges();

        WriteLog(context, userId, "Выдача книги",
            $"Экземпляр №{copy.InventoryNumber} выдан читателю {reader.FullName} (форма: {form.Name}).");
        return null;
    }

    private static ValidationResult ValidateIssue(
        LibraryDbContext context, Reader? reader, Copy? copy, LoanForm? form)
    {
        var result = new ValidationResult();

        if (reader is null)
        {
            result.AddError("Читатель не найден в картотеке.");
        }
        if (copy is null)
        {
            result.AddError("Экземпляр с указанным инвентарным номером не найден в фонде.");
        }
        if (form is null)
        {
            result.AddError("Форма выдачи не определена.");
        }
        if (!result.IsValid)
        {
            return result;
        }

        if (reader!.Status != LibraryConstants.ReaderStatusActive)
        {
            result.AddError(
                $"Читатель {reader.FullName} имеет статус «{reader.Status}»; выдача невозможна.");
        }
        if (copy!.Status!.Name != LibraryConstants.CopyStatusAvailable)
        {
            result.AddError(
                $"Экземпляр №{copy.InventoryNumber} недоступен " +
                $"(текущий статус – «{copy.Status.Name}»); выдача невозможна.");
        }
        if (HasOpenDebt(context, reader.Id))
        {
            result.AddError(
                $"У читателя {reader.FullName} есть непогашенная задолженность; " +
                "выдача заблокирована до её закрытия.");
        }

        return result;
    }

    private static bool HasOpenDebt(LibraryDbContext context, int readerId)
    {
        return context.Debts
            .Any(d => d.ClosedAt == null && d.Loan!.ReaderId == readerId);
    }

    public int ReturnCopy(int loanId)
    {
        if (loanId <= 0)
        {
            return -1;
        }

        using var context = new LibraryDbContext();
        var loan = context.Loans
            .Include(l => l.Copy).ThenInclude(c => c.Status)
            .FirstOrDefault(l => l.Id == loanId);

        if (loan is null || loan.ReturnDate is not null)
        {
            return -1;
        }

        int overdueDays = (DateTime.Today - loan.DueDate).Days;
        if (overdueDays < 0)
        {
            overdueDays = 0;
        }

        loan.ReturnDate = DateTime.Today;
        loan.IsOverdue = overdueDays > 0;

        if (loan.Copy is not null)
        {
            loan.Copy.Status = context.CopyStatuses
                .First(s => s.Name == LibraryConstants.CopyStatusAvailable);
        }

        if (overdueDays > 0)
        {
            context.Debts.Add(new Debt
            {
                LoanId = loan.Id,
                DaysOverdue = overdueDays,
                CreatedAt = DateTime.Today,
                ClosedAt = DateTime.Today,
            });
        }

        context.SaveChanges();
        WriteLog(context, loan.UserId, "Возврат книги",
            $"Экземпляр №{loan.Copy?.InventoryNumber} возвращён" +
            (overdueDays > 0 ? $", просрочка {overdueDays} дн." : ", в срок") + ".");
        return overdueDays;
    }

    public string? ExtendLoan(int loanId)
    {
        if (loanId <= 0)
        {
            return "Не выбрана выдача для продления.";
        }

        using var context = new LibraryDbContext();
        var loan = context.Loans.FirstOrDefault(l => l.Id == loanId);

        if (loan is null)
        {
            return "Выдача не найдена.";
        }
        if (loan.ReturnDate is not null)
        {
            return "Книга уже возвращена; продление невозможно.";
        }

        DateTime oldDueDate = loan.DueDate;
        DateTime newDueDate = oldDueDate.AddDays(LibraryConstants.LoanExtensionDays);

        context.LoanExtensions.Add(new LoanExtension
        {
            LoanId = loan.Id,
            OldDueDate = oldDueDate,
            NewDueDate = newDueDate,
            ExtensionDate = DateTime.Today,
        });
        loan.DueDate = newDueDate;

        context.SaveChanges();
        WriteLog(context, loan.UserId, "Продление срока",
            $"Срок возврата по выдаче №{loan.Id} продлён " +
            $"с {oldDueDate:dd.MM.yyyy} до {newDueDate:dd.MM.yyyy}.");
        return null;
    }

    private static void WriteLog(
        LibraryDbContext context, int userId, string operation, string description)
    {
        context.OperationLogs.Add(new OperationLog
        {
            Operation = operation,
            Description = description,
            CreatedAt = DateTime.Now,
            UserId = userId,
        });
        context.SaveChanges();
    }
}