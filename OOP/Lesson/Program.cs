using System.Collections.Immutable;
using System.Xml;
using Lesson.Entities;
using Lesson.Interfaces;
using Lesson.Utils;

public class Program {
    public static void Main(string[] args) {
        var meal = new Meal("I100", "пицца", Int32.MaxValue);
        var meal2 = new Meal("I200", "заяц", Int32.MaxValue);
        
        // Сериализация объекта в Json
        var json = meal.ToJson();
        JsonFileHelper.WriteJson("../../../answer1.json", meal);

        // Диссериализация объекта из json
        var meal3 = Meal.FromJson(json);
        Console.WriteLine(meal3.ToJson());

        // Запись массива объектов в Json
        var mealList = new List<IBaseJsonable>() {meal, meal2,  meal3};
        JsonFileHelper.WriteJsonItemsList("../../../answer2.json", mealList);
        
        // Чтение списка объектов из json
        var meallist = JsonFileHelper.ReadJsonItemsList<Meal>("../../../answer2.json");
        Console.WriteLine();
        foreach (var item in meallist) {
            Console.WriteLine(item.ToString());
        }
        
        // Запись meal-ов в Json как order
        var orders = new List<IBaseJsonable>() {Order.Create("петя", meallist), Order.Create("пупкин", meallist)};
        JsonFileHelper.WriteJsonItemsList("../../../answer3.json", orders);
        
        // Чтение order-ов как json
        var orderList = JsonFileHelper.ReadJsonItemsList<Order>("../../../answer3.json");
        foreach (var item in orderList) {
            Console.WriteLine(item.ToString());
        }
    }
}