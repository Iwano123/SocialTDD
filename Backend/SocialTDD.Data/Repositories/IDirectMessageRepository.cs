using SocialTDD.Domain.Models;

namespace SocialTDD.Data.Repositories;

public interface IDirectMessageRepository
{
    Task<DirectMessage> AddAsync(DirectMessage message);
    Task<IEnumerable<DirectMessage>> GetByRecipientIdAsync(int recipientId);
    Task<IEnumerable<DirectMessage>> GetConversationAsync(int userId1, int userId2);
}

