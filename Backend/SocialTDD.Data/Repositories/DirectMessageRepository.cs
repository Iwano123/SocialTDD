using Microsoft.EntityFrameworkCore;
using SocialTDD.Data.Contexts;
using SocialTDD.Domain.Models;

namespace SocialTDD.Data.Repositories;

public class DirectMessageRepository : IDirectMessageRepository
{
    private readonly SocialDbContext _context;

    public DirectMessageRepository(SocialDbContext context)
    {
        _context = context;
    }

    public async Task<DirectMessage> AddAsync(DirectMessage message)
    {
        _context.DirectMessages.Add(message);
        await _context.SaveChangesAsync();
        return message;
    }

    public async Task<IEnumerable<DirectMessage>> GetByRecipientIdAsync(int recipientId)
    {
        return await _context.DirectMessages
            .Include(dm => dm.Sender)
            .Where(dm => dm.RecipientId == recipientId)
            .ToListAsync();
    }

    public async Task<IEnumerable<DirectMessage>> GetConversationAsync(int userId1, int userId2)
    {
        return await _context.DirectMessages
            .Include(dm => dm.Sender)
            .Include(dm => dm.Recipient)
            .Where(dm => 
                (dm.SenderId == userId1 && dm.RecipientId == userId2) ||
                (dm.SenderId == userId2 && dm.RecipientId == userId1))
            .ToListAsync();
    }
}


