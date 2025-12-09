using System.ComponentModel.DataAnnotations;

namespace SocialTDD.Api.Models.Requests;

public class SendDirectMessageRequest
{
    [Required]
    public int SenderId { get; set; }
    
    [Required]
    public int RecipientId { get; set; }
    
    [Required]
    [StringLength(1000, MinimumLength = 1)]
    public string Content { get; set; } = string.Empty;
}


