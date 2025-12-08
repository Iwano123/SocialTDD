using SocialTDD.Application.DTOs;
using SocialTDD.Application.Interfaces;
using SocialTDD.Domain.Entities;

namespace SocialTDD.Application.Services;

public class TimelineService : ITimelineService
{
    private readonly IPostRepository _postRepository;

    public TimelineService(IPostRepository postRepository)
    {
        _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
    }

    public async Task<IEnumerable<PostResponse>> GetTimelineAsync(Guid userId)
    {
        // TODO: Implementera enligt TDD - låt testet vägleda dig!
        throw new NotImplementedException();
    }
}