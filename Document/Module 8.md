# Technical Documentation – Module 8: Job Opening Management

## 1. Introduction

The Job Opening Management module allows approved CompanyHR users to create and manage job opportunities within the Campus Placement Management Platform.

Companies can define job descriptions, employment types, salary ranges, academic eligibility requirements, application deadlines, and target colleges.

The module also supports job publication and closure workflows.

## 2. Objectives

- Allow approved CompanyHR users to create job openings.
- Associate job openings with their respective companies.
- Define academic eligibility requirements.
- Target multiple approved colleges for a single job.
- Support job editing while in Draft status.
- Implement job publishing and closing.
- Display published opportunities to targeted colleges.

## 3. Technologies Used

- ASP.NET Core Web API – .NET 10
- Entity Framework Core
- SQL Server
- Repository Pattern
- Service Layer Architecture
- DTO-based API communication

## 4. Entities

### JobOpening

Stores job information, eligibility requirements, ownership, deadlines, and publication status.

Important properties include:

- JobOpeningId
- JobTitle
- JobDescription
- Location
- EmploymentType
- SalaryMin and SalaryMax
- MinimumCGPA
- MaximumBacklogs
- EligibleGraduationYear
- EligibleDepartments
- ApplicationDeadline
- Status
- CompanyId
- CreatedByCompanyHRId
- CreatedAt
- PublishedAt
- ClosedAt

### JobTargetCollege

Acts as a junction entity between JobOpening and College.

It allows one job opening to target multiple colleges and one college to receive multiple job openings.

## 5. Database Relationships

- Company to JobOpening: One-to-Many.
- CompanyHR to JobOpening: One-to-Many.
- JobOpening to JobTargetCollege: One-to-Many.
- College to JobTargetCollege: One-to-Many.

A unique composite index on JobOpeningId and CollegeId prevents the same college from being added twice to one job opening.

## 6. Job Status Management

The JobStatus enum contains:

- Draft
- Published
- Closed
- Cancelled

New jobs are created as Draft.

Only Draft jobs can be edited and published. Published jobs can be closed. Closed and Cancelled jobs cannot be published again.

## 7. Employment Type

The EmploymentType enum contains:

- FullTime
- Internship
- Contract

This categorizes the type of opportunity offered by the company.

## 8. Eligibility Criteria

Each job contains academic eligibility requirements:

- Minimum CGPA
- Maximum active backlogs
- Eligible graduation year
- Eligible departments

EligibleDepartments is stored as a comma-separated string in this implementation.

These values will be used by the future Job Application module to determine whether a student meets the job requirements.

## 9. DTO Implementation

The module uses:

- JobCreateRequest – accepts new job details and target college IDs.
- JobUpdateRequest – accepts changes to Draft job details.
- JobResponse – returns job information, company name, and target college information.

The backend assigns CompanyId and CreatedByCompanyHRId after validating the HR user. The client cannot select another company when creating a job.

## 10. Repository Layer

The JobRepository handles database operations:

- Create a job opening.
- Retrieve all job openings.
- Retrieve a job by ID.
- Retrieve jobs created by a particular CompanyHR.
- Retrieve published jobs targeted to a college.
- Update a job opening.

Entity Framework Core Include and ThenInclude are used to retrieve related Company, CompanyHR, and College information.

## 11. Service Layer

The JobService contains business rules.

Before creating a job, it verifies:

1. The CompanyHR exists.
2. The CompanyHR is approved.
3. The associated company is approved.
4. All target colleges exist.
5. All target colleges are approved.
6. The application deadline is in the future.
7. Salary and academic eligibility values are valid.

The service also validates ownership before allowing a CompanyHR user to update, publish, or close a job.

## 12. Controller Layer

The JobController exposes REST API endpoints.

| Method | Endpoint | Purpose |
|---|---|---|
| POST | /api/jobs/create/{companyHRId} | Create a job |
| GET | /api/jobs | Retrieve jobs |
| GET | /api/jobs/{id} | Retrieve a job |
| GET | /api/jobs/companyhr/{companyHRId} | Retrieve jobs created by an HR |
| GET | /api/jobs/college/{collegeId} | Retrieve published jobs for a college |
| PUT | /api/jobs/{id}/companyhr/{companyHRId} | Update a Draft job |
| PUT | /api/jobs/{id}/publish/{companyHRId} | Publish a job |
| PUT | /api/jobs/{id}/close/{companyHRId} | Close a published job |

## 13. Job Publication Workflow

1. An approved CompanyHR creates a job.
2. The job is saved with Draft status.
3. The HR user defines academic eligibility criteria and target colleges.
4. The HR user publishes the job.
5. Targeted colleges can retrieve the published job.
6. The HR user can close the published job.

## 14. Dependency Injection

The following dependencies are registered:

- IJobRepository → JobRepository
- IJobService → JobService

The service uses existing CompanyHR and College repositories to validate related records.

## 15. Database Migration

The database migration is created using:

    dotnet ef migrations add AddJobOpeningManagement
    dotnet ef database update

This creates JobOpenings and JobTargetColleges tables with the required relationships and constraints.

## 16. Challenges Faced

### A. Managing many-to-many relationships

One job can target multiple colleges, and each college can receive multiple job openings.

**Solution:** Implemented a separate JobTargetCollege junction entity with a composite unique index.

### B. Validating CompanyHR ownership

A company can have multiple HR users, and one HR user should not modify another HR user's job.

**Solution:** Stored CreatedByCompanyHRId and validated ownership in the service layer.

### C. Implementing job publication rules

Draft, Published, and Closed jobs require different operations.

**Solution:** Used a JobStatus enum and implemented separate service methods for updating, publishing, and closing jobs.

### D. Validating eligibility requirements

Invalid salary ranges, CGPA limits, deadlines, or target colleges can affect recruitment.

**Solution:** Added service-level validation before saving and publishing job openings.

### E. Managing related entity information

Job responses need company names and target college details in addition to their foreign keys.

**Solution:** Used EF Core navigation properties and Include/ThenInclude to retrieve related records.

## 17. Security Considerations and Future Improvements

- Enforce JWT authentication and CompanyHR role authorization.
- Derive the HR identity from authenticated claims rather than accepting HR IDs from URLs.
- Restrict HR users to jobs belonging to their approved company.
- Restrict Draft job visibility to authorized users.
- Add pagination, filtering, and search.
- Add support for richer eligibility criteria.
- Add job approval if required by the institution's recruitment workflow.
- Record changes in an audit history.
- Prevent applications after the deadline in the future Job Application module.

## 18. Learning Outcomes

Through this module, I learned:

- Implementing many-to-many relationships using junction entities.
- Configuring EF Core relationships and composite unique indexes.
- Connecting job openings with companies and HR users.
- Implementing ownership validation.
- Designing eligibility criteria.
- Managing job publication states.
- Retrieving related data using Include and ThenInclude.
- Implementing business rules in the service layer.

## 19. Conclusion

The Job Opening Management module provides a structured way for approved companies and HR users to manage recruitment opportunities.

It supports job creation, eligibility configuration, college targeting, publication, and closure.

This module establishes the foundation for the next stage of the platform: managing student applications and recruitment activities.