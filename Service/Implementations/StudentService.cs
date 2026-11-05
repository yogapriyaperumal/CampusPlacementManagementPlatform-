using CampusPlacementManagementPlatform.DTO.Student;
using CampusPlacementManagementPlatform.Models;
using CampusPlacementManagementPlatform.Respositories.Interfaces;
using CampusPlacementManagementPlatform.Service.Interfaces;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace CampusPlacementManagementPlatform.Service.Implementations
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IPlacementOfficerRepository _officerRepository;
        private readonly ICollegeRepository _collegeRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public StudentService(
            IStudentRepository studentRepository,
            IPlacementOfficerRepository officerRepository,
            ICollegeRepository collegeRepository,
            UserManager<ApplicationUser> userManager)
        {
            _studentRepository = studentRepository;
            _officerRepository = officerRepository;
            _collegeRepository = collegeRepository;
            _userManager = userManager;
        }

        private async Task<PlacementOfficer> ValidateOfficerAsync(
            int officerId)
        {
            var officer = await _officerRepository.GetByIdAsync(officerId);

            if (officer == null)
                throw new InvalidOperationException(
                    "Placement officer not found.");

            if (officer.Status != OfficerStatus.Approved)
                throw new InvalidOperationException(
                    "Placement officer is not approved.");

            var college = await _collegeRepository.GetByIdAsync(
                officer.CollegeId);

            if (college == null ||
                college.Status != CollegeStatus.Approved)
            {
                throw new InvalidOperationException(
                    "Officer's college is not approved.");
            }

            return officer;
        }

        private static StudentResponse MapToResponse(Student student)
        {
            return new StudentResponse
            {
                StudentId = student.StudentId,
                UserId = student.UserId,
                CollegeId = student.CollegeId,
                CollegeName = student.College?.CollegeName ?? string.Empty,
                PlacementOfficerId = student.PlacementOfficerId,
                RegisterNumber = student.RegisterNumber,
                Department = student.Department,
                Degree = student.Degree,
                GraduationYear = student.GraduationYear,
                CGPA = student.CGPA,
                ActiveBacklogs = student.ActiveBacklogs,
                ResumeUrl = student.ResumeUrl,
                Status = student.Status.ToString(),
                CreatedAt = student.CreatedAt,
                ApprovedAt = student.ApprovedAt
            };
        }

        public async Task<StudentResponse> CreateAsync(
            int officerId,
            StudentCreateRequest request)
        {
            var officer = await ValidateOfficerAsync(officerId);

            var user = await _userManager.FindByIdAsync(request.UserId);

            if (user == null)
                throw new InvalidOperationException(
                    "Identity user not found.");

            var existingStudent =
                await _studentRepository.GetByUserIdAsync(request.UserId);

            if (existingStudent != null)
                throw new InvalidOperationException(
                    "This user already has a student profile.");

            var existingRegisterNumber =
                await _studentRepository
                    .GetByCollegeAndRegisterNumberAsync(
                        officer.CollegeId,
                        request.RegisterNumber.Trim());

            if (existingRegisterNumber != null)
                throw new InvalidOperationException(
                    "Register number already exists in this college.");

            var student = new Student
            {
                UserId = request.UserId,
                CollegeId = officer.CollegeId,
                PlacementOfficerId = officerId,
                RegisterNumber = request.RegisterNumber.Trim(),
                Department = request.Department.Trim(),
                Degree = request.Degree.Trim(),
                GraduationYear = request.GraduationYear,
                CGPA = request.CGPA,
                ActiveBacklogs = request.ActiveBacklogs,
                ResumeUrl = request.ResumeUrl,
                Status = StudentStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _studentRepository.CreateAsync(student);

            return MapToResponse(created);
        }

        public async Task<List<StudentResponse>> GetAllAsync()
        {
            var students = await _studentRepository.GetAllAsync();

            return students.Select(MapToResponse).ToList();
        }

        public async Task<StudentResponse?> GetByIdAsync(int id)
        {
            var student = await _studentRepository.GetByIdAsync(id);

            return student == null ? null : MapToResponse(student);
        }

        public async Task<List<StudentResponse>> GetByOfficerIdAsync(
            int officerId)
        {
            await ValidateOfficerAsync(officerId);

            var students =
                await _studentRepository.GetByOfficerIdAsync(officerId);

            return students.Select(MapToResponse).ToList();
        }

        public async Task<StudentResponse> UpdateAsync(
            int id,
            int officerId,
            StudentUpdateRequest request)
        {
            await ValidateOfficerAsync(officerId);

            var student = await _studentRepository.GetByIdAsync(id);

            if (student == null)
                throw new KeyNotFoundException("Student not found.");

            if (student.PlacementOfficerId != officerId)
                throw new InvalidOperationException(
                    "You cannot update a student managed by another officer.");

            if (student.Status == StudentStatus.Suspended)
                throw new InvalidOperationException(
                    "Suspended student records cannot be updated.");

            student.Department = request.Department.Trim();
            student.Degree = request.Degree.Trim();
            student.GraduationYear = request.GraduationYear;
            student.CGPA = request.CGPA;
            student.ActiveBacklogs = request.ActiveBacklogs;
            student.ResumeUrl = request.ResumeUrl;

            var updated = await _studentRepository.UpdateAsync(student);

            return MapToResponse(updated);
        }

        public async Task<StudentResponse> ApproveAsync(
            int id,
            int officerId)
        {
            await ValidateOfficerAsync(officerId);

            var student = await _studentRepository.GetByIdAsync(id);

            if (student == null)
                throw new KeyNotFoundException("Student not found.");

            if (student.PlacementOfficerId != officerId)
                throw new InvalidOperationException(
                    "You cannot approve a student managed by another officer.");

            if (student.Status != StudentStatus.Pending)
                throw new InvalidOperationException(
                    "Only pending students can be approved.");

            student.Status = StudentStatus.Approved;
            student.ApprovedAt = DateTime.UtcNow;

            var updated = await _studentRepository.UpdateAsync(student);

            return MapToResponse(updated);
        }

        public async Task<StudentResponse> RejectAsync(
            int id,
            int officerId)
        {
            await ValidateOfficerAsync(officerId);

            var student = await _studentRepository.GetByIdAsync(id);

            if (student == null)
                throw new KeyNotFoundException("Student not found.");

            if (student.PlacementOfficerId != officerId)
                throw new InvalidOperationException(
                    "You cannot reject a student managed by another officer.");

            if (student.Status != StudentStatus.Pending)
                throw new InvalidOperationException(
                    "Only pending students can be rejected.");

            student.Status = StudentStatus.Rejected;
            student.ApprovedAt = null;

            var updated = await _studentRepository.UpdateAsync(student);

            return MapToResponse(updated);
        }

        public async Task<StudentImportResult> ImportExcelAsync(
            int officerId,
            IFormFile file)
        {
            var officer = await ValidateOfficerAsync(officerId);

            if (file == null || file.Length == 0)
                throw new InvalidOperationException(
                    "Please upload a non-empty Excel file.");

            if (!Path.GetExtension(file.FileName)
                .Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Only .xlsx files are supported.");
            }

            var result = new StudentImportResult();

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            stream.Position = 0;

            using var workbook = new XLWorkbook(stream);

            var worksheet = workbook.Worksheet(1);
            var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;

            for (int row = 2; row <= lastRow; row++)
            {
                result.TotalRecords++;

                try
                {
                    var userId = worksheet.Cell(row, 1).GetString().Trim();
                    var registerNumber =
                        worksheet.Cell(row, 2).GetString().Trim();
                    var department =
                        worksheet.Cell(row, 3).GetString().Trim();
                    var degree =
                        worksheet.Cell(row, 4).GetString().Trim();

                    if (!int.TryParse(
                        worksheet.Cell(row, 5).GetString(),
                        out int graduationYear))
                    {
                        throw new InvalidOperationException(
                            "Invalid graduation year.");
                    }

                    if (!decimal.TryParse(
                        worksheet.Cell(row, 6).GetString(),
                        out decimal cgpa))
                    {
                        throw new InvalidOperationException("Invalid CGPA.");
                    }

                    if (!int.TryParse(
                        worksheet.Cell(row, 7).GetString(),
                        out int backlogs))
                    {
                        throw new InvalidOperationException(
                            "Invalid active backlog count.");
                    }

                    var resumeUrl =
                        worksheet.Cell(row, 8).GetString().Trim();

                    if (string.IsNullOrWhiteSpace(userId) ||
                        string.IsNullOrWhiteSpace(registerNumber) ||
                        string.IsNullOrWhiteSpace(department) ||
                        string.IsNullOrWhiteSpace(degree))
                    {
                        throw new InvalidOperationException(
                            "Required fields are missing.");
                    }

                    if (graduationYear < 2000 || graduationYear > 2100)
                        throw new InvalidOperationException(
                            "Graduation year is outside the allowed range.");

                    if (cgpa < 0 || cgpa > 10)
                        throw new InvalidOperationException(
                            "CGPA must be between 0 and 10.");

                    if (backlogs < 0 || backlogs > 100)
                        throw new InvalidOperationException(
                            "Active backlogs must be between 0 and 100.");

                    if (await _userManager.FindByIdAsync(userId) == null)
                        throw new InvalidOperationException(
                            "Identity user not found.");

                    if (await _studentRepository.GetByUserIdAsync(userId)
                        != null)
                    {
                        throw new InvalidOperationException(
                            "User already has a student profile.");
                    }

                    if (await _studentRepository
                        .GetByCollegeAndRegisterNumberAsync(
                            officer.CollegeId,
                            registerNumber) != null)
                    {
                        throw new InvalidOperationException(
                            "Register number already exists in this college.");
                    }

                    var student = new Student
                    {
                        UserId = userId,
                        CollegeId = officer.CollegeId,
                        PlacementOfficerId = officerId,
                        RegisterNumber = registerNumber,
                        Department = department,
                        Degree = degree,
                        GraduationYear = graduationYear,
                        CGPA = cgpa,
                        ActiveBacklogs = backlogs,
                        ResumeUrl = string.IsNullOrWhiteSpace(resumeUrl)
                            ? null
                            : resumeUrl,
                        Status = StudentStatus.Pending,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _studentRepository.CreateAsync(student);

                    result.ImportedRecords++;
                }
                catch (Exception ex)
                {
                    result.Errors.Add($"Row {row}: {ex.Message}");
                }
            }

            result.FailedRecords =
                result.TotalRecords - result.ImportedRecords;

            return result;
        }
    }
}