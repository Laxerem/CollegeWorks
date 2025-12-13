using System.ComponentModel.DataAnnotations;
using System.Net.Security;
using FluentValidation;

namespace Lesson.Validators;

public class JsonValidator : AbstractValidator<string> {
    /// <summary>
    /// Быстрое валидирование на соответствие с форматом JSON (Не проверяет полностью)
    /// </summary>
    /// <exception cref="Exception"></exception>
    public JsonValidator() {
        RuleFor(x => x.Length).NotEmpty().GreaterThan(1).WithMessage("String length must be greater than 1");
        RuleFor(x => x)
            .Must(str => {
                str = str.Trim().Replace("\n", ""); // Форматируем строку
                if (!str.StartsWith("{") && !str.StartsWith("[")) // Начинается с {
                    throw new Exception("Json string must start with '{' or '['");
                
                if (!str.EndsWith("}") && !str.EndsWith("]")) { // Заканчивается на }
                    throw new Exception("Json string must end with '}' or ']'");
                }
                
                return true;
            }).WithMessage("Not Valid Json string");
    }
}