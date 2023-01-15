using Xunit;

namespace E2ETests;

[CollectionDefinition(nameof(WebApplicationFactoryCollection))]
public sealed class WebApplicationFactoryCollection : ICollectionFixture<WebApplicationFactoryFixture>
{
}

[CollectionDefinition(nameof(AuthorizedWebApplicationFactoryCollection))]
public sealed class AuthorizedWebApplicationFactoryCollection : ICollectionFixture<AuthorizedWebApplicationFactoryFixture>
{
}

[CollectionDefinition(nameof(SharedWebApplicationFactoryCollection))]
public sealed class SharedWebApplicationFactoryCollection : ICollectionFixture<SharedWebApplicationFactoryFixture>
{
}
