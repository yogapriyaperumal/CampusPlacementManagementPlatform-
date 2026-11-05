# Technical Documentation – Module 3: College Management

## 1. Introduction

The College Management module is responsible for managing college information within the Campus Placement Management Platform.

Colleges can register themselves by providing their institutional details. Every newly registered college is assigned a **Pending** status by default. The SuperAdmin approval workflow will be implemented in a later module.

This module provides APIs to register colleges, retrieve college information, and update college details.

## 2. Objectives

- Implement college registration.
- Store college information in SQL Server using Entity Framework Core.
- Ensure each college has a unique CollegeCode.
- Validate mandatory college details.
- Provide APIs to retrieve all colleges and individual college details.
- Allow existing college information to be updated.
- Maintain the college registration status and creation timestamp.

## 3. Technologies Used

| Technology | Purpose |
|---|---|
| ASP.NET Core Web API (.NET 10) | API development |
| Entity Framework Core 10 | ORM and database operations |
| Microsoft SQL Server | Database |
| C# | Backend programming |
| Data Annotations | Input validation |
| LINQ | Query operations |
| Postman | API testing |

## 4. College Entity Design

The College entity represents an educational institution registered on the platform.

**Model:** `College.cs`

The model contains the following properties:

| Property | Data Type | Description |
|---|---|---|
| CollegeId | int | Primary key |
| CollegeName | string | Name of the college |
| CollegeCode | string | Unique institutional code |
| UniversityName | string? | Affiliated university |
| OfficialEmail | string | Official college email |
| PhoneNumber | string | College contact number |
| Address | string | College address |
| Website | string? | Official website |
| Status | CollegeStatus | Registration approval status |
| CreatedAt | DateTime | College registration timestamp |

### Validation

Data Annotations are used to validate the college details.

- `[Required]` ensures mandatory fields are provided.
- `[MaxLength]` restricts the maximum length of input values.
- `[EmailAddress]` validates the official email format.

The `CollegeStatus` enum defines the following states:

- Pending
- Approved
- Rejected
- Suspended
- Inactive

Every newly registered college receives the Pending status by default.

## 5. Database Configuration

### ApplicationDbContext Integration

The College entity was integrated into the existing `ApplicationDbContext`, which inherits from `IdentityDbContext<ApplicationUser>`.

The following DbSet was added:

`public DbSet<College> Colleges { get; set; }`

This enables Entity Framework Core to perform database operations on the Colleges table.

### Fluent API Configuration

The `OnModelCreating()` method was overridden to configure the College entity.

The following configurations were applied:

1. **Primary Key:** CollegeId was configured as the primary key.
2. **Unique Index:** CollegeCode was configured with a unique index to prevent duplicate college codes.
3. **Required Fields:** CollegeName, CollegeCode, OfficialEmail, PhoneNumber, and Address were configured as mandatory fields.
4. **Maximum Length:** Appropriate maximum lengths were configured for the database columns.
5. **Identity Configuration:** The base `OnModelCreating()` method was called to preserve the existing ASP.NET Core Identity configuration.

## 6. Data Transfer Objects (DTOs)

DTOs were introduced to separate the API request and response structures from the database entity.

### 6.1 CollegeCreateRequest

Used when registering a new college.

It accepts:
- CollegeName
- CollegeCode
- UniversityName
- OfficialEmail
- PhoneNumber
- Address
- Website

Validation attributes ensure that the required information is provided correctly.

### 6.2 CollegeUpdateRequest

Used to update existing college information.

It allows changes to:
- CollegeName
- UniversityName
- OfficialEmail
- PhoneNumber
- Address
- Website

CollegeCode is not included because it acts as the unique institutional identifier. Registration status is also not modified through this endpoint.

### 6.3 CollegeResponse

Used to return college information to API consumers.

It contains the college ID, name, code, university, contact details, address, website, status, and creation timestamp.

Using response DTOs prevents unnecessary exposure of the internal database entity.

## 7. Repository Layer

The repository layer handles database interactions and separates data-access logic from business logic.

### ICollegeRepository

The repository interface defines the following operations:

- `CreateAsync()` – Adds a new college.
- `GetAllAsync()` – Retrieves all registered colleges.
- `GetByIdAsync()` – Retrieves a college using its ID.
- `GetByCodeAsync()` – Checks whether a college code already exists.
- `UpdateAsync()` – Updates an existing college.

### CollegeRepository

The implementation uses `ApplicationDbContext` and Entity Framework Core methods.

Important methods include:

- `AddAsync()` to add a new college.
- `SaveChangesAsync()` to persist database changes.
- `AsNoTracking()` for read-only queries.
- `FirstOrDefaultAsync()` to retrieve a matching college.
- `ToListAsync()` to retrieve the college collection.
- `Update()` to mark an existing college as modified.

Asynchronous database operations improve the application's ability to handle concurrent API requests without blocking request-processing threads during database I/O.

## 8. Service Layer

The service layer contains the business logic of the College Management module.

### ICollegeService

Defines the operations exposed to the controller:

- Create a college.
- Retrieve all colleges.
- Retrieve a college by ID.
- Update college information.

### CollegeService

The service implementation performs the following operations:

**College Registration:**

1. Receives the college registration DTO.
2. Normalizes the CollegeCode by removing surrounding spaces and converting it to uppercase.
3. Checks whether the CollegeCode already exists.
4. Creates a College entity.
5. Assigns Pending as the initial status.
6. Sets the CreatedAt timestamp using UTC time.
7. Calls the repository to save the college.
8. Maps the saved entity to CollegeResponse.

