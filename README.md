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
-   **Region**: `ap-south-1`
-   **Registry**: `248732772276.dkr.ecr.ap-south-1.amazonaws.com`
-   **Repository**: `city-project`
-   **Tags**: `api-latest` and `web-latest`

### 2. Infrastructure Provisioning (CloudFormation)
The project includes a comprehensive AWS CloudFormation template (`cloudformation.yaml`) that provisions a production-ready environment:

*   **Compute**: ECS Fargate services for Frontend and Backend.
*   **Database**: Amazon RDS for SQL Server Express.
*   **Networking**:
    *   **Application Load Balancer (ALB)**: External entry point for the website.
    *   **AWS Cloud Map**: Internal Service Discovery (`cityproject.local`) for service-to-service communication.
*   **Observability**: CloudWatch Log Groups for centralized logging.

#### Deployment Command
To deploy the stack to AWS, use the following CLI command (replace placeholders with your specific IDs):

```powershell
aws cloudformation deploy `
  --stack-name CityProject-Prod `
  --template-file cloudformation.yaml `
  --region ap-south-1 `
  --capabilities CAPABILITY_IAM `
  --parameter-overrides `
    VpcId="vpc-XXXXXX" `
    PrivateSubnet1="subnet-XXXXXX" `
    PrivateSubnet2="subnet-XXXXXX" `
    PublicSubnet1="subnet-XXXXXX" `
    PublicSubnet2="subnet-XXXXXX" `
    DatabasePassword="YourSecurePassword123_" `
    CityApiImageUri="248732772276.dkr.ecr.ap-south-1.amazonaws.com/city-project:api-latest" `
    CityWebImageUri="248732772276.dkr.ecr.ap-south-1.amazonaws.com/city-project:web-latest"
```

### 3. Configuration & Networking
-   **Frontend-to-Backend**: `CityWeb` connects to `CityApi` using the internal DNS name `http://api.cityproject.local:8080` provided by Cloud Map.
-   **Database**: `CityApi` connects to the RDS instance via the connection string automatically injected into the container's environment variables.

## Project Structure
-   `/CityWeb`: Razor Pages project.
-   `/CityApi`: Web API project with EF Core logic.
-   `cloudformation.yaml`: AWS Infrastructure-as-Code template.
-   `docker-compose.yml`: Local orchestration.
-   `CityProject.sln`: Visual Studio Solution.
-   `build-and-push.bat`: AWS ECR automation script.

