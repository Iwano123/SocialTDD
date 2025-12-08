using SocialTDD.Application.DTOs;

namespace SocialTDD.Application.Interfaces;

public interface ITimelineService
{
    Task<IEnumerable<PostResponse>> GetTimelineAsync(Guid userId);
}