# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /src

# Install dependencies for globalization
RUN apk add --no-cache icu-libs

# Copy csproj and restore as distinct layers
COPY ["CityWeb.csproj", "./"]
RUN dotnet restore "CityWeb.csproj"

# Copy everything else and build
COPY . .
RUN dotnet publish "CityWeb.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine
WORKDIR /app

# Install dependencies for globalization
RUN apk add --no-cache icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

COPY --from=build /app/publish .

# Standard port for .NET 8+ container images
EXPOSE 8080

ENTRYPOINT ["dotnet", "CityWeb.dll"]
