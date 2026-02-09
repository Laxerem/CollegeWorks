using System.Text;
using Server.Interfaces;
using Server.Utils;

namespace Server.Entities;

public class Meal : IJsonable<Meal> {
    public string? Id { get; private set; }
    public string? title { get; private set; }
    public int? cost { get; private set; }

    private Meal() {
        this.Id = "none";
        this.title = "none";
        this.cost = 0;
    }

    public Meal(string? id, string? title, int? cost) {
        this.Id = id;
        this.title = title;
        this.cost = cost;
    }

    public string ToJson() {
        var sp = new StringBuilder();
        sp.Append("{");
        sp.Append($"\"id\": \"{Id}\",");
        sp.Append($"\"title\": \"{title}\",");
        sp.Append($"\"cost\": {cost}");
        sp.Append("}");
        return sp.ToString();
    }

    public static Meal? FromJson(string jsonString) {
        // Валидируем и нормализуем JSON
        var normalizedJson = JsonHelper.NormalizeJson(jsonString);

        // Убираем внешние фигурные скобки и кавычки
        normalizedJson = normalizedJson[1..^1].Replace("\"", "");

        var jsonArray = normalizedJson.Split(',');

        string? mealId = null;
        string? mealTitle = null;
        int? mealCost = null;

        for (int i = 0; i < jsonArray.Length; i++) {
            var obj = jsonArray[i].Split(":");
            var key = obj[0].Trim();
            var value = obj.Length > 1 ? string.Join(":", obj.Skip(1)).Trim() : "";

            switch (key) {
                case "id":
                    mealId = value;
                    break;
                case "title":
                    mealTitle = value;
                    break;
                case "cost":
                    try {
                        mealCost = int.Parse(value);
                    }
                    catch (Exception) {
                        mealCost = null;
                    }
                    break;
                default:
                    return null;
            }
        }

        return new Meal(mealId, mealTitle, mealCost.Value);
    }

    public override string ToString() {
        return $"  • {(Id != null ? $"\"{Id}\"" : "null")}: {(title != null ? $"\"{title}\"" : "null")} — {(cost != null ? $"\"{cost}\"" : "null")} руб";
    }
}