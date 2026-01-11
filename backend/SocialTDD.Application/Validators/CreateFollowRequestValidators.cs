using FluentValidation;
using SocialTDD.Application.DTOs;

namespace SocialTDD.Application.Validators;

public class CreateFollowRequestValidator : AbstractValidator<CreateFollowRequest>
{
    public CreateFollowRequestValidator()
    {
        RuleFor(x => x.FollowerId)
            .NotEmpty().WithMessage("FollowerId får inte vara tomt.");

        RuleFor(x => x.FollowingId)
            .NotEmpty().WithMessage("FollowingId får inte vara tomt.");

        RuleFor(x => x)
            .Must(x => x.FollowerId != x.FollowingId)
            .WithMessage("En användare kan inte följa sig själv.");
    }
}