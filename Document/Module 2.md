# Module 2: Database and Entity Framework Core Configuration

## 1. Introduction

Database configuration is one of the fundamental stages in developing the Campus Placement Management Platform. The application needs a reliable mechanism to store, retrieve, update and manage information related to users, colleges, placement officers, companies, students, job openings and placement applications.

To achieve this, the project uses Microsoft SQL Server as its relational database management system and Entity Framework Core (EF Core) as its Object-Relational Mapping (ORM) framework.

ASP.NET Core Identity is integrated with Entity Framework Core to manage user accounts, authentication-related information and role-based access data.

This module establishes the database infrastructure required for the remaining modules of the application.

## 2. Objective

The main objective of this module is to establish a connection between the ASP.NET Core Web API and Microsoft SQL Server and configure ASP.NET Core Identity using Entity Framework Core.

The implementation provides:

- Database connectivity using SQL Server.
- Object-Relational Mapping using Entity Framework Core.
- Centralized database management through ApplicationDbContext.
- User management using ASP.NET Core Identity.
- Role management for different application users.
- Dependency injection for database and Identity services.
- Database schema creation and version management using migrations.

## 3. Technologies and Packages Used

### 3.1 Microsoft SQL Server

Microsoft SQL Server is a relational database management system (RDBMS) developed by Microsoft.

It stores application data in structured tables consisting of rows and columns.

For example, the Campus Placement Management Platform will eventually contain tables such as:

- AspNetUsers – stores registered user account information.
- AspNetRoles – stores application roles.
- Colleges – stores college information.
- Students – stores student information.
- Companies – stores company information.
- JobOpenings – stores job opening information.
- JobApplications – stores student job applications.

SQL Server maintains relationships between tables using primary keys and foreign keys.

**Why SQL Server is used:**

- Supports relational data storage.
- Provides transaction management.
- Maintains data integrity through constraints.
- Supports relationships between multiple entities.
- Integrates with Entity Framework Core.
- Provides secure and reliable database operations.

### 3.2 Entity Framework Core (EF Core)

Entity Framework Core is an Object-Relational Mapper (ORM) developed by Microsoft.

An ORM allows developers to work with database records using programming language objects instead of writing SQL queries for every operation.

Normally, an application needs SQL queries to communicate with a relational database.

For example:

```sql
SELECT * FROM AspNetUsers;
```

Using Entity Framework Core, the same operation can be performed using LINQ:

```csharp
var users = await _context.Users.ToListAsync();
```

EF Core translates this LINQ expression into an appropriate SQL query and executes it against SQL Server.

**Major responsibilities of EF Core:**

1. Maps C# classes to database tables.
2. Maps class properties to table columns.
3. Converts LINQ queries into SQL queries.
4. Tracks changes made to entity objects.
5. Generates SQL commands for inserting, updating and deleting records.
6. Manages database relationships.
7. Maintains database schema changes through migrations.

### 3.3 Object-Relational Mapping Architecture

ORM acts as a bridge between the object-oriented programming model and the relational database model.

The application works with C# objects, while SQL Server stores the corresponding information in tables.

For example:

C# model:

```csharp
public class Student
{
    public int StudentId { get; set; }

    public string RegisterNumber { get; set; }
        = string.Empty;

    public string Department { get; set; }
        = string.Empty;
}
```

Corresponding database table concept:

| StudentId | RegisterNumber | Department |
|---|---|---|
| 1 | 22AIDS001 | AI & DS |
| 2 | 22AIDS002 | Computer Science |

EF Core maps the Student class to a database table and its properties to database columns.

This reduces the need to manually write SQL statements and makes database operations easier to maintain.

## 4. Package Installation

The following NuGet packages were installed to support database connectivity, Identity integration and migration management.

### 4.1 Microsoft.EntityFrameworkCore.SqlServer

This package provides the SQL Server database provider for Entity Framework Core.

It allows EF Core to communicate with Microsoft SQL Server.

It provides the `UseSqlServer()` extension method, which configures EF Core to use SQL Server as its database provider.

