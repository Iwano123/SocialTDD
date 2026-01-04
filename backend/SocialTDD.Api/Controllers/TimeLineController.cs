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
        _logger.LogWarning("Ogiltigt användar-ID vid hämtning av tidslinje: {Message}", ex.Message);
        return BadRequest(new { error = ex.Message });
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Ett oväntat fel uppstod vid hämtning av tidslinje");
        return StatusCode(500, new { error = "Ett oväntat fel uppstod." });
    }
}