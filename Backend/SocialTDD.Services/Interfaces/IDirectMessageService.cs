using SocialTDD.Domain.Models;

namespace SocialTDD.Services.Interfaces;

public interface IDirectMessageService
{
    Task<DirectMessage> SendDirectMessageAsync(int senderId, int recipientId, string content);
    Task<IEnumerable<DirectMessage>> GetReceivedMessagesAsync(int userId);
    Task<IEnumerable<DirectMessage>> GetConversationAsync(int userId1, int userId2);
}