Example:

```csharp
options.UseSqlServer(connectionString);
```

Without this provider, EF Core would not know how to communicate with SQL Server through its SQL Server-specific provider.

### 4.2 Microsoft.EntityFrameworkCore.Design

This package provides design-time functionality required by Entity Framework Core tools.

It is used when executing commands such as:

```powershell
dotnet ef migrations add InitialIdentitySetup
```

The EF Core tools use the application's database context and model configuration to generate migration files.

### 4.3 Microsoft.AspNetCore.Identity.EntityFrameworkCore

This package integrates ASP.NET Core Identity with Entity Framework Core.

It provides the `IdentityDbContext` class, which includes the database configuration required to store Identity information.

It enables the application to persist:

- User accounts.
- Roles.
- User-role relationships.
- User claims.
- External login information.
- Authentication tokens.

### 4.4 Microsoft.AspNetCore.Authentication.JwtBearer

This package provides JWT Bearer authentication support.

It allows the API to validate JSON Web Tokens sent by clients in HTTP requests.

It will be configured in the Authentication module when implementing JWT-based authentication.

## 5. SQL Server Connection String Configuration

A connection string contains the information required for an application to connect to a database.

It is configured inside the `appsettings.json` file.

Implementation:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.\\SQLEXPRESS;Database=CampusPlacementDB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
  }
}
```

### Explanation of each connection string property

**Server=.\\SQLEXPRESS**

Specifies the SQL Server instance to which the application connects.

- `.` represents the local computer.
- `SQLEXPRESS` represents the SQL Server Express instance name.

Therefore, `.\\SQLEXPRESS` refers to a local SQL Server Express instance.

**Database=CampusPlacementDB**

Specifies the database name.

If the database does not exist, EF Core migrations can create it when the database update command is executed, provided the SQL Server instance and permissions are configured correctly.

**Trusted_Connection=True**

Uses Windows authentication to connect to SQL Server.

This means the application uses the Windows credentials of the running process instead of specifying a SQL Server username and password in the connection string.

**TrustServerCertificate=True**

Trusts the SQL Server certificate without validating its certificate chain.

This is commonly used in local development environments. Production environments should use properly configured and validated TLS certificates.

**MultipleActiveResultSets=true**

Enables Multiple Active Result Sets (MARS).

It allows multiple active result sets on a single SQL Server connection. This is a SQL Server connection option and does not mean that multiple database transactions are automatically created.

### How the connection string is accessed

In `Program.cs`:

```csharp
var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection");
```

`builder.Configuration` provides access to the application's configuration sources.

`GetConnectionString("DefaultConnection")` retrieves the value associated with the `DefaultConnection` key inside the `ConnectionStrings` section.

The retrieved value is passed to Entity Framework Core during database service registration.

**Why use appsettings.json?**

It separates configuration information from application source code.

This makes it easier to change database settings without modifying the database access implementation.

Sensitive production credentials should be stored using secure configuration providers, such as environment variables or .NET user secrets during development, rather than committed to source control.

## 6. ApplicationUser – Extending ASP.NET Core Identity

ASP.NET Core Identity provides a built-in user management system.

It includes an existing `IdentityUser` class containing commonly required user account properties.

Instead of creating a completely new authentication user model, the project extends the existing IdentityUser class.

Implementation in `Models/ApplicationUser.cs`:

```csharp
using Microsoft.AspNetCore.Identity;

namespace CampusPlacementManagementPlatform.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;

        public AccountStatus AccountStatus { get; set; }
            = AccountStatus.Pending;

        public DateTime CreatedAt { get; set; }
            = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}
