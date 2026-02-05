# Swagger UI Default Configuration for Aspire

## Summary
Swagger UI has been configured as the default page for your WolverineIssue API when running in Aspire.

## Changes Made

### 1. Program.cs
- **Swagger Services**: Added `AddEndpointsApiExplorer()` and `AddSwaggerGen()` to register Swagger services
- **Swagger Middleware**: Configured `UseSwagger()` and `UseSwaggerUI()` to be available in all environments (not just Development)
- **SwaggerUI Configuration**: Set custom endpoint and route prefix to `/swagger`
- **Root Redirect**: Added a root path (`/`) endpoint that redirects to `/swagger` so Aspire dashboard opens Swagger by default

### 2. launchSettings.json
- **Browser Launch**: Enabled `launchBrowser: true` for both HTTP and HTTPS profiles
- **Launch URL**: Set `launchUrl: "swagger"` to open Swagger UI by default when launching the project directly
- This ensures when you run the API standalone (not through Aspire), it opens to Swagger

### 3. AppHost.cs
- **External Endpoints**: Added `.WithExternalHttpEndpoints()` to make the API accessible from external browsers
- This allows the Aspire dashboard to properly link to your API's Swagger UI

### 4. WolverineIssue.csproj
- Added `Swashbuckle.AspNetCore` package (version 10.1.1)

## How to Use

### Running with Aspire Dashboard
1. Start the Aspire AppHost project
2. Open the Aspire Dashboard (it will open automatically)
3. Click on the API endpoint URL in the dashboard
4. You'll be automatically redirected to `/swagger` showing the Swagger UI

### Running the API Standalone
1. Run the WolverineIssue project directly
2. A browser will automatically open to the Swagger UI at `/swagger`

### Accessing Swagger UI
- **Swagger UI**: `https://localhost:7238/swagger` or `http://localhost:5250/swagger`
- **Swagger JSON**: `https://localhost:7238/swagger/v1/swagger.json`
- **Root URL**: `https://localhost:7238/` (automatically redirects to Swagger)

## API Endpoints in Swagger

Your `/publish` endpoint is now documented in Swagger UI where you can:
- View the endpoint documentation
- Test the endpoint interactively
- See request/response schemas
- Try it out with sample data

## Notes
- Swagger UI is now available in all environments (not just Development) to support Aspire
- The root path (`/`) automatically redirects to Swagger for convenience
- All Aspire integrations point to Swagger UI by default
