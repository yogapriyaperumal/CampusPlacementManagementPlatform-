# Technical Documentation – Module 10: Recruitment Drive Management

## 1. Introduction

The Recruitment Drive Management module manages recruitment events organized by approved companies through their approved CompanyHR users.

It connects the Company, CompanyHR, and JobOpening modules. It allows an authorized company representative to create a recruitment drive, specify its schedule and mode, and update its progress.

## 2. Objectives

- Create recruitment drives for published job openings.
- Associate each drive with a company and CompanyHR.
- Maintain drive dates, descriptions, mode, and venue or meeting link.
- Track recruitment drive status.
- Prevent CompanyHR users from creating drives for another company's job openings.
- Retrieve drives by ID and company.

## 3. Technologies Used

- ASP.NET Core Web API – REST API development.
- Entity Framework Core – database operations.
- SQL Server – persistent data storage.
- C# – application logic.
- Postman – API testing.

## 4. Database Design

### RecruitmentDrive

| Column | Description |
|---|---|
| RecruitmentDriveId | Primary key |
| DriveTitle | Recruitment drive name |
| Description | Drive details |
| CompanyId | Foreign key to Company |
| CompanyHRId | Foreign key to CompanyHR |
| JobOpeningId | Foreign key to JobOpening |
| StartDate | Drive start date and time |
| EndDate | Drive end date and time |
| Mode | Online or Offline |
| VenueOrMeetingLink | Meeting link or physical venue |
| Status | Current recruitment drive status |
| CreatedAt | Record creation timestamp |

### Relationships

- Company → RecruitmentDrive: One-to-Many.
- CompanyHR → RecruitmentDrive: One-to-Many.
- JobOpening → RecruitmentDrive: One-to-Many.

Each recruitment drive references one company, one CompanyHR, and one job opening.

## 5. Recruitment Drive Status

The RecruitmentDriveStatus enum contains:

- Planned
- Open
- InProgress
- Completed
- Cancelled

The allowed progression is:

Planned → Open → InProgress → Completed

A drive can also be cancelled before completion.

Completed and Cancelled are terminal states in this implementation.

## 6. Application Architecture

The module follows the existing layered architecture:

Controller → Service → Repository → ApplicationDbContext → SQL Server

### Controller

Receives HTTP requests and returns appropriate HTTP responses.

### Service

Contains validation and business rules, including company ownership checks, approval checks, date validation, and status transitions.

### Repository

Handles database operations through Entity Framework Core.

### DTOs

Separate incoming request data from outgoing response data and prevent direct exposure of database entities.

## 7. API Endpoints

| Method | Endpoint | Purpose |
|---|---|---|
| POST | /api/recruitmentdrives | Create a drive |
| GET | /api/recruitmentdrives | Retrieve all drives |
| GET | /api/recruitmentdrives/{id} | Retrieve a drive |
| GET | /api/recruitmentdrives/company/{companyId} | Retrieve company drives |
| PUT | /api/recruitmentdrives/{id} | Update drive details |
| PUT | /api/recruitmentdrives/{id}/status | Update drive status |

## 8. Business Validations

1. Only approved CompanyHR users can create drives.
2. The associated company must be approved.
3. The selected job opening must exist and be published.
4. The job opening must belong to the CompanyHR's company.
5. The end date must be later than the start date.
6. The start date cannot be in the past during creation.
7. Online drives require a meeting link.
8. Offline drives require a venue.
9. Completed and cancelled drives cannot be edited.
10. Invalid status transitions are rejected.

## 9. Database Migration

Migration name: AddRecruitmentDriveManagement

Commands:

- dotnet build
- dotnet ef migrations add AddRecruitmentDriveManagement
- dotnet ef database update

## 10. Testing

The module should be tested through Postman.

Test scenarios include:

- Creating an online recruitment drive.
- Creating an offline recruitment drive.
- Retrieving all drives.
- Retrieving a drive by ID.
- Retrieving drives by company.
- Updating drive details.
- Updating recruitment drive status.
- Rejecting unapproved CompanyHR users.
- Rejecting job openings owned by another company.
- Rejecting invalid dates and status transitions.

## 11. Challenges Faced

### Company and job ownership validation

A CompanyHR must not create a recruitment drive for another company's job opening. This required comparing the CompanyHR's CompanyId with the JobOpening's CompanyId.

### Managing entity relationships

The recruitment drive depends on three existing entities: Company, CompanyHR, and JobOpening. Correct foreign key configuration is necessary to maintain database consistency.

### Status transition validation

Allowing arbitrary status changes could create an inconsistent recruitment workflow. A defined status progression was implemented in the service layer.

### Date and mode validation

The application validates the start and end dates and ensures that online drives have a meeting link and offline drives have a venue.

## 12. Security Considerations and Future Improvements

The current implementation accepts CompanyHRId from the request and does not yet establish the creator's identity from the authenticated user.

For production:

- Require CompanyHR authentication and authorization.
- Derive the CompanyHR identity from JWT claims instead of trusting a request-supplied ID.
- Restrict drive creation and updates to the owning CompanyHR or authorized administrators.
- Restrict company-based retrieval to authorized users.
- Add audit history for drive changes.
- Add pagination and filtering for large drive lists.

## 13. Learning Outcomes

- Implemented a recruitment drive entity with multiple foreign key relationships.
- Practiced Entity Framework Core relationship configuration.
- Applied layered architecture using repositories and services.
- Implemented business validation across existing modules.
- Learned to enforce ownership checks between related entities.
- Implemented controlled status transitions.
- Tested REST API endpoints and negative scenarios using Postman.

## 14. Conclusion

The Recruitment Drive Management module connects company job openings with organized recruitment events. It provides a structured way to create, retrieve, update, and track recruitment drives while enforcing company ownership and approval rules.

This module prepares the platform for the next stage: managing interview rounds and interview schedules for candidates participating in recruitment drives.