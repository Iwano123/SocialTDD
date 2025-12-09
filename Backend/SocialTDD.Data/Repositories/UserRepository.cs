using Microsoft.EntityFrameworkCore;
using SocialTDD.Data.Contexts;
using SocialTDD.Domain.Models;

namespace SocialTDD.Data.Repositories;

public class UserRepository : IUserRepository
{
    private readonly SocialDbContext _context;

    public UserRepository(SocialDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id);
    }
}