```

### 6.1 Why inherit from IdentityUser?

`IdentityUser` already provides several user account properties.

Some important inherited properties are:

| Property | Purpose |
|---|---|
| Id | Unique identifier of the user |
| UserName | User's login name |
| Email | User's email address |
| PasswordHash | Stores the hashed password |
| PhoneNumber | User's phone number |
| EmailConfirmed | Indicates whether the email is confirmed |
| PhoneNumberConfirmed | Indicates whether the phone number is confirmed |
| AccessFailedCount | Tracks unsuccessful login attempts |
| LockoutEnd | Stores the lockout expiration time |

By inheriting from `IdentityUser`, the project can reuse these existing features rather than implementing user account management from scratch.

### 6.2 Custom properties

The project adds additional properties to represent application-specific user information.

**FullName**

```csharp
public string FullName { get; set; } = string.Empty;
```

Stores the full name of the user.

This information can be used for students, placement officers, company HR representatives and administrators.

**AccountStatus**

```csharp
public AccountStatus AccountStatus { get; set; }
    = AccountStatus.Pending;
```

Represents the current account status.

The default value is Pending.

This supports the platform's approval workflow, where certain user accounts need to be reviewed before they can access their permitted functionality.

**CreatedAt**

```csharp
public DateTime CreatedAt { get; set; }
    = DateTime.UtcNow;
```

Stores the date and time when the user account is created.

`DateTime.UtcNow` returns the current time in Coordinated Universal Time (UTC).

Using UTC helps maintain consistent timestamps across different time zones.

**UpdatedAt**

```csharp
public DateTime? UpdatedAt { get; set; }
```

Stores the last modification date and time.

The property is nullable because a newly created account may not have been updated after registration.

### 6.3 AccountStatus Enum

An enumeration defines a fixed collection of named constant values.

The project uses an enum to represent the account status.

Implementation in `Models/AccountStatus.cs`:

```csharp
namespace CampusPlacementManagementPlatform.Models
{
    public enum AccountStatus
    {
        Pending,
        Approved,
        Rejected,
        Suspended,
        Inactive
    }
}
```

Explanation:

- Pending – the account is waiting for approval.
- Approved – the account has been approved.
- Rejected – the account has been rejected.
- Suspended – the account has temporarily restricted access.
- Inactive – the account is not currently active.

Using an enum is better than using arbitrary strings because it provides a predefined set of allowed values and reduces typing errors.

By default, C# assigns integer values to enum members starting from zero.

Therefore:

| Status | Default integer value |
|---|---:|
| Pending | 0 |
| Approved | 1 |
| Rejected | 2 |
| Suspended | 3 |
| Inactive | 4 |

EF Core normally stores an enum property as its underlying integer unless a different conversion is configured.

## 7. ApplicationDbContext – Database Context

`ApplicationDbContext` is one of the most important components of Entity Framework Core.

A DbContext represents a session with the database.

It is responsible for managing entity objects, tracking changes and coordinating database operations.

Implementation in `Data/ApplicationDbContext.cs`:

```csharp
using CampusPlacementManagementPlatform.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CampusPlacementManagementPlatform.Data
{
    public class ApplicationDbContext
        : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
```

### 7.1 Why inherit from IdentityDbContext?

`IdentityDbContext<ApplicationUser>` is a specialized DbContext provided by ASP.NET Core Identity.

It includes the database model required for Identity operations.

It uses the custom ApplicationUser class instead of the default IdentityUser class.

This allows Identity to store both its built-in user properties and the additional properties defined in ApplicationUser.

### 7.2 Constructor explanation

```csharp
public ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options)
    : base(options)
{
}
```

The constructor receives database configuration through dependency injection.

`DbContextOptions<ApplicationDbContext>` contains configuration information such as:

- Database provider.
- SQL Server connection details.
- Other EF Core configuration options.

The `base(options)` statement passes these settings to the parent `IdentityDbContext` class.

This allows the base DbContext to use the SQL Server configuration registered in Program.cs.

### 7.3 What is DbSet?

A DbSet represents a collection of entities associated with a database table.

For example:

```csharp
public DbSet<Student> Students { get; set; }
```

This tells EF Core that Student entities should be included in the database model and provides a way to query and modify student records.

Example:

```csharp
var students = await _context.Students.ToListAsync();
```

This retrieves student records from the database.

In the current implementation, explicit business entity DbSets have not yet been added because the Student, College, Company and other business models will be introduced in later modules.

The IdentityDbContext already provides the Identity-related sets required for user and role management.

### 7.4 Responsibilities of DbContext

The DbContext performs several important tasks:

1. Maintains the connection between the application and the database.
2. Tracks changes made to entity objects.
3. Provides LINQ-based querying.
4. Generates SQL commands for data modifications.
5. Manages entity relationships.
6. Coordinates saving changes to the database.
7. Provides the model information used by EF Core migrations.

### 7.5 Change Tracking

EF Core uses change tracking to identify modifications made to entities.

For example:

```csharp
var user = await _context.Users
    .FirstOrDefaultAsync(u => u.Id == userId);

