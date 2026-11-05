# Technical Documentation – Module 6: CompanyHR Management

## 1. Introduction

The CompanyHR Management module manages Human Resource representatives associated with companies registered on the Campus Placement Management Platform.

CompanyHR representatives are responsible for handling recruitment activities on behalf of their organizations. Each HR representative must have an existing ASP.NET Core Identity user account and must be associated with a registered company.

New HR registrations receive Pending status and require SuperAdmin approval before receiving authorized recruitment access.

## 2. Objectives

- Register HR representatives.
- Associate HR profiles with Identity user accounts.
- Associate HR representatives with companies.
- Ensure that only approved companies can register HR representatives.
- Prevent duplicate HR profiles for the same Identity user.
- Retrieve and update HR information.
- Implement approval and rejection operations.
- Record approval details and timestamps.

## 3. Technologies Used

| Technology | Purpose |
|---|---|
| ASP.NET Core Web API (.NET 10) | Backend API development |
| Entity Framework Core 10 | ORM and database operations |
| Microsoft SQL Server | Data storage |
| ASP.NET Core Identity | User management |
| C# | Backend programming |
| LINQ | Database querying |
| Postman | API testing |

## 4. Entity Design

### 4.1 CompanyHR Model

The CompanyHR entity represents an HR representative registered under a company.

| Property | Data Type | Description |
|---|---|---|
| CompanyHRId | int | Primary key |
| UserId | string | Identity user foreign key |
| User | ApplicationUser | Navigation property |
| CompanyId | int | Company foreign key |
| Company | Company | Navigation property |
| Designation | string? | HR designation |
| Status | CompanyHRStatus | Registration and approval status |
| ApprovedBy | string? | Identity user ID of approver |
| ApprovedAt | DateTime? | Approval timestamp |

### 4.2 CompanyHRStatus Enum

The CompanyHRStatus enum defines the following states:

- Pending
- Approved
- Rejected
- Suspended

Every newly registered HR representative receives Pending status by default.

## 5. Database Configuration and Relationships

The CompanyHR entity was integrated into the existing ApplicationDbContext through the CompanyHRs DbSet.

Entity Framework Core Fluent API was used to configure the relationships.

### ApplicationUser and CompanyHR

One Identity user can have at most one CompanyHR profile.

A unique index on UserId prevents duplicate HR profiles.

### Company and CompanyHR

One company can have multiple HR representatives, while each HR representative belongs to one company.

### ApplicationUser and ApprovedBy

ApprovedBy stores the Identity user ID of the person who approves the HR registration. It is an optional relationship because pending and rejected records do not require an approval record.

Delete behavior was configured using Restrict to prevent accidental deletion of referenced records.

## 6. Data Transfer Objects

Three DTOs were implemented.

### 6.1 CompanyHRCreateRequest

Receives:
- UserId
- CompanyId
- Designation

Approval-related fields are excluded from the registration request.

### 6.2 CompanyHRUpdateRequest

Allows the HR designation to be updated without modifying the Identity user, company association, or approval status.

### 6.3 CompanyHRResponse

Returns:
- CompanyHRId
- UserId
- CompanyId
- CompanyName
- Designation
- Status
- ApprovedBy
- ApprovedAt

DTOs separate external API contracts from internal database entities.

## 7. Repository Layer

The repository layer handles database access.

### ICompanyHRRepository

Defines the following operations:

- CreateAsync()
- GetAllAsync()
- GetByIdAsync()
- GetByUserIdAsync()
- UpdateAsync()

### CompanyHRRepository

The implementation uses ApplicationDbContext and Entity Framework Core.

Important methods include:

- AddAsync() to create an HR record.
- SaveChangesAsync() to persist changes.
- Include() to load related company information.
- AsNoTracking() for read-only queries.
- FirstOrDefaultAsync() to retrieve a record.
- Update() to mark an entity as modified.

GetByUserIdAsync() is used to prevent duplicate HR registrations.

## 8. Service Layer

The service layer contains the business rules.

### 8.1 Registration

1. Receives CompanyHRCreateRequest.
2. Checks whether the Identity user exists using UserManager.
3. Checks whether the user already has a CompanyHR profile.
4. Checks whether the requested company exists.
5. Verifies that the company is Approved.
6. Creates a CompanyHR entity.
7. Assigns Pending status.
8. Saves the entity using the repository.
9. Converts the saved entity into CompanyHRResponse.

### 8.2 Retrieval

The service retrieves all HR records or an individual record through the repository and maps the results into response DTOs.

### 8.3 Update

The service verifies that the requested HR profile exists and updates its designation.

### 8.4 Approval

The approval process:

1. Retrieves the HR record.
2. Verifies that the record is Pending.
3. Checks whether the approving Identity user exists.
4. Changes the status to Approved.
5. Stores the approver's user ID.
6. Records the UTC approval timestamp.

