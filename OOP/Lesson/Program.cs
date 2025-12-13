using Lesson.Entities;
using Lesson.Utils;

public class Program {
    public static void Main(string[] args) {
        
        // Чтение списка объектов из json
        var mealList = JsonFileHelper.ReadJsonItemsList<Meal>("../../../Menu1.json");
        Console.WriteLine("---> СПИСОК МЕНЮ <---");
        foreach (var item in mealList) {
            Console.WriteLine(item.ToString());
        }
        
        // Чтение массива Orders
        var orderList = JsonFileHelper.ReadJsonItemsList<Order>("../../../Orders1.json");
        Console.WriteLine();
        Console.WriteLine("---> СПИСОК ЗАКАЗОВ <---");
        foreach (var order in orderList) {
            Console.WriteLine(order.ToString());
            Console.WriteLine("--------------");
        }
    }
}