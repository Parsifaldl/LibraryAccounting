using LibraryAccounting.Constants;
using LibraryAccounting.Data;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace LibraryAccounting.Tests;

public sealed class TestDatabaseFixture : IDisposable
{
    public string DatabaseName { get; }

    public TestDatabaseFixture()
    {
        DatabaseName = $"LibraryDb_Test_{Guid.NewGuid():N}";

        string scriptsDir = Path.Combine(AppContext.BaseDirectory, "DatabaseScripts");
        string createScript = File
            .ReadAllText(Path.Combine(scriptsDir, "CreateDB.sql"))
            .Replace("LibraryDb", DatabaseName);
        string fillScript = File
            .ReadAllText(Path.Combine(scriptsDir, "FillDB.sql"))
            .Replace("LibraryDb", DatabaseName);

        ExecuteScript(createScript);
        ExecuteScript(fillScript);

        LibraryDbContext.ConnectionStringOverride = BuildConnectionString();
    }

    public string BuildConnectionString()
    {
        return new SqlConnectionStringBuilder(LibraryConstants.ConnectionString)
        {
            InitialCatalog = DatabaseName,
        }.ConnectionString;
    }

    public LibraryDbContext CreateContext() => new();

    public void Dispose()
    {
        LibraryDbContext.ConnectionStringOverride = null;

        try
        {
            using var connection = new SqlConnection(MasterConnectionString());
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText =
                $"IF DB_ID(N'{DatabaseName}') IS NOT NULL " +
                $"ALTER DATABASE [{DatabaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; " +
                $"IF DB_ID(N'{DatabaseName}') IS NOT NULL DROP DATABASE [{DatabaseName}];";
            command.ExecuteNonQuery();
        }
        catch
        {

        }
    }

    private static string MasterConnectionString()
    {
        return new SqlConnectionStringBuilder(LibraryConstants.ConnectionString)
        {
            InitialCatalog = "master",
        }.ConnectionString;
    }

    private static void ExecuteScript(string script)
    {
        using var connection = new SqlConnection(MasterConnectionString());
        connection.Open();
        foreach (string batch in SplitBatches(script))
        {
            using var command = connection.CreateCommand();
            command.CommandText = batch;
            command.ExecuteNonQuery();
        }
    }

    private static IEnumerable<string> SplitBatches(string script)
    {
        var batches = new List<string>();
        var current = new List<string>();

        foreach (string line in script.Replace("\r\n", "\n").Split('\n'))
        {
            if (line.Trim().Equals("GO", StringComparison.OrdinalIgnoreCase))
            {
                batches.Add(string.Join("\n", current));
                current.Clear();
            }
            else
            {
                current.Add(line);
            }
        }

        if (current.Count > 0)
        {
            batches.Add(string.Join("\n", current));
        }

        return batches.Where(b => !string.IsNullOrWhiteSpace(b));
    }
}