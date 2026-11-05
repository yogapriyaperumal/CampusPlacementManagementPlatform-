# Technical Documentation – Module 7: Student Management

## 1. Introduction

The Student Management module manages student enrollment and academic information within the Campus Placement Management Platform.

Students are associated with an approved college and managed by an approved Placement Officer. Student enrollment requires approval before the student can participate in placement activities.

The module also supports bulk student enrollment through Excel file uploads.

## 2. Objectives

- Create and maintain student profiles.
- Connect student profiles with ASP.NET Core Identity accounts.
- Associate students with their respective colleges.
- Track the Placement Officer responsible for each student.
- Prevent duplicate student accounts and register numbers.
- Implement student approval and rejection.
- Support bulk enrollment through Excel.

## 3. Technologies Used

- ASP.NET Core Web API – .NET 10
- Entity Framework Core
- SQL Server
- ASP.NET Core Identity
- ClosedXML – Excel file processing
- Repository Pattern
- Service Layer Architecture
- DTO-based API communication

## 4. Student Entity

The Student entity contains:

| Field | Purpose |
|---|---|
| StudentId | Primary key |
| UserId | Identity account reference |
| CollegeId | Associated college |
| PlacementOfficerId | Responsible placement officer |
| RegisterNumber | College registration number |
| Department | Academic department |
| Degree | Degree qualification |
| GraduationYear | Expected graduation year |
| CGPA | Academic performance |
| ActiveBacklogs | Current backlog count |
| ResumeUrl | Optional resume link |
| Status | Enrollment status |
| CreatedAt | Enrollment creation time |
| ApprovedAt | Approval timestamp |

## 5. Database Relationships

- ApplicationUser to Student: One-to-One.
- College to Student: One-to-Many.
- PlacementOfficer to Student: One-to-Many.

Unique constraints are configured for UserId and the combination of CollegeId and RegisterNumber.

These constraints prevent duplicate student profiles and duplicate registration numbers within a college.

## 6. Student Status Management

The StudentStatus enum contains:

- Pending
- Approved
- Rejected
- Suspended

Every newly enrolled student receives Pending status by default.

Approval changes the status to Approved and records the approval timestamp. Rejection changes the status to Rejected.

## 7. DTO Implementation

Separate DTOs are used for:

- StudentCreateRequest – accepts enrollment details.
- StudentUpdateRequest – accepts editable academic details.
- StudentResponse – returns student and college information.
- StudentImportResult – returns Excel import statistics and errors.

The client cannot directly choose the student's college, placement officer, or enrollment status during enrollment. These values are controlled by the backend.

## 8. Repository Layer

The StudentRepository communicates with ApplicationDbContext.

It provides methods to:

- Create student records.
- Retrieve all students.
- Retrieve a student by ID.
- Find a student by Identity UserId.
- Check register-number uniqueness within a college.
- Retrieve students managed by an officer.
- Update student information.

Entity Framework Core is used for database operations.

## 9. Service Layer

The StudentService implements the business rules.

Before enrollment, it verifies:

1. The Placement Officer exists.
2. The officer is approved.
3. The officer's college is approved.
4. The Identity user exists.
5. The user does not already have a student profile.
6. The register number is not already used in that college.

It also ensures that an officer can only update, approve, or reject students assigned to that officer.

## 10. Controller Layer

The StudentController exposes REST API endpoints for student operations.

| Method | Endpoint | Purpose |
|---|---|---|
| POST | /api/students/enroll/{officerId} | Enroll a student |
| GET | /api/students | Retrieve all students |
| GET | /api/students/{id} | Retrieve a student |
| GET | /api/students/officer/{officerId} | Retrieve an officer's students |
| PUT | /api/students/{id}/officer/{officerId} | Update student details |
| PUT | /api/students/{id}/approve/{officerId} | Approve enrollment |
| PUT | /api/students/{id}/reject/{officerId} | Reject enrollment |
| POST | /api/students/bulk-import/{officerId} | Import students using Excel |

## 11. Bulk Excel Import

ClosedXML is used to read `.xlsx` files.

The uploaded file contains student Identity IDs, register numbers, departments, degrees, graduation years, CGPAs, backlog counts, and optional resume URLs.

The service processes each row, validates the information, checks for existing records, and creates valid student profiles.

Newly imported students receive Pending status.

The response reports total records, successfully imported records, failed records, and row-specific errors.

The current implementation saves each valid row individually, so an import can partially succeed.

## 12. Dependency Injection

The following dependencies are registered:

- IStudentRepository → StudentRepository
- IStudentService → StudentService

The service also uses the existing PlacementOfficer repository, College repository, and ASP.NET Core Identity UserManager.

## 13. Database Migration

The migration is created using:

    dotnet ef migrations add AddStudentManagement
    dotnet ef database update

This creates the Students table and its associated foreign keys and unique indexes.

## 14. Challenges Faced

### A. Managing multiple relationships

The Student entity references Identity users, colleges, and placement officers.

**Solution:** Configured the relationships explicitly using Entity Framework Core Fluent API.

### B. Preventing duplicate student records

A user must not have multiple student profiles, and register numbers must be unique within a college.

**Solution:** Implemented service-level checks and database unique indexes.

### C. Validating Placement Officer access

An officer should not manage students from another college or another officer's assigned records.

**Solution:** Validated the officer's approval status, college approval status, and student-to-officer association.

### D. Implementing bulk Excel import

Excel rows may contain invalid values, missing information, or duplicate records.

**Solution:** Used ClosedXML and added row-level validation with an import result containing error details.

### E. Maintaining enrollment status

Newly created student records should not automatically become approved.

**Solution:** Set Pending as the default status and implemented separate approval and rejection operations.

## 15. Security Considerations and Future Improvements

- Protect student management endpoints with JWT authentication and role-based authorization.
- Derive the officer identity from authenticated claims instead of accepting officer IDs from public request URLs.
- Ensure only Placement Officers can enroll and approve students.
- Restrict officers to their assigned college.
- Add pagination and search for large student datasets.
- Add stronger file size and upload validation.
- Consider database transactions and bulk operations for large Excel imports.
- Add an approval history and audit trail.

## 16. Learning Outcomes

Through this module, I learned:

- Implementing one-to-one and one-to-many relationships using EF Core.
- Connecting Identity users with application-specific profiles.
- Implementing composite unique indexes.
- Applying service-layer business validation.
- Handling approval and rejection workflows.
- Processing Excel files using ClosedXML.
- Returning row-level validation errors.
- Building REST APIs using layered architecture.

## 17. Conclusion

The Student Management module provides a structured way to enroll and manage students within the Campus Placement Management Platform.

It connects students with their colleges and placement officers, validates academic information, supports approval workflows, and enables bulk enrollment through Excel.

This module establishes the student data foundation required for future Job Opening, Job Application, and Recruitment Drive modules.