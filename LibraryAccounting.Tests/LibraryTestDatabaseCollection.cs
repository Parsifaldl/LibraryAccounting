using Xunit;

namespace LibraryAccounting.Tests;

[CollectionDefinition("LibraryTestDatabase")]
public sealed class LibraryTestDatabaseCollection : ICollectionFixture<TestDatabaseFixture>
{
}