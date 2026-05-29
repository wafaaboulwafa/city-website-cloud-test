# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /src

# Install dependencies for SQL Client
RUN apk add --no-cache icu-libs

# Copy csproj and restore as distinct layers
COPY ["CityApi.csproj", "./"]
RUN dotnet restore "CityApi.csproj"

# Copy everything else and build
COPY . .
RUN dotnet publish "CityApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine
WORKDIR /app

# Install dependencies for SQL Client
RUN apk add --no-cache icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "CityApi.dll"]
