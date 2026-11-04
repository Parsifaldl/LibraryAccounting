using LibraryAccounting.Constants;
using LibraryAccounting.Models;
using LibraryAccounting.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace LibraryAccounting.Tests;

[Collection("LibraryTestDatabase")]
public class LoanServiceTests
{
    private readonly TestDatabaseFixture _fixture;

    public LoanServiceTests(TestDatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    private (int ReaderId, int UserId, int HomeFormId, int RoomFormId) GetReferenceIds()
    {
        using var context = _fixture.CreateContext();
        return (
            context.Readers.First(r => r.TicketNumber == "БН-2021004").Id,
            context.Users.First(u => u.Login == "bibliotekar").Id,
            context.LoanForms.First(f => f.Name == LibraryConstants.LoanFormHome).Id,
            context.LoanForms.First(f => f.Name == LibraryConstants.LoanFormReadingRoom).Id
        );
    }

    [Fact]
    public void IssueCopy_HomeForm_SetsDueDateIn14Days()
    {
        var ids = GetReferenceIds();
        var service = new LoanService();

        string? homeError = service.IssueCopy(ids.ReaderId, "ИН-000107", ids.HomeFormId, ids.UserId);
        string? roomError = service.IssueCopy(ids.ReaderId, "ИН-000112", ids.RoomFormId, ids.UserId);

        Assert.Null(homeError);
        Assert.Null(roomError);

        using var context = _fixture.CreateContext();
        Loan homeLoan = context.Loans.Single(l => l.Copy!.InventoryNumber == "ИН-000107");
        Loan roomLoan = context.Loans.Single(l => l.Copy!.InventoryNumber == "ИН-000112");
        Assert.Equal(DateTime.Today.AddDays(LibraryConstants.HomeLoanDays), homeLoan.DueDate);
        Assert.Equal(DateTime.Today, roomLoan.DueDate);
    }

    [Fact]
    public void IssueCopy_ValidData_CreatesLoanAndMarksCopyIssued()
    {
        var ids = GetReferenceIds();
        var service = new LoanService();

        string? error = service.IssueCopy(ids.ReaderId, "ИН-000111", ids.HomeFormId, ids.UserId);

        Assert.Null(error);

        using var context = _fixture.CreateContext();
        Loan loan = context.Loans.Single(l => l.Copy!.InventoryNumber == "ИН-000111");
        Assert.Null(loan.ReturnDate);
        Assert.Equal(DateTime.Today.AddDays(LibraryConstants.HomeLoanDays), loan.DueDate);

        Copy copy = context.Copies.Include(c => c.Status).First(c => c.InventoryNumber == "ИН-000111");
        Assert.Equal(LibraryConstants.CopyStatusIssued, copy.Status!.Name);

        Assert.Contains(
            context.OperationLogs.Where(l => l.Operation == "Выдача книги").ToList(),
            l => l.Description!.Contains("ИН-000111"));
    }

    [Theory]
    [InlineData("ИН-000103", "Выдан")]
    [InlineData("ИН-000113", "Списан")]
    public void IssueCopy_UnavailableCopy_ReturnsError(string inventoryNumber, string expectedStatus)
    {
        var ids = GetReferenceIds();
        var service = new LoanService();

        string? error = service.IssueCopy(ids.ReaderId, inventoryNumber, ids.HomeFormId, ids.UserId);

        Assert.NotNull(error);
        Assert.Contains("недоступен", error!);

        using var context = _fixture.CreateContext();
        Copy copy = context.Copies.Include(c => c.Status)
            .First(c => c.InventoryNumber == inventoryNumber);
        Assert.Equal(expectedStatus, copy.Status!.Name);
        Assert.DoesNotContain(
            context.Loans.ToList(),
            l => l.CopyId == copy.Id && l.IssueDate == DateTime.Today);
    }

    [Fact]
    public void IssueCopy_ReaderWithOpenDebt_ReturnsError()
    {
        using var context = _fixture.CreateContext();
        int sidorovId = context.Readers.First(r => r.TicketNumber == "БН-2024015").Id;
        var ids = GetReferenceIds();
        var service = new LoanService();

        string? error = service.IssueCopy(sidorovId, "ИН-000104", ids.HomeFormId, ids.UserId);

        Assert.NotNull(error);
        Assert.Contains("задолженность", error!);
        Assert.DoesNotContain(
            context.Loans.ToList(),
            l => l.Copy!.InventoryNumber == "ИН-000104");
    }

    [Fact]
    public void IssueCopy_BlockedReader_ReturnsError()
    {
        int kuznetsovaId;
        using (var context = _fixture.CreateContext())
        {
            Reader kuznetsova = context.Readers.First(r => r.TicketNumber == "БН-2022021");
            kuznetsova.Status = LibraryConstants.ReaderStatusBlocked;
            context.SaveChanges();
            kuznetsovaId = kuznetsova.Id;
        }

        try
        {
            var ids = GetReferenceIds();
            var service = new LoanService();

            string? error = service.IssueCopy(
                kuznetsovaId, "ИН-000105", ids.HomeFormId, ids.UserId);

            Assert.NotNull(error);
            Assert.Contains(LibraryConstants.ReaderStatusBlocked, error!);
        }
        finally
        {
            using var context = _fixture.CreateContext();
            Reader kuznetsova = context.Readers.First(r => r.TicketNumber == "БН-2022021");
            kuznetsova.Status = LibraryConstants.ReaderStatusActive;
            context.SaveChanges();
        }
    }

    [Fact]
    public void IssueCopy_MissingFields_ReturnsValidationErrors()
    {
        var ids = GetReferenceIds();
        var service = new LoanService();

        string? error = service.IssueCopy(null, null, null, ids.UserId);

        Assert.NotNull(error);
        Assert.Contains("Не выбран читатель", error!);
        Assert.Contains("Не указан инвентарный номер", error);
        Assert.Contains("Не выбрана форма выдачи", error);
    }

    [Fact]
    public void ReturnCopy_InTime_ReturnsZeroAndNoDebt()
    {
        var ids = GetReferenceIds();
        int ivanovId;
        using (var context = _fixture.CreateContext())
        {
            ivanovId = context.Readers.First(r => r.TicketNumber == "БН-2023001").Id;
        }

        var service = new LoanService();
        Assert.Null(service.IssueCopy(ivanovId, "ИН-000109", ids.HomeFormId, ids.UserId));

        int loanId;
        using (var context = _fixture.CreateContext())
        {
            loanId = context.Loans.Single(l => l.Copy!.InventoryNumber == "ИН-000109").Id;
        }

        int overdueDays = service.ReturnCopy(loanId);

        Assert.Equal(0, overdueDays);
        using var check = _fixture.CreateContext();
        Loan loan = check.Loans.Include(l => l.Copy).ThenInclude(c => c.Status).First(l => l.Id == loanId);
        Assert.Equal(DateTime.Today, loan.ReturnDate);
        Assert.False(loan.IsOverdue ?? true);
        Assert.Equal(LibraryConstants.CopyStatusAvailable, loan.Copy!.Status!.Name);
        Assert.DoesNotContain(check.Debts.ToList(), d => d.LoanId == loanId);
    }

    [Fact]
    public void ReturnCopy_Overdue_CreatesDebtRecord()
    {
        using var context = _fixture.CreateContext();
        Loan loan = context.Loans.First(l => l.Copy!.InventoryNumber == "ИН-000103");

        int expectedDays = (DateTime.Today - loan.DueDate).Days;
        Assert.True(expectedDays > 0, "Срок возврата в тестовых данных ещё не наступил.");

        var service = new LoanService();

        int overdueDays = service.ReturnCopy(loan.Id);

        Assert.Equal(expectedDays, overdueDays);

        using var check = _fixture.CreateContext();
        Loan returned = check.Loans.Include(l => l.Copy).ThenInclude(c => c.Status)
            .First(l => l.Id == loan.Id);
        Assert.Equal(DateTime.Today, returned.ReturnDate);
        Assert.True(returned.IsOverdue ?? false);
        Assert.Equal(LibraryConstants.CopyStatusAvailable, returned.Copy!.Status!.Name);

        Debt debt = check.Debts
            .Where(d => d.LoanId == loan.Id && d.ClosedAt != null)
            .First();
        Assert.Equal(expectedDays, debt.DaysOverdue);

        Assert.Contains(
            check.OperationLogs.Where(l => l.Operation == "Возврат книги").ToList(),
            l => l.Description!.Contains("просрочка"));
    }

    [Fact]
    public void ExtendLoan_ShiftsDueDateBy14Days()
    {
        using var context = _fixture.CreateContext();
        Loan loan = context.Loans.First(l => l.Copy!.InventoryNumber == "ИН-000110");
        DateTime oldDueDate = loan.DueDate;

        var service = new LoanService();

        string? error = service.ExtendLoan(loan.Id);

        Assert.Null(error);

        using var check = _fixture.CreateContext();
        Loan extended = check.Loans.First(l => l.Id == loan.Id);
        Assert.Equal(oldDueDate.AddDays(LibraryConstants.LoanExtensionDays), extended.DueDate);

        LoanExtension extension = check.LoanExtensions.Single(e => e.LoanId == loan.Id);
        Assert.Equal(oldDueDate, extension.OldDueDate);
        Assert.Equal(oldDueDate.AddDays(LibraryConstants.LoanExtensionDays), extension.NewDueDate);

        Assert.Contains(
            check.OperationLogs.Where(l => l.Operation == "Продление срока").ToList(),
            l => l.Description!.Contains($"№{loan.Id}"));
    }

    [Fact]
    public void ExtendLoan_ReturnedLoan_ReturnsError()
    {
        var ids = GetReferenceIds();
        int ivanovId;
        using (var context = _fixture.CreateContext())
        {
            ivanovId = context.Readers.First(r => r.TicketNumber == "БН-2023001").Id;
        }

        var service = new LoanService();
        Assert.Null(service.IssueCopy(ivanovId, "ИН-000108", ids.RoomFormId, ids.UserId));

        int loanId;
        using (var context = _fixture.CreateContext())
        {
            loanId = context.Loans.Single(l => l.Copy!.InventoryNumber == "ИН-000108").Id;
        }

        Assert.Equal(0, service.ReturnCopy(loanId));

        string? error = service.ExtendLoan(loanId);

        Assert.NotNull(error);
        Assert.Contains("продление невозможно", error!);
    }
}