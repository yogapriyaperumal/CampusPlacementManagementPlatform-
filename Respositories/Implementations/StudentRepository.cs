using CampusPlacementManagementPlatform.Data;
using CampusPlacementManagementPlatform.Models;
using CampusPlacementManagementPlatform.Respositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CampusPlacementManagementPlatform.Respositories.Implementations
{
    public class StudentRepository : IStudentRepository
    {
        private readonly ApplicationDbContext _context;

        public StudentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Student> CreateAsync(Student student)
        {
            await _context.Students.AddAsync(student);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(student.StudentId)
                ?? student;
        }

        public async Task<List<Student>> GetAllAsync()
        {
            return await _context.Students
                .Include(s => s.College)
                .AsNoTracking()
                .OrderBy(s => s.StudentId)
                .ToListAsync();
        }

        public async Task<Student?> GetByIdAsync(int id)
        {
            return await _context.Students
                .Include(s => s.College)
                .FirstOrDefaultAsync(s => s.StudentId == id);
        }

        public async Task<Student?> GetByUserIdAsync(string userId)
        {
            return await _context.Students
                .Include(s => s.College)
                .FirstOrDefaultAsync(s => s.UserId == userId);
        }

        public async Task<Student?> GetByCollegeAndRegisterNumberAsync(
            int collegeId,
            string registerNumber)
        {
            return await _context.Students
                .FirstOrDefaultAsync(s =>
                    s.CollegeId == collegeId &&
                    s.RegisterNumber == registerNumber);
        }

        public async Task<List<Student>> GetByOfficerIdAsync(int officerId)
        {
            return await _context.Students
                .Include(s => s.College)
                .Where(s => s.PlacementOfficerId == officerId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Student> UpdateAsync(Student student)
        {
            _context.Students.Update(student);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(student.StudentId)
                ?? student;
        }
    }
}