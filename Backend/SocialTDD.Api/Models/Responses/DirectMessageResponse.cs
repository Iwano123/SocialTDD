namespace SocialTDD.Api.Models.Responses;

public class DirectMessageResponse
{
    public int Id { get; set; }
    public int SenderId { get; set; }
    public int RecipientId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
    public bool IsRead { get; set; }

    public DirectMessageResponse(Domain.Models.DirectMessage message)
    {
        Id = message.Id;
        SenderId = message.SenderId;
        RecipientId = message.RecipientId;
        Content = message.Content;
        SentAt = message.SentAt;
        IsRead = message.IsRead;
    }
}


