using Microsoft.AspNetCore.Mvc;
using SocialTDD.Application.DTOs;
using SocialTDD.Application.Interfaces;

namespace SocialTDD.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostsController : ControllerBase
{
    private readonly IPostService _postService;
    private readonly ITimelineService _timelineService;
    private readonly ILogger<PostsController> _logger;

    public PostsController(IPostService postService, ITimelineService timelineService, ILogger<PostsController> logger)
    {
        _postService = postService;
        _timelineService = timelineService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<PostResponse>> CreatePost([FromBody] CreatePostRequest request)
    {
        // Validering sker automatiskt via FluentValidation.AspNetCore
        // Om valideringen misslyckas returneras automatiskt BadRequest med felmeddelanden
        
        try
        {
            var result = await _postService.CreatePostAsync(request);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Ogiltigt argument vid skapande av inlägg: {Message}", ex.Message);
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ett oväntat fel uppstod vid skapande av inlägg");
            return StatusCode(500, new { error = "Ett oväntat fel uppstod. Försök igen senare." });
        }
    }

    [HttpGet("timeline/{userId}")]
    public async Task<ActionResult<List<PostResponse>>> GetTimeline(Guid userId)
    {
        try
        {
            var result = await _timelineService.GetTimelineAsync(userId);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Ogiltigt argument vid hämtning av tidslinje: {Message}", ex.Message);
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ett oväntat fel uppstod vid hämtning av tidslinje");
            return StatusCode(500, new { error = "Ett oväntat fel uppstod. Försök igen senare." });
        }
    }
}


