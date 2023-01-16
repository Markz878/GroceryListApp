using Xunit;

namespace E2ETests.Infrastructure;

[CollectionDefinition(nameof(WebApplicationFactoryCollection))]
public sealed class WebApplicationFactoryCollection : ICollectionFixture<WebApplicationFactoryFixture>
{
}
