#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

COPY ["Server/GroceryListHelper.Server.csproj", "GroceryListHelper/Server/"]
COPY ["Client/GroceryListHelper.Client.csproj", "GroceryListHelper/Client/"]
COPY ["Shared/GroceryListHelper.Shared.csproj", "GroceryListHelper/Shared/"]
COPY ["DataAccess/GroceryListHelper.DataAccess.csproj", "GroceryListHelper/DataAccess/"]
RUN dotnet restore "GroceryListHelper/Server/GroceryListHelper.Server.csproj"

COPY . ./
RUN dotnet publish "Server/GroceryListHelper.Server.csproj" -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "GroceryListHelper.Server.dll"]
EXPOSE 80
EXPOSE 443