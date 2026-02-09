using Lesson.Utils;
using Server.Entities;
using Server.Exceptions;
using Server.Registers;
using Server.Utils;

namespace Server.Controllers;

/// <summary>
/// Контроллер для клиента
/// </summary>
public class ClientController {
    private readonly OrderRegistry _orders;
    private readonly MealsRegister _meals;

    public ClientController(OrderRegistry orders, MealsRegister meals) {
        _orders = orders;
        _meals = meals;
    }
    
    public bool CreateOrder(string student, string mealsJsonList) {
        var meals = JsonHelper.DeserializeJsonList<Meal>(mealsJsonList);
        
        var order = Order.Create(student, meals);
        JsonFileHelper.WriteJson("../../../menu.json", order);
        return true;
    }

    public Order GetMenu(string studentId) {
        var order = _orders.GetOrder(studentId);
        if (order == null) {
            throw new NotFoundException("Menu not found");
        }
        return order;
    }

    public List<Order> GetAllOrders() {
        return _orders.GetAllOrders();
    }

    public List<Meal> GetAllMeals() {
        return _meals.GetAllMeals();
    }

    public Order GetOrderById(string studentId) {
        var order = _orders.GetOrder(studentId);
        if (order == null) {
            throw new NotFoundException("Order not found");
        }
        return order;
    }

    public Meal GetMealById(string mealId) {
        var meal = _meals.GetMeal(mealId);
        if (meal == null) {
            throw new NotFoundException("Meal not found");
        }
        return meal;
    }

    public bool AddMeal(string mealJson) {
        var meal = Meal.FromJson(mealJson);
        if (meal == null) {
            throw new ArgumentException("Invalid meal JSON");
        }

        // Проверяем, существует ли блюдо с таким ID
        if (meal.Id != null && _meals.GetMeal(meal.Id) != null) {
            throw new ArgumentException($"Meal with ID '{meal.Id}' already exists");
        }

        _meals.AddMeal(meal);
        JsonFileHelper.AppendJsonToList("Menu1.json", meal);
        return true;
    }

    public bool AddOrder(string orderJson) {
        var order = Order.FromJson(orderJson);
        if (order == null) {
            throw new ArgumentException("Invalid order JSON");
        }

        // Проверяем, существует ли заказ с таким StudentID
        if (_orders.GetOrder(order.StudentId) != null) {
            throw new ArgumentException($"Order for student '{order.StudentId}' already exists");
        }

        _orders.AddOrder(order);
        JsonFileHelper.AppendJsonToList("Orders1.json", order);
        return true;
    }

    public bool DeleteMeal(string mealId) {
        var meal = _meals.GetMeal(mealId);
        if (meal == null) {
            throw new NotFoundException($"Meal with ID '{mealId}' not found");
        }

        _meals.RemoveMeal(mealId);
        // Перезаписываем файл со всеми оставшимися блюдами
        JsonFileHelper.WriteJsonItemsList("Menu1.json", _meals.GetAllMeals());
        return true;
    }

    public bool DeleteOrder(string studentId) {
        var order = _orders.GetOrder(studentId);
        if (order == null) {
            throw new NotFoundException($"Order for student '{studentId}' not found");
        }

        _orders.RemoveOrder(studentId);
        // Перезаписываем файл со всеми оставшимися заказами
        JsonFileHelper.WriteJsonItemsList("Orders1.json", _orders.GetAllOrders());
        return true;
    }
}