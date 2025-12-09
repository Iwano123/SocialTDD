namespace SocialTDD.Domain.Models;

public class DirectMessage
{
    public int Id { get; set; }
    public int SenderId { get; set; }
    public int RecipientId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
    public bool IsRead { get; set; }
    
    // Navigation properties
    public User? Sender { get; set; }
    public User? Recipient { get; set; }
}

