using FluentValidation;
using SocialTDD.Application.DTOs;

namespace SocialTDD.Application.Validators;

public class CreatePostRequestValidator : AbstractValidator<CreatePostRequest>
{
    private const int MaxMessageLength = 500;
    private const int MinMessageLength = 1;

    public CreatePostRequestValidator()
    {
        // Validera SenderId
        RuleFor(x => x.SenderId)
            .NotEmpty()
            .WithMessage("Avsändare-ID är obligatoriskt.")
            .NotEqual(Guid.Empty)
            .WithMessage("Avsändare-ID får inte vara tomt.");

        // Validera RecipientId
        RuleFor(x => x.RecipientId)
            .NotEmpty()
            .WithMessage("Mottagare-ID är obligatoriskt.")
            .NotEqual(Guid.Empty)
            .WithMessage("Mottagare-ID får inte vara tomt.");

        // Validera att avsändare och mottagare inte är samma
        RuleFor(x => x.SenderId)
            .NotEqual(x => x.RecipientId)
            .WithMessage("Avsändare och mottagare kan inte vara samma användare.");

        // Validera Message
        RuleFor(x => x.Message)
            .NotNull()
            .WithMessage("Meddelande är obligatoriskt.")
            .Must(message => !string.IsNullOrWhiteSpace(message))
            .WithMessage("Meddelande får inte vara tomt eller bara innehålla mellanslag.")
            .MinimumLength(MinMessageLength)
            .WithMessage($"Meddelande måste vara minst {MinMessageLength} tecken.")
            .MaximumLength(MaxMessageLength)
            .WithMessage($"Meddelande får inte vara längre än {MaxMessageLength} tecken.")
            .Must(message => message != null && message.Trim().Length >= MinMessageLength)
            .WithMessage($"Meddelande måste vara minst {MinMessageLength} tecken efter borttagning av mellanslag.");
    }
}




