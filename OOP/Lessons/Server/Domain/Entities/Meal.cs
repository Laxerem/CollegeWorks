using System.Text;
using Server.Interfaces;
using Server.Utils;

namespace Server.Database.Entities;

public class Meal : IJsonable<Meal> {
    public int Id { get; private set; }
    public string Title { get; private set; }
    public int Cost { get; private set; }

    private Meal() {
        this.Title = "none";
        this.Cost = 0;
    }

    public Meal(int id, string title, int cost) {
        this.Id = id;
        this.Title = title;
        this.Cost = cost;
    }

    public string ToJson() {
        var sp = new StringBuilder();
        sp.Append("{");
        sp.Append($"\"id\": \"{Id}\",");
        sp.Append($"\"Title\": \"{Title}\",");
        sp.Append($"\"Cost\": {Cost}");
        sp.Append("}");
        return sp.ToString();
    }

    public static Meal? FromJson(string jsonString) {
        // Валидируем и нормализуем JSON
        var normalizedJson = JsonHelper.NormalizeJson(jsonString);

        // Убираем внешние фигурные скобки и кавычки
        normalizedJson = normalizedJson[1..^1].Replace("\"", "");

        var jsonArray = normalizedJson.Split(',');

        int? mealId = null;
        string? mealTitle = null;
        int? mealCost = null;

        for (int i = 0; i < jsonArray.Length; i++) {
            var obj = jsonArray[i].Split(":");
            var key = obj[0].Trim();
            var value = obj.Length > 1 ? string.Join(":", obj.Skip(1)).Trim() : "";

            switch (key) {
                case "Id":
                    mealId = int.Parse(value);
                    break;
                case "Title":
                    mealTitle = value;
                    break;
                case "Cost":
                    mealCost = int.Parse(value);
                    break;
                default:
                    return null;
            }
        }

        return new Meal(mealId.Value, mealTitle, mealCost.Value);
    }

    public override string ToString() {
        return $"  • {(Id != null ? $"\"{Id}\"" : "null")}: {(Title != null ? $"\"{Title}\"" : "null")} — {(Cost != null ? $"\"{Cost}\"" : "null")} руб";
    }
}