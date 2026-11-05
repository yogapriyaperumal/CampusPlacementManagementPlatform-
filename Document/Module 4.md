# Technical Documentation – Module 4: Placement Officer Management

## 1. Introduction

The Placement Officer Management module manages placement officers associated with registered colleges in the Campus Placement Management Platform.

A placement officer is responsible for representing their institution and will eventually manage student enrollment and placement activities.

Each placement officer must have an existing ASP.NET Core Identity user account and must be associated with an approved college. Newly registered placement officers are assigned Pending status until the SuperAdmin reviews their registration.

## 2. Objectives

- Register placement officers.
- Associate placement officers with existing Identity user accounts.
- Associate officers with approved colleges.
- Prevent duplicate placement officer profiles for the same user.
- Retrieve and update placement officer information.
- Implement officer approval and rejection operations.
- Maintain approval details and timestamps.

## 3. Technologies Used

| Technology | Purpose |
|---|---|
| ASP.NET Core Web API (.NET 10) | API development |
| Entity Framework Core 10 | Database operations |
| SQL Server | Data storage |
| ASP.NET Core Identity | User management |
| C# | Backend development |
| LINQ | Data querying |
| Postman | API testing |

## 4. Entity Design

### 4.1 PlacementOfficer Model

The PlacementOfficer entity stores the profile and institutional association of each placement officer.

| Property | Data Type | Description |
|---|---|---|
| PlacementOfficerId | int | Primary key |
| UserId | string | Foreign key referencing ApplicationUser |
| User | ApplicationUser | Navigation property |
| CollegeId | int | Foreign key referencing College |
| College | College | Navigation property |
| Designation | string? | Officer's designation |
| Status | OfficerStatus | Registration and approval status |
| ApprovedBy | string? | Identity user ID of approver |
| ApprovedAt | DateTime? | Approval timestamp |

### 4.2 OfficerStatus Enum

The OfficerStatus enum defines the possible states of a placement officer:

- Pending
- Approved
- Rejected
- Suspended

New placement officers are assigned Pending status by default.

## 5. Database Relationships

The following relationships were configured using Entity Framework Core Fluent API.

### ApplicationUser and PlacementOfficer

One Identity user can have at most one placement officer profile.

A unique index on UserId prevents duplicate officer profiles.

### College and PlacementOfficer

One college can have multiple placement officers, while each placement officer belongs to one college.

### ApplicationUser and ApprovedBy

ApprovedBy references the Identity user who approved the placement officer. This is an optional relationship because a pending or rejected officer does not have an approval record.

Delete behavior was configured using Restrict to prevent accidental deletion of referenced users or colleges.

## 6. Data Transfer Objects

DTOs were introduced to separate API request and response structures from database entities.

### 6.1 PlacementOfficerCreateRequest

Receives the registration details:

- UserId
- CollegeId
- Designation

Approval-related properties are not accepted from the registration request.

### 6.2 PlacementOfficerUpdateRequest

Allows the designation to be updated without changing the Identity user, college association, or approval status.

### 6.3 PlacementOfficerResponse

Returns:

- PlacementOfficerId
- UserId
- CollegeId
- CollegeName
- Designation
- Status
- ApprovedBy
- ApprovedAt

The CollegeName is obtained through the College navigation property.

## 7. Repository Layer

The repository layer handles database operations and isolates data-access logic from business logic.

### IPlacementOfficerRepository

The interface defines:

- CreateAsync()
- GetAllAsync()
- GetByIdAsync()
- GetByUserIdAsync()
- UpdateAsync()

### PlacementOfficerRepository

The implementation uses ApplicationDbContext and Entity Framework Core.

Important methods:

- AddAsync() inserts a new placement officer.
- SaveChangesAsync() persists changes.
- Include() loads the related College information.
- AsNoTracking() improves read-only query efficiency.
- FirstOrDefaultAsync() retrieves a matching record.
- Update() marks an entity as modified.

The GetByUserIdAsync() method is used to check for an existing placement officer profile.

## 8. Service Layer

The service layer implements the business rules of the module.

### Registration

1. Receives the registration request.
2. Checks whether the Identity user exists using UserManager.
3. Checks whether the user already has a placement officer profile.
4. Checks whether the requested college exists.
5. Verifies that the college has Approved status.
6. Creates a placement officer with Pending status.
7. Saves the record through the repository.
8. Converts the saved entity into a response DTO.

### Retrieval

The service retrieves all placement officers or a specific officer and maps the entities to response DTOs.

### Update

The service checks whether the officer exists and updates the permitted designation field.

### Approval

The service:

1. Finds the placement officer.
2. Verifies that the officer is Pending.
3. Verifies that the approving Identity user exists.
4. Changes the status to Approved.
5. Stores the approver's user ID.
6. Records the UTC approval timestamp.

### Rejection

The service verifies that the officer is Pending and changes the status to Rejected.

## 9. Controller Layer

The PlacementOfficerController exposes REST API endpoints.

Base route: /api/placementofficer

