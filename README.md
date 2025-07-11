# eShift

## Description

eShift is an ASP.NET Core Web API project designed for managing logistics operations, including branches, admins, assistants, drivers, customers, jobs, trips, and more. The API follows the MVC pattern and provides structured, logged, and CORS-enabled endpoints for all major entities.

## Table of Contents
- [Description](#description)
- [Installation](#installation)
- [Usage](#usage)
- [Features](#features)
- [Configuration](#configuration)
- [License](#license)

## Installation

1. **Clone the repository:**
   ```sh
   git clone https://github.com/yourusername/eshift.git
   cd eshift
   ```
2. **Restore dependencies:**
   ```sh
   dotnet restore
   ```
3. **Update database connection string** in `appsettings.json`.
4. **Apply migrations (if needed):**
   ```sh
   dotnet ef database update
   ```
5. **Run the application:**
   ```sh
   dotnet run
   ```

## Usage

- The API is available at `http://localhost:5000` by default.
- Use tools like Postman to interact with endpoints (see `apiDocument.json` for ready-to-import requests).
- All endpoints return structured responses with status, message, timestamp, traceId, and data.

## Features
- ASP.NET Core Web API with MVC pattern
- Entity Framework Core with MySQL
- CRUD endpoints for all major entities (Admin, Assistant, Branch, City, Container, Customer, Driver, Job, JobStop, Load, Lorry, Trip)
- Structured API responses and logging
- CORS enabled for cross-origin requests
- OpenAPI/Swagger support for API documentation

## Configuration
- **Database:** Set your MySQL connection string in `appsettings.json` under `DefaultConnection`.
- **CORS:** Configured to allow any origin, header, and method by default. Adjust in `Program.cs` as needed.
- **API Documentation:** OpenAPI/Swagger is enabled in development mode.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
