using FluentValidation;
using SocialTDD.Application.DTOs;
using SocialTDD.Application.Interfaces;
using SocialTDD.Domain.Entities;

namespace SocialTDD.Application.Services;

public class PostService : IPostService
{
    private readonly IPostRepository _postRepository;
    private readonly IValidator<CreatePostRequest> _validator;

    public PostService(IPostRepository postRepository, IValidator<CreatePostRequest> validator)
    {
        _postRepository = postRepository;
        _validator = validator;
    }

    public async Task<PostResponse> CreatePostAsync(CreatePostRequest request)
    {
        // Validera input
        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        // Validera att avsändare existerar
        var senderExists = await _postRepository.UserExistsAsync(request.SenderId);
        if (!senderExists)
        {
            throw new ArgumentException($"Avsändare med ID {request.SenderId} finns inte.", nameof(request.SenderId));
        }

        // Validera att mottagare existerar
        var recipientExists = await _postRepository.UserExistsAsync(request.RecipientId);
        if (!recipientExists)
        {
            throw new ArgumentException($"Mottagare med ID {request.RecipientId} finns inte.", nameof(request.RecipientId));
        }

        // Skapa post
        var post = new Post
        {
            Id = Guid.NewGuid(),
            SenderId = request.SenderId,
            RecipientId = request.RecipientId,
            Message = request.Message,
            CreatedAt = DateTime.UtcNow
        };

        var createdPost = await _postRepository.CreateAsync(post);

        return new PostResponse
        {
            Id = createdPost.Id,
            SenderId = createdPost.SenderId,
            RecipientId = createdPost.RecipientId,
            Message = createdPost.Message,
            CreatedAt = createdPost.CreatedAt
        };
    }
}


