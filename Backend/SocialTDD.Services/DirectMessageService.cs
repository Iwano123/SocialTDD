using SocialTDD.Domain.Models;
using SocialTDD.Data.Repositories;
using SocialTDD.Services.Interfaces;

namespace SocialTDD.Services;

public class DirectMessageService : IDirectMessageService
{
    private readonly IUserRepository _userRepository;
    private readonly IDirectMessageRepository _messageRepository;
    private const int MaxMessageLength = 1000;

    public DirectMessageService(IUserRepository userRepository, IDirectMessageRepository messageRepository)
    {
        _userRepository = userRepository;
        _messageRepository = messageRepository;
    }

    public async Task<DirectMessage> SendDirectMessageAsync(int senderId, int recipientId, string content)
    {
        // Validera input
        if (string.IsNullOrWhiteSpace(content))
        {
            throw new ArgumentException("Message content cannot be empty", nameof(content));
        }
        
        if (content.Length > MaxMessageLength)
        {
            throw new ArgumentException($"Message content cannot exceed {MaxMessageLength} characters", nameof(content));
        }
        
        // Förhindra att skicka till sig själv
        if (senderId == recipientId)
        {
            throw new InvalidOperationException("Cannot send message to yourself");
        }
        
        // Verifiera att mottagare existerar
        var recipient = await _userRepository.GetByIdAsync(recipientId);
        if (recipient == null)
        {
            throw new InvalidOperationException($"Recipient with id {recipientId} not found");
        }
        
        var message = new DirectMessage
        {
            SenderId = senderId,
            RecipientId = recipientId,
            Content = content.Trim(),
            SentAt = DateTime.UtcNow
        };
        
        return await _messageRepository.AddAsync(message);
    }

    public async Task<IEnumerable<DirectMessage>> GetReceivedMessagesAsync(int userId)
    {
        var messages = await _messageRepository.GetByRecipientIdAsync(userId);
        return messages.OrderByDescending(m => m.SentAt);
    }

    public async Task<IEnumerable<DirectMessage>> GetConversationAsync(int userId1, int userId2)
    {
        var messages = await _messageRepository.GetConversationAsync(userId1, userId2);
        return messages.OrderBy(m => m.SentAt);
    }
}

