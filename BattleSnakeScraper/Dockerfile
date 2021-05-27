FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["BattleSnakeScraper/BattleSnakeScraper.csproj", "BattleSnakeScraper/"]
RUN dotnet restore "BattleSnakeScraper/BattleSnakeScraper.csproj"
COPY . .
WORKDIR "/src/BattleSnakeScraper"
RUN dotnet build "BattleSnakeScraper.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BattleSnakeScraper.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BattleSnakeScraper.dll"]
