using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialTDD.Api.Extensions;
using SocialTDD.Api.Models;
using SocialTDD.Application.DTOs;
using SocialTDD.Application.Interfaces;

namespace SocialTDD.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FollowController : ControllerBase
{
    private readonly IFollowService _followService;

    public FollowController(IFollowService followService)
    {
        _followService = followService;
    }

    [HttpPost]
    public async Task<ActionResult<FollowResponse>> FollowUser([FromBody] CreateFollowRequest request)
    {
        try
        {
            // Hämta FollowerId från JWT token
            var followerId = User.GetUserId();
            
            // Sätt FollowerId från token för säkerhet
            var authenticatedRequest = new CreateFollowRequest
            {
                FollowerId = followerId,
                FollowingId = request.FollowingId
            };
            
            var result = await _followService.FollowUserAsync(authenticatedRequest);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ErrorResponse(ErrorCodes.INVALID_USER_ID, ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new ErrorResponse(ErrorCodes.ALREADY_FOLLOWING, ex.Message));
        }
        catch (FluentValidation.ValidationException ex)
        {
            var details = new Dictionary<string, object>
            {
                { "errors", ex.Errors.Select(e => new { property = e.PropertyName, message = e.ErrorMessage }) }
            };
            return BadRequest(new ErrorResponse(ErrorCodes.VALIDATION_ERROR, ex.Message, details));
        }
    }

    [HttpDelete("{followingId}")]
    public async Task<IActionResult> UnfollowUser(Guid followingId)
    {
        try
        {
            // Hämta FollowerId från JWT token
            var followerId = User.GetUserId();
            await _followService.UnfollowUserAsync(followerId, followingId);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new ErrorResponse(ErrorCodes.USER_NOT_FOUND, ex.Message));
        }
    }

    [HttpGet("followers/{userId}")]
    public async Task<ActionResult<List<FollowResponse>>> GetFollowers(Guid userId)
    {
        try
        {
            var result = await _followService.GetFollowersAsync(userId);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ErrorResponse(ErrorCodes.INVALID_USER_ID, ex.Message));
        }
    }

    [HttpGet("following/{userId}")]
    public async Task<ActionResult<List<FollowResponse>>> GetFollowing(Guid userId)
    {
        try
        {
            var result = await _followService.GetFollowingAsync(userId);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ErrorResponse(ErrorCodes.INVALID_USER_ID, ex.Message));
        }
    }

    [HttpGet("followers")]
    public async Task<ActionResult<List<FollowResponse>>> GetMyFollowers()
    {
        try
        {
            // Hämta UserId från JWT token
            var userId = User.GetUserId();
            var result = await _followService.GetFollowersAsync(userId);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ErrorResponse(ErrorCodes.INVALID_USER_ID, ex.Message));
        }
    }

    [HttpGet("following")]
    public async Task<ActionResult<List<FollowResponse>>> GetMyFollowing()
    {
        try
        {
            // Hämta UserId från JWT token
            var userId = User.GetUserId();
            var result = await _followService.GetFollowingAsync(userId);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ErrorResponse(ErrorCodes.INVALID_USER_ID, ex.Message));
        }
    }
}