using System.ComponentModel.DataAnnotations;
using System.Net.Security;
using FluentValidation;

namespace Lesson.Validators;

public class JsonValidator : AbstractValidator<string> {
    public JsonValidator() {
        RuleFor(x => x.Length).NotEmpty().GreaterThan(1).WithMessage("String length must be greater than 1");
        RuleFor(x => x)
            .Must(str => {
                str = str.Trim().Replace("\n", ""); // Форматируем строку
                if (!str.StartsWith("{")) // Начинается с {
                    throw new Exception("Json string must start with '{'");
                
                if (!str.EndsWith("}")) { // Заканчивается на }
                    throw new Exception("Json string must end with '}'");
                }
                
                var array = str.Split(":"); // На n-ое количество ключей n-ое количество значений
                if (array.Length % 2 != 0 & array.Length > 0) {
                    return false;
                }
                
                return true;
            }).WithMessage("Not Valid Json string");
    }
}