### 8.5 Rejection

The service checks whether the HR record is Pending and changes its status to Rejected.

## 9. Controller Layer

The CompanyHRController exposes REST API endpoints.

Base route: /api/companyhr

| HTTP Method | Endpoint | Description |
|---|---|---|
| POST | /api/companyhr | Register CompanyHR |
| GET | /api/companyhr | Retrieve all HR records |
| GET | /api/companyhr/{id} | Retrieve HR by ID |
| PUT | /api/companyhr/{id} | Update HR designation |
| PUT | /api/companyhr/{id}/approve | Approve HR registration |
| PUT | /api/companyhr/{id}/reject | Reject HR registration |

The controller receives requests, calls the service layer, and returns suitable HTTP status codes.

## 10. Dependency Injection

The following services were registered using scoped dependency injection:

- ICompanyHRRepository → CompanyHRRepository
- ICompanyHRService → CompanyHRService

Constructor injection is used to provide the repository and service dependencies.

## 11. Database Migration

The following commands were used to create and apply the migration:

```powershell
dotnet build
dotnet ef migrations add AddCompanyHRManagement
dotnet ef database update
```

The migration creates the CompanyHRs table with its primary key, foreign keys, unique UserId index, and configured columns.

## 12. API Testing Using Postman

The module APIs were designed to be tested using Postman.

### Register CompanyHR

Method: POST

Endpoint: /api/companyhr

Expected result: 201 Created.

The response should contain Pending status.

### Retrieve All CompanyHR Records

Method: GET

Endpoint: /api/companyhr

Expected result: 200 OK.

### Retrieve CompanyHR by ID

Method: GET

Endpoint: /api/companyhr/1

Expected result: 200 OK or 404 Not Found.

### Update CompanyHR

Method: PUT

Endpoint: /api/companyhr/1

Expected result: 200 OK.

### Approve CompanyHR

Method: PUT

Endpoint: /api/companyhr/1/approve?approvedBy=YOUR_SUPERADMIN_USER_ID

Expected result: 200 OK.

The status should change to Approved, and approval details should be recorded.

### Reject CompanyHR

Method: PUT

Endpoint: /api/companyhr/2/reject

Expected result: 200 OK with Rejected status.

### Additional Validation Tests

- Nonexistent Identity user should be rejected.
- Duplicate HR registration should be rejected.
- Nonexistent company should be rejected.
- A company that is not Approved should not be allowed to register HR representatives.
- Already processed HR registrations should not be approved or rejected again.

Actual Postman screenshots should be inserted after successfully executing the tests.

## 13. Challenges Faced

### 13.1 Integrating ASP.NET Core Identity

The HR profile needed to reference an existing Identity user whose primary key is a string.

Solution: Configured UserId as a foreign key to ApplicationUser.

### 13.2 Preventing Duplicate HR Registration

The same Identity user should not be registered multiple times as an HR representative.

Solution: Implemented duplicate checking in the service and a unique index on UserId.

### 13.3 Validating Company Approval

Only approved companies should be permitted to register HR representatives.

Solution: Added company existence and approval-status checks before creating an HR profile.

### 13.4 Managing Approval Information

HR registration and HR approval are separate operations.

Solution: Introduced CompanyHRStatus and implemented approval and rejection methods in the service layer.

### 13.5 Retrieving Related Company Information

The API response requires the company name along with the CompanyId.

Solution: Used Entity Framework Core Include() to retrieve the related Company entity.

## 14. Security Considerations and Future Improvements

The current implementation includes registration and approval business logic. However, the approval endpoints require proper authorization before production use.

The following improvements are planned:

- Restrict approval and rejection to SuperAdmin.
- Obtain the approver ID from authenticated JWT claims.
- Associate registration with the authenticated user's Identity account.
- Prevent unapproved HR representatives from accessing recruitment features.
- Restrict HR access to their own company's data.
- Implement resource-based authorization.
- Add centralized exception handling and audit logging.

## 15. Learning Outcomes

Through this module, the following concepts were implemented and understood:

- One-to-one and one-to-many entity relationships.
- Foreign key configuration using Fluent API.
- Unique indexes.
- ASP.NET Core Identity UserManager.
- Repository and Service patterns.
- DTO design and entity mapping.
- Business validation.
- Approval and rejection workflows.
- Dependency injection.
- Asynchronous database operations.
- REST API implementation.
- EF Core migrations and Postman testing.

## 16. Conclusion

The CompanyHR Management module establishes the relationship between Identity users and recruiting companies.

It supports HR registration, profile retrieval, designation updates, and approval or rejection operations. It also ensures that HR registrations are associated with valid Identity users and approved companies.

This module provides the foundation for Job Opening Management, where approved company HR representatives will create and manage recruitment opportunities.