using CleanArchitectureTemplate.Application.Common.Interfaces;
using CleanArchitectureTemplate.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureTemplate.Infrastructure.Persistence.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _context.Set<User>()
            .Include(u => u.Campus)
            .FirstOrDefaultAsync(u => u.Email == username);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Set<User>()
            .Include(u => u.Campus)
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetByUserCodeAsync(string userCode)
    {
        return await _context.Set<User>()
            .Include(u => u.Campus)
            .FirstOrDefaultAsync(u => u.UserCode == userCode);
    }

    public async Task<bool> IsEmailExistsAsync(string email)
    {
        return await _context.Set<User>()
            .AnyAsync(u => u.Email == email);
    }

    public async Task<bool> IsUserCodeExistsAsync(string userCode)
    {
        return await _context.Set<User>()
            .AnyAsync(u => u.UserCode == userCode);
    }
}