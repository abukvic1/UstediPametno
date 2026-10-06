# UstediPametno 

UstediPametno is a personal finance web application developed using ASP.NET Core. The idea behind the application is to help users keep track of their income, expenses, savings and monthly spending in one place.

The application allows users to create monthly financial plans, set savings goals, record transactions and track how much money they have available for spending. It also includes a badge system that rewards users for reaching savings milestones and for staying with the application over time.

## Features

### User Accounts

* User registration and login
* User authentication using ASP.NET Core Identity
* User roles: User and Administrator
* Each user can access and manage their own financial data

### Income and Expenses

Users can:

* Add and manage income sources
* Add and manage fixed expenses
* Record financial transactions
* Track their current financial situation

### Monthly Financial Plan

The monthly plan is used to organize the user's finances for a specific month.

The application calculates:

* Total income
* Total expenses
* Amount saved
* Available money for spending
* Remaining amount
* Daily spending budget

The available amount is recalculated when the user adds new income, expenses, transactions or savings.

### Savings Goals

Users can create savings goals and track their progress.

For example, a user can create a goal for:

* A vacation
* A new phone
* A car
* Education
* An emergency fund

The application keeps track of the amount saved towards each goal.

### Badges

UstediPametno includes a badge system that gives users achievements for reaching savings milestones and for their loyalty to the application.

For example, users can receive a badge after reaching an important savings milestone or after being registered for a certain period of time.

The application can also send a congratulatory email when a user reaches an important savings milestone.

## Technologies

The project was developed using:

* C#
* ASP.NET Core MVC
* Entity Framework Core
* SQL Server
* ASP.NET Core Identity
* Razor Views
* HTML
* CSS
* Bootstrap
* Git and GitHub

## Architecture

The application is organized into several layers:

```text
Controller
    ↓
Service
    ↓
Repository
    ↓
Entity Framework Core
    ↓
SQL Server
```

### Controllers

Controllers handle HTTP requests and connect the user interface with the application logic.

### Services

Services contain the main business logic of the application.

For example, the monthly plan service is responsible for calculating the available amount and daily spending budget.

### Repositories

Repositories are responsible for communicating with the database.

A generic repository is used for common database operations, while services use repositories when they need to access or modify data.

### Models

Models represent the main entities stored in the database, such as users, income sources, expenses, savings goals, monthly plans, transactions and badges.

### ViewModels

ViewModels are used to prepare the data that needs to be displayed in the views without exposing or passing unnecessary data directly from the database models.

## Database

The application uses SQL Server together with Entity Framework Core.

Some of the main entities are:

* `Korisnik`
* `IzvorPrihoda`
* `FiksniTrosak`
* `CiljStednje`
* `MjesecniPlan`
* `Transakcija`
* `Bedz`
* `KorisnikBedz`
* `MjesecniPlanCilj`

Entity Framework Core migrations are used to create and update the database.

## Authentication and Authorization

ASP.NET Core Identity is used for user authentication and account management.

Users can register, log in and log out of the application.

The application also uses roles to provide different permissions for users and administrators.

Authorization is handled using ASP.NET Core authorization and `[Authorize]` attributes.

## Dependency Injection

Dependency Injection is used throughout the application.

Services and repositories are registered in `Program.cs` and injected into controllers and other services when they are needed.

This makes the application easier to maintain and keeps the different parts of the application separated.

## Project Structure

```text
UstediPametno/
│
├── Controllers/
├── Data/
├── Migrations/
├── Models/
├── Repositories/
├── Services/
├── ViewModels/
├── Views/
├── wwwroot/
│
├── appsettings.json
├── Program.cs
└── UstediPametno.csproj
```

## How It Works

A typical operation in the application follows this process:

```text
User
 ↓
View
 ↓
Controller
 ↓
Service
 ↓
Repository
 ↓
Entity Framework Core
 ↓
SQL Server
```

For example, when a user adds a transaction, the controller receives the request and passes it to the appropriate service. The service performs the necessary business logic and uses the repository to save the data to the database.

## Getting Started

### Requirements

To run the project locally, you need:

* .NET SDK
* SQL Server
* Visual Studio or another IDE that supports ASP.NET Core
* Git

### Clone the repository

```bash
git clone https://github.com/YOUR-USERNAME/UstediPametno.git
```

Open the project in Visual Studio.

### Configure the database

Update the `DefaultConnection` connection string in `appsettings.json` with your SQL Server connection.

Example:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=UstediPametno;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

### Apply migrations

Run the following command in the Package Manager Console or terminal:

```bash
dotnet ef database update
```

### Run the application

Run the project from Visual Studio or use:

```bash
dotnet run
```

## Security

Sensitive information such as database credentials and email configuration should not be stored directly in a public repository.

Configuration values can be stored using environment variables, User Secrets or other appropriate configuration methods.

## Purpose of the Project

UstediPametno was created as a practical project for learning and applying concepts from web application development.

Through this project, I worked with:

* ASP.NET Core MVC
* C#
* Entity Framework Core
* SQL Server
* ASP.NET Core Identity
* Authentication and authorization
* Role-based access
* Dependency Injection
* Repository Pattern
* Service Layer
* CRUD operations
* Database migrations
* Business logic
* Email notifications
* Git and GitHub
* Application deployment

The project helped me understand how different parts of a web application work together, from the user interface and controllers to the business logic and database.

## Future Improvements

Some features I would like to add in the future are:

* More detailed financial statistics
* Charts for income and expenses
* Improved spending analysis
* More advanced savings recommendations
* Additional notifications
* Improved mobile responsiveness
* More filtering and reporting options

## Author

**Amina Bukvić**

This project was developed as a personal learning and portfolio project while studying Electrical Engineering and Computer Science.

---

⭐ Thank you for checking out UstediPametno!
