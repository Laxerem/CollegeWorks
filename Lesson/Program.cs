using System.Xml;
using Lesson.Entities;
using Lesson.Interfaces;
using Lesson.Utils;

public class Program {
    public static void Main(string[] args) {
        var meal = new Meal("I100", "бургер", Int32.MaxValue);
        var meal2 = new Meal("I200", "мясо", Int32.MaxValue);
        
        var json = meal.ToJson();
        Console.WriteLine(json);
        
        JsonFileHelper.WriteJson("../../../answer1.json", meal);

        var meal3 = Meal.FromJson(json);
        Console.WriteLine(meal3.ToJson());

        var mealList = new List<IJsonable>() {meal, meal2,  meal3};
        
        BaseFileHelper.ClearFile("../../../answer2.json");
        JsonFileHelper.WriteJsonItemsList("../../../answer2.json", mealList);
    }
}