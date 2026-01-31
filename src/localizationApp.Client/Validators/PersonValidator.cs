using FluentValidation;
using localizationApp.Client.Models.Dtos;
using localizationApp.Client.Services.Translation;

namespace localizationApp.Client.Validators;

public class CreatePersonDtoValidator : AbstractValidator<CreatePersonDto>
{
    public CreatePersonDtoValidator(TranslationService trad)
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage(trad.Global.Validation["Required"])
            .MaximumLength(50).WithMessage(string.Format(trad.Global.Validation["MaxLength"], 50));

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage(trad.Global.Validation["Required"])
            .MaximumLength(50).WithMessage(string.Format(trad.Global.Validation["MaxLength"], 50));

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(trad.Global.Validation["Required"])
            .MaximumLength(100).WithMessage(string.Format(trad.Global.Validation["MaxLength"], 100))
            .EmailAddress().WithMessage(trad.Global.Validation["InvalidEmail"]);
    }
}

public class UpdatePersonDtoValidator : AbstractValidator<UpdatePersonDto>
{
    public UpdatePersonDtoValidator(TranslationService trad)
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage(trad.Global.Validation["Required"])
            .MaximumLength(50).WithMessage(string.Format(trad.Global.Validation["MaxLength"], 50));

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage(trad.Global.Validation["Required"])
            .MaximumLength(50).WithMessage(string.Format(trad.Global.Validation["MaxLength"], 50));

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(trad.Global.Validation["Required"])
            .MaximumLength(100).WithMessage(string.Format(trad.Global.Validation["MaxLength"], 100))
            .EmailAddress().WithMessage(trad.Global.Validation["InvalidEmail"]);
    }
}