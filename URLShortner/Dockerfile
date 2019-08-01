FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["URLShortner/URLShortner.csproj", "URLShortner/"]
RUN dotnet restore "URLShortner/URLShortner.csproj"
COPY . .
WORKDIR "/src/URLShortner"
RUN dotnet build "URLShortner.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "URLShortner.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "URLShortner.dll"]