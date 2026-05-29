# Gemini CLI Instructional Context: City Listing Project

## Project Overview
This project is a cloud-optimized, two-tier .NET 10 application specifically designed for training on **AWS Fargate** deployment. It demonstrates a decoupled architecture where a Razor Pages frontend communicates with a RESTful Web API backend.

### Architecture
-   **CityWeb (Frontend)**: .NET 10 Razor Pages. Consumes the API via `HttpClient`.
-   **CityApi (Backend)**: .NET 10 Web API. Manages data via Entity Framework Core and MSSQL.
-   **Database**: MSSQL (Local SQL Express or containerized SQL Server).

## Core Technologies
-   **Framework**: .NET 10 (C#)
-   **Data Access**: Entity Framework Core (SQL Server)
-   **Containerization**: Docker with **Alpine Linux** base images for minimal footprint.
-   **Observability**: Integrated HTTP Logging and custom semantic request logging.
-   **API Documentation**: Swagger/OpenAPI (mapped to root in CityApi).

## Key Commands & Workflows

### Local Development
*   **Full Stack (Docker Compose)**:
    ```bash
    docker-compose up -d --build
    ```
    *   Website: `http://localhost:8080`
    *   API (Swagger): `http://localhost:5000`
*   **Monitoring Logs**:
    ```bash
    docker-compose logs -f city-web city-api
    ```
*   **Stop Stack**:
    ```bash
    docker-compose down -v
    ```

### Building & Testing
*   **Build Solution**: `dotnet build CityProject.sln`
*   **Run Projects Individually**: Use the Visual Studio Solution or `dotnet run` within service directories.

### AWS Deployment (ECR)
*   **Automated Push**:
    ```bash
    .\build-and-push.bat
    ```
    *   *Note*: Ensure `AWS_PROFILE` and `ECR_REGISTRY` variables are correct in the script.

## Development Conventions

### Coding Style
*   **Decoupling**: The frontend (`CityWeb`) must **never** access the database directly. It must go through `CityApi`.
*   **Logging**: Maintain semantic logging in middleware for both services (e.g., `"Page Request: {Method} {Path}"`).
*   **Configuration**: Prefer `appsettings.json` for defaults and **Environment Variables** for secrets/overrides (essential for Fargate).

### Database Handling
*   The `CityApi` project includes an **auto-seeding** mechanism in `Program.cs`.
*   `context.Database.EnsureCreated()` is used for rapid training setup; migrations should be considered for production-ready versions.

### Containerization
*   Always use the optimized multi-stage Alpine Dockerfiles.
*   The `icu-libs` package is required in Alpine for SQL Client globalization support.

## Project Structure
-   `/CityWeb`: Razor Pages frontend.
-   `/CityApi`: Data-handling backend.
-   `docker-compose.yml`: Orchestrates services and SQL Server container.
-   `CityProject.sln`: Unified solution for IDE development.
-   `build-and-push.bat`: Automation for AWS ECR deployments.
