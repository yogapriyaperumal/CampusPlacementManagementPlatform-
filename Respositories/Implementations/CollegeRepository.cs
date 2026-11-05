using CampusPlacementManagementPlatform.Data;
using CampusPlacementManagementPlatform.Models;
using CampusPlacementManagementPlatform.Respositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CampusPlacementManagementPlatform.Respositories.Implementations
{
    public class CollegeRepository : ICollegeRepository
    {
        private readonly ApplicationDbContext _context;

        public CollegeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<College> CreateAsync(College college)
        {
            await _context.Colleges.AddAsync(college);

            await _context.SaveChangesAsync();

            return college;
        }

        public async Task<List<College>> GetAllAsync()
        {
            return await _context.Colleges
                .AsNoTracking()
                .OrderBy(c => c.CollegeName)
                .ToListAsync();
        }

        public async Task<College?> GetByIdAsync(int id)
        {
            return await _context.Colleges
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CollegeId == id);
        }

        public async Task<College?> GetByCodeAsync(string collegeCode)
        {
            return await _context.Colleges
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CollegeCode == collegeCode);
        }

        public async Task UpdateAsync(College college)
        {
            _context.Colleges.Update(college);

            await _context.SaveChangesAsync();
        }
    }
}