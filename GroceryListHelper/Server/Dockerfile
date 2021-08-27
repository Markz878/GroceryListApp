#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["GroceryListHelper/Server/GroceryListHelper.Server.csproj", "GroceryListHelper/Server/"]
COPY ["GroceryListHelper/Client/GroceryListHelper.Client.csproj", "GroceryListHelper/Client/"]
COPY ["GroceryListHelper/Shared/GroceryListHelper.Shared.csproj", "GroceryListHelper/Shared/"]
COPY ["GroceryListHelper.DataAccess/GroceryListHelper.DataAccess.csproj", "GroceryListHelper.DataAccess/"]
RUN dotnet restore "GroceryListHelper/Server/GroceryListHelper.Server.csproj"
COPY . .
WORKDIR "/src/GroceryListHelper/Server"
RUN dotnet build "GroceryListHelper.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GroceryListHelper.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GroceryListHelper.Server.dll"]