using System.Text;
using Server.Database.Entities;
using Server.Interfaces;
using Server.Utils;

namespace Server.Entities;

public class Order : IJsonable<Order> {
    public int Id { get; private set; }
    public string Date { get; private set; }
    public string StudentId { get; private set; }
    public List<Meal> Meals;

    private Order(int id, string date, string studentId, List<Meal> meals) {
        Id = id;
        Date = date;
        StudentId = studentId;
        Meals = meals;
    }

    public static Order Create(int id, string date, string studentId, List<Meal> meals) {
        return new Order(id, date, studentId, meals);
    }

    public static Order Create(string date, string studentId, List<Meal> meals) {
        return new Order(0, date, studentId, meals);
    }

    public static Order? FromJson(string jsonString) {
        var normalizeJson = JsonHelper.NormalizeJson(jsonString)[1..^1];

        string? studentId = null;
        string? date = null;
        List<Meal> meals = new();

        string[] soughtElements = { "StudentID", "date", "meals" };

        foreach (var element in soughtElements) {
            var startNameIndex = normalizeJson.IndexOf(element, StringComparison.Ordinal);

            if (startNameIndex == -1) {
                continue;
            }

            var endNameIndex = IBaseJsonable.GetStringEndIndex(startNameIndex - 1, normalizeJson);

            if (!endNameIndex.HasValue) {
                return null;
            }

            var elementName = normalizeJson[startNameIndex..endNameIndex.Value];
            switch (elementName) {
                case "StudentID":
                    studentId = JsonHelper.ReadFirstStringValue(endNameIndex.Value + 1, normalizeJson);
                    break;
                case "date":
                    date = JsonHelper.ReadFirstStringValue(endNameIndex.Value + 1, normalizeJson);
                    break;
                case "meals":
                    for (int i = endNameIndex.Value; i < normalizeJson.Length; i++) {
                        if (normalizeJson[i] == '[') {
                            var arrayEndIndex = IBaseJsonable.GetElementEndIndex(i, '[', ']', normalizeJson);
                            if (!arrayEndIndex.HasValue) {
                                throw new Exception("Malformed meals array in JSON");
                            }
                            var end = arrayEndIndex.Value + 1;
                            meals = JsonHelper.DeserializeJsonList<Meal>(normalizeJson[i..end]);
                        }
                    }
                    break;
            }
        }

        if (date == null && studentId == null) {
            return null;
        }

        return Create(date!, studentId!, meals);
    }

    public string ToJson() {
        var sp = new StringBuilder();
        var mealsJson = Meals.Count > 0 ? JsonHelper.SerializeJsonList(Meals) : "[]";
        sp.Append("{");
        sp.Append($"\"id\": {Id},");
        sp.Append($"\"StudentID\": \"{StudentId}\",");
        sp.Append($"\"date\": \"{Date}\",");
        sp.Append($"\"meals\": {mealsJson}");
        sp.Append("}");
        return sp.ToString();
    }

    public override string ToString() {
        var sb = new StringBuilder();
        sb.AppendLine($"Order #{Id}: {StudentId} | {Date}");
        sb.AppendLine("─────────────────────────");
        if (Meals.Any()) {
            foreach (var meal in Meals) {
                sb.AppendLine(meal.ToString());
            }
        } else {
            sb.AppendLine("(no meals)");
        }
        sb.AppendLine("─────────────────────────");
        sb.AppendLine($"Total: {Meals.Sum(m => m.Cost):F2} рублей");
        return sb.ToString();
    }
}
