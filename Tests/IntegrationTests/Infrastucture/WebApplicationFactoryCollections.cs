namespace GroceryListHelper.Tests.IntegrationTests.Infrastucture;

[CollectionDefinition(nameof(WebApplicationFactoryCollection))]
public sealed class WebApplicationFactoryCollection : ICollectionFixture<WebApplicationFactoryFixture>
{
}