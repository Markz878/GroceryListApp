using Xunit;

namespace E2ETests;

[CollectionDefinition(nameof(AuthorizedWebApplicationFactoryCollection))]
public class AuthorizedWebApplicationFactoryCollection : ICollectionFixture<AuthorizedWebApplicationFactoryFixture>
{
}
