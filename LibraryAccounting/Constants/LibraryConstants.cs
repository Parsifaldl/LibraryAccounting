namespace LibraryAccounting.Constants;

public static class LibraryConstants
{
    public const string ConnectionString =
        "Server=localhost\\SQLEXPRESS;Database=LibraryDb;Trusted_Connection=True;TrustServerCertificate=True;";

    public const int HomeLoanDays = 14;

    public const int LoanExtensionDays = 14;

    public const string CopyStatusAvailable = "Доступен";
    public const string CopyStatusIssued = "Выдан";
    public const string CopyStatusWrittenOff = "Списан";
    public const string CopyStatusLost = "Утерян";

    public const string LocationAbonement = "Абонемент";
    public const string LocationReadingRoom = "Читальный зал";

    public const string ReaderStatusActive = "Активен";
    public const string ReaderStatusBlocked = "Заблокирован";

    public const string LoanFormHome = "На дом";
    public const string LoanFormReadingRoom = "В читальный зал";
}