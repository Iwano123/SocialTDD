using FluentValidation;
using SocialTDD.Application.DTOs;

namespace SocialTDD.Application.Validators;

public class CreatePostRequestValidator : AbstractValidator<CreatePostRequest>
{
    private const int MaxMessageLength = 500;
    private const int MinMessageLength = 1;

    public CreatePostRequestValidator()
    {
        RuleFor(x => x.SenderId)
            .NotEmpty()
            .WithMessage("Avsändare-ID är obligatoriskt.");

        RuleFor(x => x.RecipientId)
            .NotEmpty()
            .WithMessage("Mottagare-ID är obligatoriskt.");

        RuleFor(x => x.SenderId)
            .NotEqual(x => x.RecipientId)
            .WithMessage("Avsändare och mottagare kan inte vara samma användare.");

        RuleFor(x => x.Message)
            .NotEmpty()
            .WithMessage("Meddelande är obligatoriskt.")
            .MinimumLength(MinMessageLength)
            .WithMessage($"Meddelande måste vara minst {MinMessageLength} tecken.")
            .MaximumLength(MaxMessageLength)
            .WithMessage($"Meddelande får inte vara längre än {MaxMessageLength} tecken.");
    }
}




