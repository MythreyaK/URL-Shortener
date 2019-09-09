FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["URLShortener/URLShortener.csproj", "URLShortener/"]
RUN dotnet restore "URLShortener/URLShortener.csproj"
COPY . .
WORKDIR "/src/URLShortener"
RUN dotnet build "URLShortener.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "URLShortener.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "URLShortener.dll"]