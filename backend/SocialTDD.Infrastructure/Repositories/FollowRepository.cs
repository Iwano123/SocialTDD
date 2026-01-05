using Microsoft.EntityFrameworkCore;
using SocialTDD.Application.Interfaces;
using SocialTDD.Domain.Entities;
using SocialTDD.Infrastructure.Data;

namespace SocialTDD.Infrastructure.Repositories;

public class FollowRepository : IFollowRepository
{
  private readonly ApplicationDbContext _context;

  public FollowRepository(ApplicationDbContext context)
  {
    _context = context;
  }

  public async Task<Follow> CreateAsync(Follow follow)
  {
    _context.Follows.Add(follow);
    await _context.SaveChangesAsync();
    return follow;
  }

  public async Task<bool> FollowExistsAsync(Guid followerId, Guid followingId)
  {
    return await _context.Follows
        .AnyAsync(f => f.FollowerId == followerId && f.FollowingId == followingId);
  }

  public async Task<IEnumerable<Follow>> GetFollowersAsync(Guid userId)
  {
    return await _context.Follows
        .Include(f => f.Follower)
        .Include(f => f.Following)
        .Where(f => f.FollowingId == userId)
        .ToListAsync();
  }

  public async Task<IEnumerable<Follow>> GetFollowingAsync(Guid userId)
  {
    return await _context.Follows
        .Include(f => f.Follower)
        .Include(f => f.Following)
        .Where(f => f.FollowerId == userId)
        .ToListAsync();
  }

  public async Task<bool> UserExistsAsync(Guid userId)
  {
    return await _context.Users.AnyAsync(u => u.Id == userId);
  }
}