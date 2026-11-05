using CampusPlacementManagementPlatform.Data;
using CampusPlacementManagementPlatform.Models;
using CampusPlacementManagementPlatform.Respositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CampusPlacementManagementPlatform.Respositories.Implementations
{
    public class PlacementOfficerRepository : IPlacementOfficerRepository
    {
        private readonly ApplicationDbContext _context;

        public PlacementOfficerRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Create a new placement officer
        public async Task<PlacementOfficer> CreateAsync(PlacementOfficer officer)
        {
            await _context.PlacementOfficers.AddAsync(officer);

            await _context.SaveChangesAsync();

            return await _context.PlacementOfficers
                .Include(p => p.College)
                .FirstAsync(p => p.PlacementOfficerId == officer.PlacementOfficerId);
        }

        // Retrieve all placement officers
        public async Task<List<PlacementOfficer>> GetAllAsync()
        {
            return await _context.PlacementOfficers
                .Include(p => p.College)
                .AsNoTracking()
                .OrderBy(p => p.PlacementOfficerId)
                .ToListAsync();
        }

        // Retrieve placement officer by ID
        public async Task<PlacementOfficer?> GetByIdAsync(int id)
        {
            return await _context.PlacementOfficers
                .Include(p => p.College)
                .FirstOrDefaultAsync(p => p.PlacementOfficerId == id);
        }

        // Retrieve placement officer by Identity User ID
        public async Task<PlacementOfficer?> GetByUserIdAsync(string userId)
        {
            return await _context.PlacementOfficers
                .Include(p => p.College)
                .FirstOrDefaultAsync(p => p.UserId == userId);
        }

        // Update placement officer information
        public async Task UpdateAsync(PlacementOfficer officer)
        {
            _context.PlacementOfficers.Update(officer);

            await _context.SaveChangesAsync();
        }
    }
}