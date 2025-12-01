using System.Text;
using Lesson.Interfaces;

namespace Lesson.Entities;

public class Order : IJsonable<Order> {
    public readonly DateTime dateTime;
    public readonly string student;
    public readonly List<Meal> meals;

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
    
    public static Order FromJson(string jsonString) {
        throw new NotImplementedException();
    }

    public string ToJson() {
        var sp = new StringBuilder();
        sp.Append("{");
        sp.Append($"\"date\": \"{dateTime}\",");
        sp.Append($"\"student\": \"{student}\",");
        sp.Append($"\"meals\": [");
        for (int i = 0; i < meals.Count; i++) {
            sp.Append(meals[i].ToJson());
            if (i < meals.Count - 1) {
                sp.Append(",");
            }
        }
        sp.Append("]");
        sp.Append("}");
        return sp.ToString();
    }
}