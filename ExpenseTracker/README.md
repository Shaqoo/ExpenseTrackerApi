# Expense Tracker API

Welcome to the Expense Tracker API! This is a robust backend service built with .NET 8, designed to power a personal finance application. It provides a comprehensive set of features for managing users, categories, expenses, and generating financial reports.

## ✨ Features

- **User Management & Authentication**: Secure user registration and login using JWT (JSON Web Tokens).
- **Expense Tracking**: Full CRUD (Create, Read, Update, Delete) operations for expenses.
- **Category Management**: Users can create and manage their own expense categories.
- **Multi-Currency Support**: Expenses can be recorded in different currencies, with automatic conversion to a base currency (USD).
- **Automatic Exchange Rate Updates**: A background service periodically fetches and updates currency exchange rates.
- **Reporting & Analytics**: Endpoints to generate financial summaries and KPI reports.
- **Activity Logging**: User actions are logged to a separate NoSQL database (MongoDB) for auditing and analysis.
- **Robust Caching**: Redis is used for caching frequently accessed data like exchange rates to improve performance.
- **Data Validation**: FluentValidation is used for validating incoming request data to ensure data integrity.
- **Rate Limiting**: Protects the API from abuse with a fixed-window rate limiter.
- **Health Checks**: Provides health check endpoints to monitor the status of the API and its dependencies (PostgreSQL, MongoDB, Redis).
- **Containerized Environment**: Full support for running the application and its dependencies using Docker and Docker Compose.

## 🛠️ Technologies Used

- **Framework**: .NET 8, ASP.NET Core (using Minimal APIs)
- **Primary Database**: PostgreSQL with Entity Framework Core
- **Logging Database**: MongoDB for activity logs
- **Caching**: Redis
- **Authentication**: JWT (JSON Web Tokens)
- **API Documentation**: Swagger (OpenAPI)
- **Validation**: FluentValidation
- **Testing**: xUnit, FluentAssertions, and Testcontainers for reliable integration tests.
- **Containerization**: Docker & Docker Compose

## 🏗️ Project Structure

The project follows a clean and organized structure to promote separation of concerns.

```
ExpenseTracker/
├── BackgroundServices/   # Hosted services (e.g., ExchangeRateUpdaterJob)
├── Configurations/       # Database initialization and entity type configurations
├── Endpoints/            # Minimal API endpoint definitions
├── Entities/             # Domain models and data transfer objects (DTOs)
├── ExDbContext/          # DbContext for PostgreSQL and MongoDB
├── Extensions/           # Extension methods for service registration (DI)
├── Middleware/           # Custom middleware (e.g., GlobalExceptionHandler)
├── Migrations/           # EF Core database migrations
├── Repositories/         # Data access layer (Repository pattern)
├── Services/             # Business logic layer
├── UnitOfWork/           # Unit of Work pattern implementation
├── appsettings.json      # Configuration files
├── Program.cs            # Application entry point and startup configuration
└── ExpenseTracker.csproj # Project file
```

## 🚀 Getting Started

Follow these instructions to get the project up and running on your local machine.

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)

### 1. Clone the Repository

```bash
git clone <your-repository-url>
cd ExpenseTrackerApi/ExpenseTracker
```

### 2. Configuration

The application is configured to run seamlessly with Docker Compose. The `docker-compose.yml` file (you'll need to create this) sets up the API service along with PostgreSQL, Redis, and MongoDB containers.

Create a `docker-compose.yml` file in the root of the repository (alongside the `ExpenseTracker` folder) with the following content:

```yaml
version: '3.8'

services:
  postgres:
    image: postgres:15
    container_name: expense_tracker_postgres
    environment:
      - POSTGRES_USER=admin
      - POSTGRES_PASSWORD=admin
      - POSTGRES_DB=expensedb
    ports:
      - "5432:5432"
    volumes:
      - postgres_data:/var/lib/postgresql/data

  redis:
    image: redis:7-alpine
    container_name: expense_tracker_redis
    ports:
      - "6379:6379"

  mongo:
    image: mongo:7.0
    container_name: expense_tracker_mongo
    ports:
      - "27017:27017"
    volumes:
      - mongo_data:/data/db

  expensetracker-api:
    build:
      context: .
      dockerfile: ExpenseTracker/Dockerfile
    container_name: expense_tracker_api
    ports:
      - "8080:8080"
      - "8081:8081"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ASPNETCORE_URLS=http://+:8080;https://+:8081
      - DatabaseSettings__DB_HOST=postgres
      - DatabaseSettings__DB_PORT=5432
      - DatabaseSettings__DB_USERNAME=admin
      - DatabaseSettings__DB_PASSWORD=admin
      - DatabaseSettings__DB_NAME=expensedb
      - ConnectionStrings__Redis=redis:6379
      - ConnectionStrings__MongoDb=mongodb://mongo:27017
      - JwtSettings__Secret=YourSuperSecretKeyThatIsLongAndSecureEnough
      - JwtSettings__Issuer=https://localhost:8081
      - JwtSettings__Audience=https://localhost:8081
    depends_on:
      - postgres
      - redis
      - mongo

volumes:
  postgres_data:
  mongo_data:
```

**Note**: You will also need a `Dockerfile` inside the `ExpenseTracker` directory. If you don't have one, here is a standard one:

```Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Copy csproj and restore as distinct layers
COPY *.csproj .
RUN dotnet restore

# Copy everything else and build
COPY . .
RUN dotnet publish -c Release -o /app/publish

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ExpenseTracker.dll"]
```

### 3. Run the Application

With Docker running, open a terminal in the root of the repository and run:

```bash
docker-compose up --build
```

This command will:
1. Pull the required Docker images for PostgreSQL, Redis, and MongoDB.
2. Build the Docker image for the Expense Tracker API.
3. Start all the services.

The API will be available at `https://localhost:8081` and `http://localhost:8080`.

### 4. Explore the API

Once the application is running, you can explore the API documentation and interact with the endpoints using Swagger UI.

Navigate to **https://localhost:8081/swagger** in your browser.

## 📖 API Endpoints

The API is organized into logical groups of endpoints:

- `/auth`: User registration and login.
- `/users`: User profile management.
- `/categories`: CRUD operations for expense categories.
- `/expenses`: CRUD operations for expenses.
- `/reports`: Endpoints for generating financial reports.
- `/activitylogs`: Retrieve user activity logs.
- `/health`: Health check endpoint.

### Authentication

1.  **Register a user**: Use the `POST /auth/register` endpoint.
2.  **Log in**: Use the `POST /auth/login` endpoint with the registered credentials to receive a JWT token.
3.  **Authorize requests**: Click the "Authorize" button in Swagger UI, and enter `Bearer {your_token}` to authenticate subsequent requests.

## 🧪 Running Tests

The project includes a suite of integration tests that use `Testcontainers` to spin up real database instances in Docker, ensuring that tests run in an isolated and realistic environment.

To run the tests, navigate to the project directory and execute:

```bash
dotnet test
```