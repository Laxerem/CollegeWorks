using System.Text;
using Lesson.Interfaces;
using Lesson.Validators;

namespace Lesson.Entities;

public class Meal : IJsonable<Meal> {
    private string id;
    private string title;
    private int cost;

    private Meal() {
        this.id = "none";
        this.title = "none";
        this.cost = 0;
    }

    public Meal(string id, string title, int cost) {
        this.id = id;
        this.title = title;
        this.cost = cost;
    }

    public string ToJson() {
        var sp = new StringBuilder();
        sp.Append("{");
        sp.Append($"\"id\": \"{id}\",");
        sp.Append($"\"title\": \"{title}\",");
        sp.Append($"\"cost\": {cost}");
        sp.Append("}");
        return sp.ToString();
    }

    public static Meal FromJson(string jsonString) {
        var jsonValidator = new JsonValidator();
        var result = jsonValidator.Validate(jsonString);
        if (!result.IsValid) {
            throw new Exception(result.Errors[0].ErrorMessage);
        }

        var normalizedJson = jsonString.Replace(" ", "");
        normalizedJson = normalizedJson.Replace("\n", "");
        normalizedJson = normalizedJson.Replace("\"", "");
        normalizedJson = normalizedJson[1..^1];
        
        var jsonArray = normalizedJson.Split(',');

        Meal newMeal = new Meal();
        
        
        for (int i = 0; i < jsonArray.Length; i++) {
            var obj = jsonArray[i].Split(":");
            switch (obj[0]) {
                case "id":
                    newMeal.id = obj[1];
                    break;
                case "title":
                    newMeal.title = obj[1];
                    break;
                case "cost":
                    newMeal.cost = int.Parse(obj[1]);
                    break;
                default:
                    throw new Exception($"invalid key: {obj[0]}");
            }
        }
        return newMeal;
    }

    public override string ToString() {
        var sb = new StringBuilder();
        sb.AppendLine("MEAL:");
        sb.AppendLine($"\"id\": \"{id}\",");
        sb.AppendLine($"\"title\": \"{title}\",");
        sb.AppendLine($"\"cost\": \"{cost}\"");
        return sb.ToString();
    }
}