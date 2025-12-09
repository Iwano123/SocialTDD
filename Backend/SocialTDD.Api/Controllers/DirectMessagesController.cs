using Microsoft.AspNetCore.Mvc;
using SocialTDD.Services.Interfaces;
using SocialTDD.Api.Models.Requests;
using SocialTDD.Api.Models.Responses;

namespace SocialTDD.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DirectMessagesController : ControllerBase
{
    private readonly IDirectMessageService _service;
    private readonly ILogger<DirectMessagesController> _logger;

    public DirectMessagesController(
        IDirectMessageService service,
        ILogger<DirectMessagesController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> SendDirectMessage(
        [FromBody] SendDirectMessageRequest request)
    {
        try
        {
            var message = await _service.SendDirectMessageAsync(
                request.SenderId, 
                request.RecipientId, 
                request.Content);
            
            return Ok(new DirectMessageResponse(message));
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Validation error when sending DM");
            return BadRequest(new ErrorResponse(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Business logic error when sending DM");
            return BadRequest(new ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending DM");
            return StatusCode(500, new ErrorResponse("An error occurred while sending the message"));
        }
    }

    [HttpGet("received/{userId}")]
    public async Task<IActionResult> GetReceivedMessages(int userId)
    {
        try
        {
            var messages = await _service.GetReceivedMessagesAsync(userId);
            return Ok(messages.Select(m => new DirectMessageResponse(m)));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving received messages");
            return StatusCode(500, new ErrorResponse("An error occurred while retrieving messages"));
        }
    }

    [HttpGet("conversation/{userId1}/{userId2}")]
    public async Task<IActionResult> GetConversation(int userId1, int userId2)
    {
        try
        {
            var messages = await _service.GetConversationAsync(userId1, userId2);
            return Ok(messages.Select(m => new DirectMessageResponse(m)));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving conversation");
            return StatusCode(500, new ErrorResponse("An error occurred while retrieving conversation"));
        }
    }
}


