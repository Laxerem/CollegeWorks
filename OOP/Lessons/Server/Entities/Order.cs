using System.Text;
using Lesson.Utils;
using Server.Interfaces;
using Server.Utils;

namespace Server.Entities;

public class Order : IJsonable<Order> {
    public readonly string Date;
    public readonly string StudentId;
    public List<Meal> Meals;

    private Order(string date, string studentId, List<Meal> meals) {
        this.Date = date;
        this.StudentId = studentId;
        this.Meals = meals;
    }

    public static Order Create(string student, List<Meal> meals) {
        return new Order(
            date: "",
            studentId: student,
            meals: meals
        );
    }
    
    public static Order Create(string date, string studentId, List<Meal> meals) {
        return new Order(
            date: date,
            studentId: studentId,
            meals: meals
        );
    }

    public static Order? FromJson(string jsonString) {
        var normalizeJson = JsonHelper.NormalizeJson(jsonString)[1..^1];
        
        string? studentId = null;
        string? date = null;
        List<Meal> meals = new();
        
        string[] soughtElements = {"StudentID", "date", "meals"};

        foreach (var element in soughtElements) {
            var startNameIndex = normalizeJson.IndexOf(element, StringComparison.Ordinal);
            
            if (startNameIndex == -1) {
                continue;
            }
            
            var endNameIndex = IBaseJsonable.GetStringEndIndex(startNameIndex - 1, normalizeJson);
            
            if (!endNameIndex.HasValue) {
                return null;
            }

            if (!endNameIndex.HasValue) {
                throw new Exception();
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
                                throw new Exception();
                            }

                            var end = arrayEndIndex.Value + 1;
                            var str = normalizeJson[i..end];
                            meals = JsonHelper.DeserializeJsonList<Meal>(str);
                        }
                    }
                    break;
            }
        }

        if (date == null & studentId == null) {
            return null;
        }

        return Create(date,  studentId, meals);
    }

    public string ToJson() {
        var sp = new StringBuilder();
        sp.Append("{");
        sp.Append($"\"StudentID\": \"{StudentId}\",");
        sp.Append($"\"date\": \"{Date}\",");
        sp.Append($"\"meals\": {JsonHelper.SerializeJsonList(Meals)}");
        sp.Append("}");
        return sp.ToString();
    }

    public override string ToString() {
        var sb = new StringBuilder();
        sb.AppendLine($"Order: {(StudentId != null ? $"{StudentId}" : "null")} | {(Date != null ? $"{Date}" : "null")}");
        sb.AppendLine("─────────────────────────");
        if (Meals.Any()) {
            foreach (var meal in Meals) {
                sb.AppendLine(meal.ToString());
            }   
        }
        else {
            sb.AppendLine("null");
        }
        sb.AppendLine("─────────────────────────");
        sb.AppendLine($"Total: {Meals.Sum(m => m.cost):F2} рублей");
        return sb.ToString();
    }
}