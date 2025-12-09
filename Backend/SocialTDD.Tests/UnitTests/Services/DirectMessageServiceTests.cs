using Xunit;
using Moq;
using SocialTDD.Services;
using SocialTDD.Domain.Models;
using SocialTDD.Data.Repositories;

namespace SocialTDD.Tests.UnitTests.Services;

public class DirectMessageServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IDirectMessageRepository> _mockMessageRepository;
    private readonly DirectMessageService _service;

    public DirectMessageServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockMessageRepository = new Mock<IDirectMessageRepository>();
        _service = new DirectMessageService(_mockUserRepository.Object, _mockMessageRepository.Object);
    }

    [Fact]
    public async Task SendDirectMessage_WhenRecipientExists_ShouldSucceed()
    {
        // Arrange
        var senderId = 1;
        var recipientId = 2;
        var content = "Hello!";
        
        var recipient = new User { Id = recipientId, Username = "Bob" };
        _mockUserRepository.Setup(r => r.GetByIdAsync(recipientId))
            .ReturnsAsync(recipient);
        _mockMessageRepository.Setup(r => r.AddAsync(It.IsAny<DirectMessage>()))
            .ReturnsAsync((DirectMessage m) => m);
        
        // Act
        var result = await _service.SendDirectMessageAsync(senderId, recipientId, content);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(senderId, result.SenderId);
        Assert.Equal(recipientId, result.RecipientId);
        Assert.Equal(content, result.Content);
    }

    [Fact]
    public async Task SendDirectMessage_WhenContentIsEmpty_ShouldThrowException()
    {
        // Arrange
        var senderId = 1;
        var recipientId = 2;
        var content = ""; // Tomt meddelande
        
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.SendDirectMessageAsync(senderId, recipientId, content));
    }

    [Fact]
    public async Task SendDirectMessage_WhenRecipientDoesNotExist_ShouldThrowException()
    {
        // Arrange
        var senderId = 1;
        var recipientId = 999; // Finns inte
        var content = "Hello!";
        
        _mockUserRepository.Setup(r => r.GetByIdAsync(recipientId))
            .ReturnsAsync((User?)null);
        
        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _service.SendDirectMessageAsync(senderId, recipientId, content));
    }

    [Fact]
    public async Task GetReceivedMessages_ShouldReturnOnlyMessagesForUser()
    {
        // Arrange
        var userId = 1;
        var messages = new List<DirectMessage>
        {
            new DirectMessage { Id = 1, RecipientId = userId, SenderId = 2, Content = "Message 1", SentAt = DateTime.UtcNow.AddMinutes(-10) },
            new DirectMessage { Id = 2, RecipientId = userId, SenderId = 3, Content = "Message 2", SentAt = DateTime.UtcNow },
            new DirectMessage { Id = 3, RecipientId = 999, SenderId = 2, Content = "Should not appear", SentAt = DateTime.UtcNow }
        };
        
        _mockMessageRepository.Setup(r => r.GetByRecipientIdAsync(userId))
            .ReturnsAsync(messages.Where(m => m.RecipientId == userId));
        
        // Act
        var result = await _service.GetReceivedMessagesAsync(userId);
        
        // Assert
        Assert.Equal(2, result.Count());
        Assert.All(result, m => Assert.Equal(userId, m.RecipientId));
        // Verifiera att de är sorterade i fallande ordning (senaste först)
        Assert.Equal("Message 2", result.First().Content);
    }

    [Fact]
    public async Task SendDirectMessage_WhenContentExceedsMaxLength_ShouldThrowException()
    {
        // Arrange
        var senderId = 1;
        var recipientId = 2;
        var content = new string('a', 1001); // Över maxlängd (1000)
        
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.SendDirectMessageAsync(senderId, recipientId, content));
    }

    [Fact]
    public async Task SendDirectMessage_WhenSendingToSelf_ShouldThrowException()
    {
        // Arrange
        var userId = 1;
        var content = "Hello to myself!";
        
        var user = new User { Id = userId, Username = "Alice" };
        _mockUserRepository.Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync(user);
        
        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _service.SendDirectMessageAsync(userId, userId, content));
    }
}

