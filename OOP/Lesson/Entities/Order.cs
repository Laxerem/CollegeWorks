using System.Globalization;
using System.Text;
using System.Text.Json;
using Lesson.Interfaces;
using Lesson.Utils;
using Lesson.Validators;

namespace Lesson.Entities;

public class Order : IJsonable<Order>
{
    public readonly DateTime dateTime;
    public readonly string student;
    public List<Meal> meals;

    private Order(DateTime dateTime, string student, List<Meal> meals) {
        this.dateTime = dateTime;
        this.student = student;
        this.meals = meals;
    }

    public static Order Create(string student, List<Meal> meals) {
        return new Order(
            dateTime: DateTime.Now,
            student: student,
            meals: meals
        );
    }
    
    public static Order Create(DateTime date, string student, List<Meal> meals) {
        return new Order(
            dateTime: date,
            student: student,
            meals: meals
        );
    }

    public static Order FromJson(string jsonString) {
        var normalizeJson = JsonHelper.NormalizeJson(jsonString);
        
        DateTime dateTime = new();
        string student = string.Empty;
        List<Meal> meals = new();

        for (int i = 0; i < normalizeJson.Length; i++) {
            if (normalizeJson[i] == '"') {
                var endIndex = IBaseJsonable.GetElementEndIndex(i, '"', '"', normalizeJson);
                if (endIndex.HasValue) {
                    int end = endIndex.Value;
                    var resultString = normalizeJson.Substring(i, end);

                    switch (resultString) {
                        case "date":
                            break;
                        case "student":
                            break;
                        case "meals":
                            break;
                    }
                }
            }
        }
        
        return Create(dateTime,  student, meals);
    }

    public string ToJson() {
        var sp = new StringBuilder();
        sp.Append("{");
        sp.Append($"\"date\": \"{dateTime:dd.MM.yyyy HH:mm:ss}\",");
        sp.Append($"\"student\": \"{student}\",");
        sp.Append($"\"meals\": {JsonHelper.SerializeJsonList(meals)}");
        sp.Append("}");
        return sp.ToString();
    }
}