using Xunit;

namespace E2ETests;

[CollectionDefinition(nameof(WebApplicationFactoryCollection))]
public class WebApplicationFactoryCollection : ICollectionFixture<WebApplicationFactoryFixture>
{
}