if (user != null)
{
    user.FullName = "Updated User";

    await _context.SaveChangesAsync();
}
```

Explanation:

1. EF Core retrieves the user from the database.
2. The DbContext begins tracking the retrieved entity.
3. The FullName property is modified.
4. EF Core detects the change.
5. `SaveChangesAsync()` generates and executes the required SQL UPDATE statement.

This reduces the need to manually write SQL UPDATE statements.

## 8. Dependency Injection Configuration

Dependency Injection (DI) is a software design pattern in which required dependencies are provided to a class rather than created directly inside it.

ASP.NET Core has a built-in dependency injection container.

The project uses dependency injection to register database and Identity services.

Implementation in `Program.cs`:

```csharp
var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
```

### 8.1 WebApplication.CreateBuilder

```csharp
var builder = WebApplication.CreateBuilder(args);
```

This creates the application builder.

It initializes the application's configuration, logging and service registration infrastructure.

The `builder.Services` property provides access to the dependency injection service collection.

### 8.2 AddDbContext

```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
```

`AddDbContext` registers ApplicationDbContext with the dependency injection container.

`UseSqlServer()` configures SQL Server as the database provider.

When a controller or service requests ApplicationDbContext through its constructor, ASP.NET Core can provide an appropriately configured instance.

By default, AddDbContext registers the context with a scoped lifetime.

A scoped service is generally created once per HTTP request and reused within that request's dependency injection scope.

This is appropriate for DbContext because it is designed to represent a short-lived unit of work.

A DbContext should not be used concurrently by multiple threads.

## 9. ASP.NET Core Identity Configuration

Identity is responsible for managing user accounts and role-related information.

The project registers Identity using the following configuration:

```csharp
builder.Services
    .AddIdentityCore<ApplicationUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager();
