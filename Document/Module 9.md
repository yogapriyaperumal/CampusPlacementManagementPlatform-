# Technical Documentation – Module 9: Job Application Management

## 1. Introduction

The Job Application Management module manages the process through which approved students apply for published job openings.

It validates student eligibility before accepting applications and allows the responsible CompanyHR user to review applications and update their statuses.

The module also maintains an application status history to preserve every status change.

## 2. Objectives

- Allow approved students to apply for published jobs.
- Validate academic eligibility requirements.
- Prevent duplicate applications.
- Allow students to view their submitted applications.
- Allow CompanyHR users to review and update applications.
- Allow students to withdraw eligible applications.
- Maintain a complete application status history.

## 3. Technologies Used

- ASP.NET Core Web API – .NET 10
- Entity Framework Core
- SQL Server
- Repository Pattern
- Service Layer Architecture
- DTO-based API communication
- ASP.NET Core Identity

## 4. Entities

### JobApplication

Stores each student's application for a job.

Important properties:

- JobApplicationId
- StudentId
- JobOpeningId
- CoverLetter
- Status
- AppliedAt
- LastUpdatedAt

### ApplicationStatusHistory

Maintains the history of status changes.

Important properties:

- ApplicationStatusHistoryId
- JobApplicationId
- PreviousStatus
- NewStatus
- Remarks
- UpdatedByUserId
- ChangedAt

## 5. Database Relationships

- Student to JobApplication: One-to-Many.
- JobOpening to JobApplication: One-to-Many.
- JobApplication to ApplicationStatusHistory: One-to-Many.

A unique composite index on StudentId and JobOpeningId prevents a student from submitting multiple applications for the same job.

## 6. Application Status Management

The ApplicationStatus enum contains:

- Applied
- UnderReview
- Shortlisted
- Rejected
- Selected
- Withdrawn

New applications are created with Applied status.

The service validates allowed status transitions before saving changes.

## 7. Eligibility Validation

Before accepting an application, the service checks:

1. The student exists.
2. The student is approved.
3. The student's college is approved.
4. The job exists.
5. The job is Published.
6. The application deadline has not passed.
7. The student's college is targeted by the job.
8. The student's CGPA meets the minimum requirement.
9. The student's active backlogs do not exceed the allowed limit.
10. The student's graduation year matches the job requirement.
11. The student's department is eligible.

Applications that fail these validations are rejected.

## 8. Duplicate Application Prevention

The service checks whether an application already exists for the same StudentId and JobOpeningId.

The database also enforces a unique composite index on these columns.

This provides protection at both the service and database levels.

## 9. DTO Implementation

The module uses:

- JobApplicationCreateRequest – accepts student ID, job ID, and cover letter.
- ApplicationStatusUpdateRequest – accepts the new status and remarks.
- JobApplicationResponse – returns application, student, and job details.
- ApplicationStatusHistoryResponse – returns the history of status changes.

DTOs separate API request and response structures from database entities.

## 10. Repository Layer

The JobApplicationRepository handles database operations:

- Create applications.
- Retrieve an application by ID.
- Check existing applications.
- Retrieve all applications.
- Retrieve applications by student.
- Retrieve applications by job.
- Update application information and status.
- Retrieve status history.
- Add history records.

Entity Framework Core navigation properties are used to retrieve related student and job information.

## 11. Service Layer

The JobApplicationService implements the business logic.

It handles:

- Student and job validation.
- Academic eligibility checking.
- Duplicate application prevention.
- Application creation.
- Application withdrawal.
- CompanyHR ownership validation.
- Application status transitions.
- Status history creation.

The application and its initial history record are saved together. Later status changes also update the application and add a history record in the same database save operation.

## 12. Controller Layer

The JobApplicationController exposes REST API endpoints.

| Method | Endpoint | Purpose |
|---|---|---|
| POST | /api/jobapplications/apply | Apply for a job |
| GET | /api/jobapplications | Retrieve all applications |
| GET | /api/jobapplications/{id} | Retrieve an application |
| GET | /api/jobapplications/student/{studentId} | Retrieve student applications |
| GET | /api/jobapplications/job/{jobOpeningId} | Retrieve applications for a job |
| PUT | /api/jobapplications/{id}/status/companyhr/{companyHRId} | Update application status |
| PUT | /api/jobapplications/{id}/withdraw/{studentId} | Withdraw an application |
| GET | /api/jobapplications/{id}/history | Retrieve status history |

## 13. Application Status History

Every application begins with an Applied history event.

Whenever the application status changes, the system records:

- Previous status
- New status
- Remarks
- User who performed the action, when supplied
- Date and time of the change

This makes it possible to track the complete recruitment progress of an application.

## 14. Dependency Injection

The following dependencies are registered:

- IJobApplicationRepository → JobApplicationRepository
- IJobApplicationService → JobApplicationService

The service also uses existing Student, Job, and CompanyHR repositories.

## 15. Database Migration

The migration is created using:

    dotnet ef migrations add AddJobApplicationManagement
    dotnet ef database update

This creates JobApplications and ApplicationStatusHistories tables with the required relationships and unique constraints.

## 16. Challenges Faced

### A. Validating student eligibility

A student should not be able to apply for a job without satisfying the company's academic requirements.

**Solution:** Implemented service-level checks for CGPA, backlogs, graduation year, department, college targeting, and approval status.

### B. Preventing duplicate applications

A student must not submit multiple applications for the same job.

**Solution:** Added service-level duplicate checks and a unique composite database index.

### C. Managing application status transitions

Application statuses must follow a valid recruitment workflow.

**Solution:** Implemented an enum and service-level transition validation.

### D. Maintaining application history

Updating the current status alone would lose information about earlier recruitment decisions.

**Solution:** Created a separate ApplicationStatusHistory entity to preserve every status change.

### E. Validating CompanyHR ownership

An HR user should only manage applications for jobs they created.

**Solution:** Compared the job's CreatedByCompanyHRId with the requesting CompanyHR ID before permitting status changes.

## 17. Security Considerations and Future Improvements

- Enforce JWT authentication and role-based authorization.
- Derive the student identity from authenticated claims rather than accepting StudentId from the request.
- Derive the CompanyHR identity from JWT claims rather than accepting HR IDs from URLs.
- Derive UpdatedByUserId from the authenticated user rather than accepting it as a query parameter.
- Restrict application information to authorized students, CompanyHR users, and placement officers.
- Add pagination and filtering for application lists.
- Add email or in-app notifications for status changes.
- Consider transaction and concurrency handling for simultaneous applications.
- Add a more detailed audit trail.

## 18. Learning Outcomes

Through this module, I learned:

- Implementing many-to-many relationships using application entities.
- Applying eligibility rules in a service layer.
- Preventing duplicate database records.
- Implementing status transition workflows.
- Maintaining audit and status history.
- Managing ownership and access validation.
- Using EF Core navigation properties.
- Building REST APIs for recruitment workflows.

## 19. Conclusion

The Job Application Management module connects approved students with published job opportunities.

It validates academic eligibility, prevents duplicate applications, supports application withdrawal, and allows CompanyHR users to manage recruitment decisions.

The application status history provides transparency and traceability throughout the recruitment process.

This module establishes the foundation for recruitment drives, interview rounds, and interview scheduling.