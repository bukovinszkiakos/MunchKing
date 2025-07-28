# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . .

RUN dotnet restore MunchKing.csproj
RUN dotnet build MunchKing.csproj -c Release -o /app/build
RUN dotnet publish MunchKing.csproj -c Release -o /app/publish

# Final stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

RUN mkdir -p /app/wwwroot/uploads

ENTRYPOINT ["dotnet", "MunchKing.dll"]