```

### 9.1 AddIdentityCore<ApplicationUser>()

Registers the core Identity services for the custom ApplicationUser type.

It provides services for user management, including creating users, finding users and managing user information.

It also configures the Identity user model used throughout the application.

### 9.2 AddRoles<IdentityRole>()

Enables role management.

The Campus Placement Management Platform has four main roles:

- SuperAdmin
- PlacementOfficer
- CompanyHR
- Student

Roles are used to group users according to their responsibilities.

For example, a Student should not have access to administrative functions reserved for a SuperAdmin.

The role system will be used with authorization policies in a later module.

### 9.3 AddEntityFrameworkStores<ApplicationDbContext>()

Connects ASP.NET Core Identity to Entity Framework Core.

This tells Identity to use ApplicationDbContext for storing and retrieving user, role and related authentication data.

Without this configuration, Identity would not have the configured EF Core persistence store for its database operations.

### 9.4 AddSignInManager()

Registers SignInManager services.

SignInManager provides functionality for handling sign-in-related operations.

It works with UserManager and the configured authentication system to support operations such as password-based sign-in.

JWT token generation and validation will be configured separately in the Authentication module.

## 10. Middleware Configuration

The application uses middleware to process incoming HTTP requests.

The relevant part of Program.cs is:

```csharp
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
```

Explanation:

**builder.Build()**

Builds the configured application and creates the WebApplication instance.

**MapOpenApi()**

Exposes the OpenAPI document in the development environment.

This helps developers inspect the API structure.

**UseHttpsRedirection()**

Redirects HTTP requests to HTTPS when the application has an appropriate HTTPS endpoint configured.

**UseAuthentication()**

Runs the authentication middleware.

It attempts to identify the user associated with the incoming request based on the configured authentication scheme.

The JWT Bearer scheme will be configured in the Authentication module.

**UseAuthorization()**

Runs authorization checks to determine whether the authenticated user is permitted to access the requested resource.

Authentication and authorization are different:

- Authentication answers: Who are you?
- Authorization answers: What are you allowed to access?

Authentication middleware must run before authorization middleware.

**MapControllers()**

Maps controller endpoints so that incoming HTTP requests can reach their corresponding controller actions.

**Run()**

Starts the application and begins listening for incoming requests.

## 11. Entity Framework Core Migrations

A migration is a mechanism used by Entity Framework Core to manage changes in the database schema.

Whenever the application's data model changes, migrations help update the database structure to match the current model.

Instead of manually creating database tables and columns, EF Core can generate migration instructions based on the model.

### 11.1 Installing EF Core CLI tools

The .NET EF Core command-line tool is used to manage migrations.

The tool can be installed using:

```powershell
dotnet tool install --global dotnet-ef --version 10.0.14
```

The version should be compatible with the project's EF Core packages.

### 11.2 Restoring packages

```powershell
dotnet restore
```

This downloads and restores the NuGet packages required by the project.

### 11.3 Building the project

```powershell
dotnet build
```

This compiles the application and identifies compilation errors.

A successful build confirms that the project compiles, but does not by itself prove that the database connection works.

### 11.4 Creating the initial migration

```powershell
dotnet ef migrations add InitialIdentitySetup
```

This command creates a migration named `InitialIdentitySetup`.

EF Core examines the application's current model and generates migration code representing the required database schema changes.

The migration typically contains:

- Up() – instructions to apply the schema changes.
- Down() – instructions to reverse those changes.

The generated migration files are stored in the Migrations folder.

### 11.5 Applying the migration

```powershell
dotnet ef database update
```

This command applies pending migrations to the configured SQL Server database.

EF Core executes the SQL operations represented by the migration.

It also maintains a migration history table named `__EFMigrationsHistory`.

This table records which migrations have been applied to the database.

### 11.6 Difference between Add Migration and Database Update

| Command | Purpose |
|---|---|
| migrations add | Generates migration files based on the current EF Core model |
| database update | Applies pending migration changes to the database |

Creating a migration does not automatically update the database.

The database update command is required to apply the migration.

## 12. Database Tables Created by Identity

After applying the initial Identity migration, the database contains Identity-related tables.

Some of the standard tables include:

| Table | Purpose |
|---|---|
| AspNetUsers | Stores user account details |
| AspNetRoles | Stores role information |
| AspNetUserRoles | Maps users to roles |
| AspNetUserClaims | Stores claims associated with users |
| AspNetRoleClaims | Stores claims associated with roles |
| AspNetUserLogins | Stores external login provider information |
| AspNetUserTokens | Stores user authentication tokens |
| __EFMigrationsHistory | Tracks applied EF Core migrations |

### Understanding the Identity relationships

**AspNetUsers and AspNetRoles**

A user can be assigned one or more roles, depending on the application's role assignment rules.

**AspNetUserRoles**

This is a junction table that connects users and roles.

It contains references to both the user and the role.

**AspNetUserClaims**

Stores claims associated with individual users.

A claim is a key-value pair representing information about a user.

**AspNetRoleClaims**

Stores claims associated with a role.

**AspNetUserLogins**

Stores information about external login providers, if external authentication is configured.

**AspNetUserTokens**

Stores authentication-related tokens associated with a user.

The exact set of columns and indexes is determined by the Identity model and its EF Core configuration.

## 13. Complete Database Request Flow

The following sequence explains how the application components work together.

1. A client sends an HTTP request to the Web API.
2. ASP.NET Core receives the request.
3. The request reaches the appropriate controller.
4. The controller or service receives ApplicationDbContext through dependency injection.
5. The DbContext uses its configured SQL Server provider.
6. EF Core translates LINQ expressions into SQL commands.
7. SQL Server executes the generated commands.
8. The database returns the requested information.
9. EF Core maps database records into C# objects.
10. The application processes the results and returns an HTTP response.

Simplified architecture:

Client → Controller → Service → ApplicationDbContext → EF Core → SQL Server

For Identity operations:

Controller → UserManager / SignInManager → Identity EF Store → ApplicationDbContext → SQL Server

## 14. Testing and Verification

The database configuration was verified through the following operations:

1. Restored the required NuGet packages.
2. Built the application successfully.
3. Configured the SQL Server connection string.
4. Registered ApplicationDbContext and Identity services.
5. Generated the initial Identity migration.
6. Applied the migration to SQL Server.
7. Verified that the CampusPlacementDB database was created.
8. Verified the presence of Identity-related tables.

The expected outcome is a functioning database with the standard ASP.NET Core Identity schema.

## 15. Challenges Faced and Solutions

### Challenge 1: Entity Framework Core could not identify the DbContext

Initially, the migration command displayed the error:

`No DbContext was found.`

**Reason:**

The ApplicationDbContext class was initially created as an empty class and did not inherit from DbContext or IdentityDbContext.

**Solution:**

The class was updated to inherit from:

```csharp
IdentityDbContext<ApplicationUser>
```

The required constructor was also added to receive DbContextOptions through dependency injection.

This allowed EF Core to recognize the class as a valid database context.

### Challenge 2: Understanding the relationship between Identity and EF Core

Initially, it was necessary to understand how ASP.NET Core Identity stores user and role information in SQL Server.

**Solution:**

ApplicationDbContext was configured using IdentityDbContext<ApplicationUser>, and the Identity EF Core store was registered using AddEntityFrameworkStores<ApplicationDbContext>().

This connected Identity's user management functionality to the SQL Server database.

### Challenge 3: Database configuration

The application required a valid SQL Server connection string.

**Solution:**

The connection string was configured in appsettings.json and retrieved using the configuration system in Program.cs.

### Challenge 4: Understanding migration execution

Creating a migration and applying it to the database are two separate operations.

**Solution:**

The migration was generated using `dotnet ef migrations add`, and the database schema was updated using `dotnet ef database update`.

This helped establish the difference between generating schema changes and applying them.

## 16. Learning Outcomes

After completing this module, the following concepts were understood:

**1. ORM Architecture**

Understood how Entity Framework Core maps C# classes and properties to relational database tables and columns.

**2. DbContext and DbSet**

Understood the role of DbContext in managing database sessions and how DbSet represents a collection of entities.

**3. ASP.NET Core Identity Integration**

Understood how Identity manages users and roles and how IdentityDbContext connects it with Entity Framework Core.

**4. Dependency Injection**

Understood how ASP.NET Core registers and provides services such as ApplicationDbContext to other application components.

**5. Database Migrations**

Learned how migrations are generated and applied to maintain database schema changes.

**6. Database Connectivity**

Understood how connection strings and SQL Server providers establish communication between an ASP.NET Core application and SQL Server.

## 17. Final Outcome

Module 2 successfully established the database infrastructure for the Campus Placement Management Platform.

The ASP.NET Core Web API was connected to Microsoft SQL Server through Entity Framework Core. ASP.NET Core Identity was integrated to support user and role management, and the initial migration was successfully applied.

The resulting database provides the foundation for implementing the platform's business entities and workflows in subsequent modules.

The database currently contains the Identity schema. The College, PlacementOfficer, Company, CompanyHR, Student and other business tables will be introduced in the upcoming entity implementation modules.

## 18. Git Commit Message

```bash
git add .
git commit -m "feat: configure EF Core database and ASP.NET Core Identity"
```

**Module Status:** Completed

**Next Module:** Core Business Entities – College, PlacementOfficer, Company, CompanyHR and Student.