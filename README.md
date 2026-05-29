# City Listing Website (.NET Core + MSSQL + Docker)

This project is a two-tier .NET Core application designed for training on **AWS Cloud Hosting using Fargate**. It consists of a Razor Pages website that communicates with a separate Web API to retrieve and display a list of cities from an MSSQL database.

## Architecture

-   **CityWeb**: A .NET 10 Razor Pages application (Frontend).
-   **CityApi**: A .NET 10 Web API that handles data access via Entity Framework Core (Backend).
-   **MSSQL**: A SQL Server database (Local SQL Express or Docker container).

## Features

-   **Decoupled Design**: Frontend and Backend are separate services communicating via REST.
-   **Cloud Optimized**: Uses lightweight **Alpine Linux** base images (~60MB).
-   **Auto-Seeding**: The API automatically creates the database schema and seeds sample data (New York, London, Tokyo, etc.) on startup if the DB is empty.
-   **High Observability**:
    -   Detailed HTTP Logging (headers, status codes, paths).
    -   Semantic Activity Logging (e.g., "Page Request: GET /", "API Request: GET /api/cities").
-   **Swagger UI**: Interactive API documentation available at the root of the API service.

## Local Development

### Prerequisites
-   .NET 10 SDK
-   Docker Desktop
-   SQL Server Express (optional, for direct local run)

### Option 1: Visual Studio / VS Code
1.  Open `CityProject.sln`.
2.  Configure your connection string in `CityApi/appsettings.json`.
3.  Run both projects (Set multiple startup projects).

### Option 2: Docker Compose (Recommended)
Run the entire stack (Web + API + DB) with a single command:
```bash
docker-compose up -d --build
```
-   **Website**: [http://localhost:8080](http://localhost:8080)
-   **API (Swagger)**: [http://localhost:5000](http://localhost:5000)

## AWS Fargate Deployment

### 1. Build and Push Images
Use the provided `build-and-push.bat` script to build and push both images to a single Amazon ECR repository using service-specific tags:

```bash
.\build-and-push.bat
```

The script is configured for:
-   **Registry**: `248732772276.dkr.ecr.ap-south-1.amazonaws.com`
-   **Repository**: `city-project`
-   **Tags**: `api-latest` and `web-latest`

### 2. Networking
-   Deploy the services into an ECS Cluster using the **Fargate** launch type.
-   Use **AWS Cloud Map** (Service Discovery) or an **Internal Load Balancer** to allow `CityWeb` to communicate with `CityApi`.

### 3. Configuration (Environment Variables)
In your ECS Task Definitions, set the following:

-   **CityApi**:
    -   `ConnectionStrings__DefaultConnection`: Point to your **Amazon RDS** (MSSQL) endpoint.
-   **CityWeb**:
    -   `ApiSettings__CityApiBaseUrl`: Point to the internal URL of your `CityApi` service (e.g., `http://city-api.local:8080`).

## Project Structure
-   `/CityWeb`: Razor Pages project.
-   `/CityApi`: Web API project with EF Core logic.
-   `docker-compose.yml`: Local orchestration.
-   `CityProject.sln`: Visual Studio Solution.
-   `build-and-push.bat`: AWS ECR automation script.
