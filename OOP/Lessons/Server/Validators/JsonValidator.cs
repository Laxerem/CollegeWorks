using FluentValidation;

namespace Server.Validators;

public class JsonValidator : AbstractValidator<string> {
    public JsonValidator() {
        RuleFor(x => x).NotEmpty().WithMessage("String must not be empty");

        RuleFor(x => x)
            .Must(str => str.Trim().StartsWith("{") || str.Trim().StartsWith("["))
            .WithMessage("Json string must start with '{' or '['");

        RuleFor(x => x)
            .Must(str => str.Trim().EndsWith("}") || str.Trim().EndsWith("]"))
            .WithMessage("Json string must end with '}' or ']'");

        RuleFor(x => x)
            .Must(str => {
                var trimmed = str.Trim();
                var colonCount = trimmed.Count(c => c == ':');
                return colonCount > 0;
            })
            .WithMessage("Json must contain at least one key-value pair");
    }
}