using Microsoft.AspNetCore.Mvc;
using SocialTDD.Application.DTOs;
using SocialTDD.Application.Interfaces;

namespace SocialTDD.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DirectMessagesController : ControllerBase
{
    private readonly IDirectMessageService _directMessageService;
    private readonly ILogger<DirectMessagesController> _logger;

    public DirectMessagesController(IDirectMessageService directMessageService, ILogger<DirectMessagesController> logger)
    {
        _directMessageService = directMessageService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<DirectMessageResponse>> SendDirectMessage([FromBody] CreateDirectMessageRequest request)
    {
        // Validering sker automatiskt via FluentValidation.AspNetCore
        // Om valideringen misslyckas returneras automatiskt BadRequest med felmeddelanden
        
        try
        {
            var result = await _directMessageService.SendDirectMessageAsync(request);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Ogiltigt argument vid skickande av DM: {Message}", ex.Message);
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ett oväntat fel uppstod vid skickande av DM");
            return StatusCode(500, new { error = "Ett oväntat fel uppstod. Försök igen senare." });
        }
    }

    [HttpGet("{userId}")]
    public async Task<ActionResult<List<DirectMessageResponse>>> GetReceivedMessages(Guid userId)
    {
        try
        {
            var result = await _directMessageService.GetReceivedMessagesAsync(userId);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Ogiltigt argument vid hämtning av DM: {Message}", ex.Message);
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ett oväntat fel uppstod vid hämtning av DM");
            return StatusCode(500, new { error = "Ett oväntat fel uppstod. Försök igen senare." });
        }
    }

    [HttpPut("{messageId}/read")]
    public async Task<IActionResult> MarkAsRead(Guid messageId)
    {
        try
        {
            await _directMessageService.MarkAsReadAsync(messageId);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Ogiltigt argument vid markering av DM som läst: {Message}", ex.Message);
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ett oväntat fel uppstod vid markering av DM som läst");
            return StatusCode(500, new { error = "Ett oväntat fel uppstod. Försök igen senare." });
        }
    }
}

