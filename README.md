# RentACar Project

This document provides an overview of the RentACar application, its technology stack, and how initial data (fixtures) is managed.

## Tech Stack

The application is built using the .NET ecosystem with the following core technologies:

*   **Framework:** ASP.NET Core 8.0 - A cross-platform, high-performance framework for building modern, cloud-based, internet-connected applications.
*   **Object-Relational Mapper (ORM):** Entity Framework Core 8 - Used for database interactions, simplifying data access and management.
*   **Database:** Microsoft SQL Server - The primary relational database used to store application data. Connection is configured via the `RentCarConnection` string in `appsettings.json`.
*   **Authentication & Authorization:** ASP.NET Core Identity - Manages user accounts, roles, and security.
*   **Real-time Communication:** SignalR - Implemented for features like real-time chat.
*   **Project Structure:** The solution is organized into several projects, likely following a layered architecture:
    *   `RentACar.Core`: Contains core business logic and domain models.
    *   `RentACar.Infrastructure`: Handles data access, external services, and other infrastructure concerns.
    *   `RentACar`: The main ASP.NET Core web application project.
    *   `RentACar.Tests`: Contains unit and integration tests.

## Fixtures (Data Seeding)

Initial data for the application is populated using a data seeding mechanism. This ensures that the application has necessary base data (e.g., user roles, categories, sample cars) when it's first run or when the database is initialized.

The seeding process is orchestrated by `ApplicationSeeder` which is invoked during application startup (`Program.cs`). This seeder calls other specific seeders to populate different parts of the database.

The primary seeder classes include:

*   **`ApplicationSeeder`**: The main seeder that coordinates calls to other seeders.
*   **`RoleSeeder`**: Populates initial user roles (e.g., Administrator, User).
*   **`CategorySeeder`**: Adds predefined car categories.
*   **`CarSeeder`**: Adds sample car data for demonstration and testing.
*   **`RentBillSeeder`**: Populates sample rental bill information.

These seeder classes are typically located within the `RentACar.Infrastructure` project, under the `Data/Seed/` directory. They use Entity Framework Core to interact with the database and insert the initial data.

To ensure data is seeded:
1.  Make sure the database connection string (`RentCarConnection` in `appsettings.json`) is correctly configured for your SQL Server instance.
2.  When the application starts, if migrations are applied and the database is created, the seeding logic will automatically run.

This approach provides a consistent and manageable way to handle initial application data. 