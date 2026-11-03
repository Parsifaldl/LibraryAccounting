namespace LibraryAccounting.Services;

public class ValidationResult
{
    private readonly List<string> _errors = [];

    public bool IsValid => _errors.Count == 0;

    public IReadOnlyList<string> Errors => _errors;

    public string ErrorMessage => string.Join(Environment.NewLine, _errors);

    public void AddError(string message) => _errors.Add(message);
}