**College Retrieval:**

The service retrieves college records through the repository and converts the entities into response DTOs.

**College Update:**

The service checks whether the requested college exists. If found, it updates the permitted fields and saves the changes through the repository.

The service does not allow the college registration status or CollegeCode to be changed through the general update operation.

## 9. Controller Layer

The `CollegeController` exposes REST API endpoints for college operations.

Base route:

`/api/college`

### API Endpoints

| HTTP Method | Endpoint | Description |
|---|---|---|
| POST | `/api/college` | Register a new college |
| GET | `/api/college` | Retrieve all colleges |
| GET | `/api/college/{id}` | Retrieve college by ID |
| PUT | `/api/college/{id}` | Update college information |

### Controller Responsibilities

- Accept HTTP requests.
- Receive and validate DTOs.
- Call the appropriate service methods.
- Return suitable HTTP status codes.
- Handle expected business validation errors.

### HTTP Status Codes

| Status Code | Meaning |
|---|---|
| 200 OK | Data retrieved or updated successfully |
| 201 Created | College registered successfully |
| 400 Bad Request | Invalid request data |
| 404 Not Found | College does not exist |
| 409 Conflict | Duplicate CollegeCode |

The POST endpoint uses `CreatedAtAction()` to return a 201 response with a location pointing to the newly created college resource.

## 10. Dependency Injection

The repository and service implementations were registered in `Program.cs` using scoped dependency injection.

- `ICollegeRepository` is mapped to `CollegeRepository`.
- `ICollegeService` is mapped to `CollegeService`.

Scoped lifetime creates one instance per HTTP request scope. This is appropriate for services and repositories that use the scoped Entity Framework Core DbContext.

The controller receives its service dependency through constructor injection.

## 11. Database Migration

Entity Framework Core migrations were used to apply the College entity configuration to SQL Server.

The following commands were executed:

```powershell
dotnet build

dotnet ef migrations add AddCollegeManagement

dotnet ef database update
```

### Migration Process

1. The build command verifies that the project compiles successfully.
2. The migration command generates a migration based on the changes in the EF Core model.
3. The database update command applies the migration to SQL Server.
4. The Colleges table is created with its configured columns, primary key, and unique CollegeCode index.

## 12. API Testing Using Postman

The College Management APIs were tested using Postman.

### 12.1 Register College

**Method:** POST

**Endpoint:** `/api/college`

Sample request:

```json
{
  "collegeName": "ABC Engineering College",
  "collegeCode": "ABC001",
  "universityName": "Anna University",
  "officialEmail": "contact@abc.edu",
  "phoneNumber": "9876543210",
  "address": "Dindigul, Tamil Nadu",
  "website": "https://www.abc.edu"
}
```

Expected result: HTTP 201 Created.

The response contains the registered college details with Pending status.

**Screenshot Placeholder:** College registration request and successful response.

### 12.2 Retrieve All Colleges

**Method:** GET

**Endpoint:** `/api/college`

Expected result: HTTP 200 OK with the list of registered colleges.

**Screenshot Placeholder:** Retrieve all colleges response.

### 12.3 Retrieve College by ID

**Method:** GET

**Endpoint:** `/api/college/1`

Expected result: HTTP 200 OK if the college exists, otherwise 404 Not Found.

**Screenshot Placeholder:** Retrieve college by ID response.

### 12.4 Update College

**Method:** PUT

**Endpoint:** `/api/college/1`

Sample request:

```json
{
  "collegeName": "ABC Institute of Technology",
  "universityName": "Anna University",
  "officialEmail": "admin@abc.edu",
  "phoneNumber": "9876543210",
  "address": "Updated address, Dindigul",
  "website": "https://www.abc.edu"
}
```

Expected result: HTTP 200 OK with updated college details.

**Screenshot Placeholder:** College update request and response.

### 12.5 Duplicate College Code Validation

A registration request was submitted using an already existing CollegeCode.

Expected result: HTTP 409 Conflict.

This verifies that duplicate college codes are rejected by the business logic.

**Screenshot Placeholder:** Duplicate CollegeCode error response.

### 12.6 Invalid Email Validation

A registration request was submitted with an invalid email address.

Expected result: HTTP 400 Bad Request.

This verifies the DTO validation mechanism.

**Screenshot Placeholder:** Invalid email validation response.

## 13. Challenges Faced

### 13.1 Maintaining Unique College Codes

A college code must uniquely identify an institution. Duplicate codes could create incorrect college associations.

**Solution:** Implemented duplicate checking in the service layer and a unique database index using Fluent API.

### 13.2 Separating Entity and API Models

Directly exposing database entities through API endpoints can make the application difficult to maintain and can expose unnecessary fields.

**Solution:** Created separate request and response DTOs for college operations.

### 13.3 Implementing Layered Architecture

Placing database queries and business logic directly inside controllers would tightly couple the application components.

**Solution:** Separated responsibilities into Controller, Service, and Repository layers using interfaces and dependency injection.

### 13.4 Configuring Entity Framework Core

The College entity needed to be integrated into the existing Identity-enabled DbContext without affecting Identity's database configuration.

**Solution:** Added the College DbSet and configured the entity inside `OnModelCreating()` while calling the base implementation.

### 13.5 Handling Registration Status

Newly registered colleges should not immediately become active participants in the placement platform.

**Solution:** Assigned Pending as the default registration status. The approval workflow will be handled by the SuperAdmin in a later module.

## 14. Security and Future Enhancements

The current module provides the college