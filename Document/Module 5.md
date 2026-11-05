# Technical Documentation – Module 5: Company Management

## 1. Introduction

The Company Management module manages organizations that participate in campus recruitment through the Campus Placement Management Platform.

Companies can register their organization details, including company name, email, website, industry, address, and description.

Every newly registered company is assigned Pending status. The SuperAdmin approval workflow will be implemented as part of the platform's administrative functionality.

## 2. Objectives

- Register companies on the platform.
- Store company information in SQL Server.
- Retrieve all registered companies.
- Retrieve an individual company by ID.
- Update company information.
- Maintain company registration status.
- Record the company creation timestamp.

## 3. Technologies Used

| Technology | Purpose |
|---|---|
| ASP.NET Core Web API (.NET 10) | API development |
| Entity Framework Core 10 | ORM and database operations |
| Microsoft SQL Server | Data storage |
| C# | Backend programming |
| Data Annotations | Request validation |
| LINQ | Data querying |
| Postman | API testing |

## 4. Company Entity Design

The Company entity represents an organization registered on the placement platform.

| Property | Data Type | Description |
|---|---|---|
| CompanyId | int | Primary key |
| CompanyName | string | Organization name |
| CompanyEmail | string | Official company email |
| Website | string? | Company website |
| Industry | string | Industry sector |
| Address | string | Company address |
| Description | string? | Organization description |
| Status | CompanyStatus | Registration status |
| CreatedAt | DateTime | Registration timestamp |

### CompanyStatus Enum

The CompanyStatus enum defines:

- Pending
- Approved
- Rejected
- Suspended
- Inactive

New companies receive Pending status by default.

## 5. Database Configuration

The Company entity was integrated into ApplicationDbContext through the Companies DbSet.

Fluent API was used to configure:

- CompanyId as the primary key.
- Required CompanyName, CompanyEmail, Industry, and Address properties.
- Maximum lengths for company information.
- Optional Website and Description fields.

The Company entity does not yet have relationships with other entities in this module. CompanyHR and JobOpening will reference the Company entity in upcoming modules.

## 6. Data Transfer Objects

Three DTOs were implemented.

### CompanyCreateRequest

Receives company registration information, including company name, email, website, industry, address, and description.

### CompanyUpdateRequest

Receives updated company profile information. It does not allow the client to change the company ID, registration status, or creation timestamp.

### CompanyResponse

Returns the company ID, name, email, website, industry, address, description, status, and creation timestamp.

DTOs separate API data structures from the database entity and prevent unnecessary exposure of internal properties.

## 7. Repository Layer

The repository layer manages database operations.

### ICompanyRepository

Defines:

- CreateAsync()
- GetAllAsync()
- GetByIdAsync()
- UpdateAsync()

### CompanyRepository

Uses ApplicationDbContext and Entity Framework Core.

Important methods include:

- AddAsync() to insert company information.
- SaveChangesAsync() to persist database changes.
- AsNoTracking() for read-only retrieval.
- OrderBy() to sort companies by name.
- FirstOrDefaultAsync() to retrieve a company by ID.
- Update() to mark an entity as modified.

## 8. Service Layer

The service layer contains company-related business logic.

### Registration

1. Receives CompanyCreateRequest.
2. Trims surrounding whitespace from input values.
3. Creates a Company entity.
4. Assigns Pending status.
5. Records the UTC creation timestamp.
6. Saves the entity through the repository.
7. Maps the saved entity to CompanyResponse.

### Retrieval

The service retrieves all companies or a specific company through the repository and maps the results into response DTOs.

### Update

The service checks whether the company exists. If found, it updates the permitted profile fields and saves the changes.

The company registration status is not changed through the general update operation.

## 9. Controller Layer

The CompanyController exposes REST APIs.

Base route: /api/company

| HTTP Method | Endpoint | Description |
|---|---|---|
| POST | /api/company | Register a company |
| GET | /api/company | Retrieve all companies |
| GET | /api/company/{id} | Retrieve company by ID |
| PUT | /api/company/{id} | Update company information |

The controller communicates with the service layer and returns appropriate HTTP status codes.

## 10. Dependency Injection

The following dependencies were registered using scoped lifetime:

- ICompanyRepository → CompanyRepository
- ICompanyService → CompanyService

Constructor injection is used to provide dependencies to the service and controller.

## 11. Database Migration

The following commands were used to generate and apply the Company Management migration:

```powershell
dotnet build
dotnet ef migrations add AddCompanyManagement
dotnet ef database update
```

The migration creates the Companies table in SQL Server based on the configured entity.

## 12. API Testing Using Postman

### Register Company

Method: POST

Endpoint: /api/company

Expected result: 201 Created.

### Retrieve All Companies

Method: GET

Endpoint: /api/company

Expected result: 200 OK.

### Retrieve Company by ID

Method: GET

Endpoint: /api/company/1

Expected result: 200 OK or 404 Not Found.

### Update Company

Method: PUT

Endpoint: /api/company/1

Expected result: 200 OK.

### Validation Testing

Invalid or missing required fields should result in 400 Bad Request. Requests for nonexistent company IDs should return 404 Not Found.

Actual Postman screenshots should be inserted into this section after testing.

## 13. Challenges Faced

### 13.1 Designing Company Information

The company entity needed to capture sufficient organization information without mixing recruitment-specific information into the company profile.

Solution: Created a separate Company entity containing organization details. Job-related information will be managed by the JobOpening module.

### 13.2 Separating Request and Response Models

Directly exposing the entity through API endpoints could make the application difficult to maintain.

Solution: Created separate create, update, and response DTOs.

### 13.3 Maintaining Registration Status

Newly registered companies should not immediately be permitted to participate in recruitment activities.

Solution: Introduced CompanyStatus and assigned Pending status during registration.

### 13.4 Implementing Layered Architecture

Database operations and business logic should not be placed directly inside controllers.

Solution: Separated responsibilities across controller, service, and repository layers using interfaces and dependency injection.

## 14. Security and Future Enhancements

The current module provides company registration, retrieval, and profile updates.

The following features are planned for later modules:

- SuperAdmin company approval and rejection.
- Company suspension and reactivation.
- CompanyHR registration and approval.
- JWT authentication and role-based authorization.
- Restricting company profile updates to authorized users.
- Preventing unapproved companies from publishing job openings.
- Centralized exception handling and audit logging.

## 15. Learning Outcomes

- Creating C# entity models.
- Defining enum-based status values.
- Configuring EF Core entities.
- Using Data Annotations.
- Creating request and response DTOs.
- Implementing repository and service patterns.
- Performing asynchronous database operations.
- Implementing RESTful CRUD endpoints.
- Using dependency injection.
- Applying database migrations.
- Testing APIs using Postman.

## 16. Conclusion

The Company Management module provides the foundation for managing recruiting organizations in the Campus Placement Management Platform.

It supports company registration, information retrieval, and profile updates while maintaining a Pending registration status for newly created companies.

The module will be extended through CompanyHR Management and JobOpening Management, enabling organizations to participate in the platform's recruitment workflow.