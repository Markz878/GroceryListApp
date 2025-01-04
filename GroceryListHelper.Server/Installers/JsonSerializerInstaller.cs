using GroceryListHelper.Core.Domain.CartGroups;
using GroceryListHelper.Core.Domain.CartProducts;
using GroceryListHelper.Core.Domain.StoreProducts;
using GroceryListHelper.Server.Models.CartGroups;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GroceryListHelper.Server.Installers;

public class JsonSerializerInstaller : IInstaller
{
    public void Install(WebApplicationBuilder builder)
    {
        builder.Services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
        });
    }
}

[ExcludeFromCodeCoverage]
[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(HttpValidationProblemDetails))]
[JsonSerializable(typeof(ProblemDetails))]
[JsonSerializable(typeof(ValidationProblem))]
[JsonSerializable(typeof(CartProduct))]
[JsonSerializable(typeof(CartGroup))]
[JsonSerializable(typeof(CreateCartGroupRequest))]
[JsonSerializable(typeof(UpdateCartGroupNameRequest))]
[JsonSerializable(typeof(StoreProduct))]
[JsonSerializable(typeof(StoreProduct))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{ }
