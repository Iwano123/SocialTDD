using FluentAssertions;
using FluentValidation;
using Moq;
using SocialTDD.Application.DTOs;
using SocialTDD.Application.Interfaces;
using SocialTDD.Application.Services;
using SocialTDD.Application.Validators;
using SocialTDD.Domain.Entities;

namespace SocialTDD.Tests.Services;

public class PostServiceTests
{
    private readonly Mock<IPostRepository> _mockRepository;
    private readonly IValidator<CreatePostRequest> _validator;
    private readonly PostService _postService;

    public PostServiceTests()
    {
        _mockRepository = new Mock<IPostRepository>();
        _validator = new CreatePostRequestValidator();
        _postService = new PostService(_mockRepository.Object, _validator);
    }

    [Fact]
    public async Task CreatePostAsync_ValidInput_ReturnsPostResponse()
    {
        // Arrange
        var senderId = Guid.NewGuid();
        var recipientId = Guid.NewGuid();
        var request = new CreatePostRequest
        {
            SenderId = senderId,
            RecipientId = recipientId,
            Message = "Detta är ett testmeddelande"
        };

        var expectedPost = new Post
        {
            Id = Guid.NewGuid(),
            SenderId = senderId,
            RecipientId = recipientId,
            Message = request.Message,
            CreatedAt = DateTime.UtcNow
        };

        _mockRepository.Setup(r => r.UserExistsAsync(senderId)).ReturnsAsync(true);
        _mockRepository.Setup(r => r.UserExistsAsync(recipientId)).ReturnsAsync(true);
        _mockRepository.Setup(r => r.CreateAsync(It.IsAny<Post>())).ReturnsAsync(expectedPost);

        // Act
        var result = await _postService.CreatePostAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.SenderId.Should().Be(senderId);
        result.RecipientId.Should().Be(recipientId);
        result.Message.Should().Be(request.Message);
        result.Id.Should().Be(expectedPost.Id);
        _mockRepository.Verify(r => r.CreateAsync(It.IsAny<Post>()), Times.Once);
    }