| HTTP Method | Endpoint | Description |
|---|---|---|
| POST | /api/placementofficer | Register placement officer |
| GET | /api/placementofficer | Retrieve all officers |
| GET | /api/placementofficer/{id} | Retrieve officer by ID |
| PUT | /api/placementofficer/{id} | Update officer designation |
| PUT | /api/placementofficer/{id}/approve | Approve officer |
| PUT | /api/placementofficer/{id}/reject | Reject officer |

The controller calls the service layer and returns appropriate HTTP responses.

## 10. Dependency Injection

The repository and service were registered in Program.cs using scoped dependency injection.

- IPlacementOfficerRepository → PlacementOfficerRepository
- IPlacementOfficerService → PlacementOfficerService

Scoped lifetime is appropriate because these components depend on the scoped Entity Framework Core DbContext.

## 11. Database Migration

The following Entity Framework Core commands were used:

```powershell
dotnet build
dotnet ef migrations add AddPlacementOfficerManagement
dotnet ef database update
```

The migration creates the PlacementOfficers table with the configured primary key, foreign keys, unique UserId index, and other columns.

## 12. API Testing Using Postman

### 12.1 Register Placement Officer

Method: POST

Endpoint: /api/placementofficer

Sample request:

```json
{
  "userId": "YOUR_IDENTITY_USER_ID",
  "collegeId": 1,
  "designation": "Placement Officer"
}
```

Expected result: 201 Created.

The response should contain Pending status.

### 12.2 Retrieve All Placement Officers

Method: GET

Endpoint: /api/placementofficer

Expected result: 200 OK.

### 12.3 Retrieve Placement Officer by ID

Method: GET

Endpoint: /api/placementofficer/1

Expected result: 200 OK for an existing record or 404 Not Found.

### 12.4 Update Placement Officer

Method: PUT

Endpoint: /api/placementofficer/1

Sample request:

```json
{
  "designation": "Senior Placement Coordinator"
}
```

Expected result: 200 OK.

### 12.5 Approve Placement Officer

Method: PUT

Endpoint: /api/placementofficer/1/approve?approvedBy=YOUR_SUPERADMIN_USER_ID

Expected result: 200 OK.

The officer status should change to Approved, and the approver ID and approval timestamp should be stored.

### 12.6 Reject Placement Officer

Method: PUT

Endpoint: /api/placementofficer/1/reject

Expected result: 200 OK.

The officer status should change to Rejected.

### 12.7 Additional Validation Tests

- Nonexistent Identity user should be rejected.
- Duplicate officer registration should be rejected.
- Nonexistent college should be rejected.
- A college that is not Approved should not be allowed to register officers.
- An already processed officer should not be approved or rejected again.

**Screenshot placeholders:** Insert actual Postman request and response screenshots for each test after successful execution.

## 13. Challenges Faced

### 13.1 Managing Identity Relationships

The placement officer profile needed to reference an existing Identity user whose primary key is a string rather than an integer.

**Solution:** Configured UserId as a foreign key to ApplicationUser.

### 13.2 Preventing Duplicate Officer Profiles

The same Identity user must not be registered multiple times as a placement officer.

**Solution:** Implemented duplicate checking in the service and a unique database index on UserId.

### 13.3 Validating College Approval

Placement officers should only be associated with colleges that have been approved.

**Solution:** Added a college existence check and verified the CollegeStatus before creating an officer record.

### 13.4 Implementing Approval Workflow

Officer registration and officer approval are separate operations.

**Solution:** Introduced OfficerStatus and service methods for approval and rejection.

### 13.5 Managing Related Entity Data

The API response needs the college name, not just the CollegeId.

**Solution:** Used EF Core Include() to load the related College entity.

## 14. Security Considerations and Future Improvements

The current implementation includes the approval business logic, but the approval endpoints still require proper authorization.

The current controller accepts the approver ID through a query parameter for development testing. This must be replaced with the authenticated user's ID from JWT claims.

The following improvements are planned:

- Restrict approval and rejection to SuperAdmin.
- Associate registration with the authenticated user's Identity account.
- Prevent unapproved officers from accessing placement officer features.
- Implement resource-based authorization for college-specific data.
- Add centralized exception handling and audit logging.
- Record rejection details and approval history if required.

## 15. Learning Outcomes

Through this module, the following concepts were implemented and understood:

- One-to-one and one-to-many entity relationships.
- Foreign key configuration using Fluent API.
- Unique indexes.
- ASP.NET Core Identity UserManager.
- Repository and Service patterns.
- Asynchronous database operations.
- Entity-to-DTO mapping.
- Business validation.
- Approval and rejection workflows.
- Dependency injection.
- REST API development and Postman testing.

## 16. Conclusion

The Placement Officer Management module establishes the relationship between Identity users and colleges within the Campus Placement Management Platform.

It supports officer registration, retrieval, profile updates, and approval or rejection operations. The module also validates user existence, college approval, and duplicate registrations.

This module provides the foundation for the upcoming Student Management module, where approved placement officers will be associated with student enrollment and management activities.