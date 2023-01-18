namespace ApiIntegrationTests.Infrastucture;

[CollectionDefinition(nameof(WebApplicationFactoryCollection))]
public sealed class WebApplicationFactoryCollection : ICollectionFixture<WebApplicationFactoryFixture>
{
}