    [Fact]
    public async Task CreatePostAsync_EmptyMessage_ThrowsValidationException()
    {
        // Arrange
        var request = new CreatePostRequest
        {
            SenderId = Guid.NewGuid(),
            RecipientId = Guid.NewGuid(),
            Message = string.Empty
        };

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _postService.CreatePostAsync(request));
        _mockRepository.Verify(r => r.CreateAsync(It.IsAny<Post>()), Times.Never);
    }

    [Fact]
    public async Task CreatePostAsync_MessageTooLong_ThrowsValidationException()
    {
        // Arrange
        var request = new CreatePostRequest
        {
            SenderId = Guid.NewGuid(),
            RecipientId = Guid.NewGuid(),
            Message = new string('a', 501) // Över maxlängd på 500 tecken
        };

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _postService.CreatePostAsync(request));
        _mockRepository.Verify(r => r.CreateAsync(It.IsAny<Post>()), Times.Never);
    }

    [Fact]
    public async Task CreatePostAsync_SenderAndRecipientSame_ThrowsValidationException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new CreatePostRequest
        {
            SenderId = userId,
            RecipientId = userId,
            Message = "Testmeddelande"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _postService.CreatePostAsync(request));
        _mockRepository.Verify(r => r.CreateAsync(It.IsAny<Post>()), Times.Never);
    }

    [Fact]
    public async Task CreatePostAsync_InvalidSender_ThrowsArgumentException()
    {
        // Arrange
        var senderId = Guid.NewGuid();
        var recipientId = Guid.NewGuid();
        var request = new CreatePostRequest
        {
            SenderId = senderId,
            RecipientId = recipientId,
            Message = "Testmeddelande"
        };

        _mockRepository.Setup(r => r.UserExistsAsync(senderId)).ReturnsAsync(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _postService.CreatePostAsync(request));
        exception.Message.Should().Contain("Avsändare");
        exception.Message.Should().Contain("finns inte");
        _mockRepository.Verify(r => r.CreateAsync(It.IsAny<Post>()), Times.Never);
    }

    [Fact]
    public async Task CreatePostAsync_InvalidRecipient_ThrowsArgumentException()
    {
        // Arrange
        var senderId = Guid.NewGuid();
        var recipientId = Guid.NewGuid();
        var request = new CreatePostRequest
        {
            SenderId = senderId,
            RecipientId = recipientId,
            Message = "Testmeddelande"
        };

        _mockRepository.Setup(r => r.UserExistsAsync(senderId)).ReturnsAsync(true);
        _mockRepository.Setup(r => r.UserExistsAsync(recipientId)).ReturnsAsync(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _postService.CreatePostAsync(request));
        exception.Message.Should().Contain("Mottagare");
        exception.Message.Should().Contain("finns inte");
        _mockRepository.Verify(r => r.CreateAsync(It.IsAny<Post>()), Times.Never);
    }

    [Fact]
    public async Task CreatePostAsync_EmptySenderId_ThrowsValidationException()
    {
        // Arrange
        var request = new CreatePostRequest
        {
            SenderId = Guid.Empty,
            RecipientId = Guid.NewGuid(),
            Message = "Testmeddelande"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _postService.CreatePostAsync(request));
        _mockRepository.Verify(r => r.CreateAsync(It.IsAny<Post>()), Times.Never);
    }

    [Fact]
    public async Task CreatePostAsync_EmptyRecipientId_ThrowsValidationException()
    {
        // Arrange
        var request = new CreatePostRequest
        {
            SenderId = Guid.NewGuid(),
            RecipientId = Guid.Empty,
            Message = "Testmeddelande"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _postService.CreatePostAsync(request));
        _mockRepository.Verify(r => r.CreateAsync(It.IsAny<Post>()), Times.Never);
    }

    [Fact]
    public async Task GetConversationAsync_ValidUsers_ReturnsConversation()
    {
        // Arrange
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var now = DateTime.UtcNow;

        var posts = new List<Post>
        {
            new Post
            {
                Id = Guid.NewGuid(),
                SenderId = userId1,
                RecipientId = userId2,
                Message = "Hej från användare 1",
                CreatedAt = now.AddMinutes(-10)
            },
            new Post
            {
                Id = Guid.NewGuid(),
                SenderId = userId2,
                RecipientId = userId1,
                Message = "Hej från användare 2",
                CreatedAt = now.AddMinutes(-5)
            },
            new Post
            {
                Id = Guid.NewGuid(),
                SenderId = userId1,
                RecipientId = userId2,
                Message = "Svar från användare 1",
                CreatedAt = now
            }
        };

        _mockRepository.Setup(r => r.UserExistsAsync(userId1)).ReturnsAsync(true);
        _mockRepository.Setup(r => r.UserExistsAsync(userId2)).ReturnsAsync(true);
        _mockRepository.Setup(r => r.GetConversationAsync(userId1, userId2)).ReturnsAsync(posts);

        // Act
        var result = await _postService.GetConversationAsync(userId1, userId2);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result[0].Message.Should().Be("Hej från användare 1");
        result[1].Message.Should().Be("Hej från användare 2");
        result[2].Message.Should().Be("Svar från användare 1");
        _mockRepository.Verify(r => r.GetConversationAsync(userId1, userId2), Times.Once);
    }

    [Fact]
    public async Task GetConversationAsync_EmptyConversation_ReturnsEmptyList()
    {
        // Arrange
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();

        _mockRepository.Setup(r => r.UserExistsAsync(userId1)).ReturnsAsync(true);
        _mockRepository.Setup(r => r.UserExistsAsync(userId2)).ReturnsAsync(true);
        _mockRepository.Setup(r => r.GetConversationAsync(userId1, userId2)).ReturnsAsync(new List<Post>());

        // Act
        var result = await _postService.GetConversationAsync(userId1, userId2);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
        _mockRepository.Verify(r => r.GetConversationAsync(userId1, userId2), Times.Once);
    }

    [Fact]
    public async Task GetConversationAsync_InvalidUser1_ThrowsArgumentException()
    {
        // Arrange
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();

        _mockRepository.Setup(r => r.UserExistsAsync(userId1)).ReturnsAsync(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _postService.GetConversationAsync(userId1, userId2));
        exception.Message.Should().Contain("finns inte");
        exception.ParamName.Should().Be("userId1");
        _mockRepository.Verify(r => r.GetConversationAsync(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task GetConversationAsync_InvalidUser2_ThrowsArgumentException()
    {
        // Arrange
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();

        _mockRepository.Setup(r => r.UserExistsAsync(userId1)).ReturnsAsync(true);
        _mockRepository.Setup(r => r.UserExistsAsync(userId2)).ReturnsAsync(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _postService.GetConversationAsync(userId1, userId2));
        exception.Message.Should().Contain("finns inte");
        exception.ParamName.Should().Be("userId2");
        _mockRepository.Verify(r => r.GetConversationAsync(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task GetConversationAsync_SameUser_ThrowsArgumentException()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _mockRepository.Setup(r => r.UserExistsAsync(userId)).ReturnsAsync(true);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _postService.GetConversationAsync(userId, userId));
        exception.Message.Should().Contain("kan inte ha en konversation med sig själv");
        _mockRepository.Verify(r => r.GetConversationAsync(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task GetConversationAsync_ConversationOrderedByTime()
    {
        // Arrange
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var now = DateTime.UtcNow;

        var posts = new List<Post>
        {
            new Post
            {
                Id = Guid.NewGuid(),
                SenderId = userId1,
                RecipientId = userId2,
                Message = "Tredje meddelandet",
                CreatedAt = now
            },
            new Post
            {
                Id = Guid.NewGuid(),
                SenderId = userId2,
                RecipientId = userId1,
                Message = "Första meddelandet",
                CreatedAt = now.AddMinutes(-20)
            },
            new Post
            {
                Id = Guid.NewGuid(),
                SenderId = userId1,
                RecipientId = userId2,
                Message = "Andra meddelandet",
                CreatedAt = now.AddMinutes(-10)
            }
        };

        // Sortera posts som repository skulle göra
        var sortedPosts = posts.OrderBy(p => p.CreatedAt).ToList();

        _mockRepository.Setup(r => r.UserExistsAsync(userId1)).ReturnsAsync(true);
        _mockRepository.Setup(r => r.UserExistsAsync(userId2)).ReturnsAsync(true);
        _mockRepository.Setup(r => r.GetConversationAsync(userId1, userId2)).ReturnsAsync(sortedPosts);

        // Act
        var result = await _postService.GetConversationAsync(userId1, userId2);

        // Assert
        result.Should().HaveCount(3);
        result[0].Message.Should().Be("Första meddelandet");
        result[1].Message.Should().Be("Andra meddelandet");
        result[2].Message.Should().Be("Tredje meddelandet");
    }
}


