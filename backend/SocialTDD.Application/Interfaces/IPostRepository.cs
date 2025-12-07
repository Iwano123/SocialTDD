using SocialTDD.Domain.Entities;

namespace SocialTDD.Application.Interfaces;

public interface IPostRepository
{
    Task<Post> CreateAsync(Post post);
    Task<Post?> GetByIdAsync(Guid id);
    Task<IEnumerable<Post>> GetByRecipientIdAsync(Guid recipientId);
    Task<bool> UserExistsAsync(Guid userId);
}

