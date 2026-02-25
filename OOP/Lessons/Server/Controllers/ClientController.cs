using Server.Database.Entities;
using Server.Database.Repositories;
using Server.Entities;
using Server.Exceptions;

namespace Server.Controllers;

public class ClientController {
    private readonly MealRepository _mealRepository;
    private readonly OrderRepository _orderRepository;

    public ClientController(MealRepository mealRepository, OrderRepository orderRepository) {
        _mealRepository = mealRepository;
        _orderRepository = orderRepository;
    }

    public List<Meal> GetAllMeals() => _mealRepository.GetAll();

    public Meal GetMealById(int id) {
        var meal = _mealRepository.GetById(id);
        if (meal == null) throw new NotFoundException($"Meal with id '{id}' not found");
        return meal;
    }

    public Meal AddMeal(string title, int cost) => _mealRepository.Create(title, cost);

    public bool DeleteMeal(int id) {
        if (_mealRepository.GetById(id) == null) throw new NotFoundException($"Meal with id '{id}' not found");
        return _mealRepository.DeleteById(id);
    }

    public List<Order> GetAllOrders() => _orderRepository.GetAll();

    public Order GetOrderById(int id) {
        var order = _orderRepository.GetById(id);
        if (order == null) throw new NotFoundException($"Order with id '{id}' not found");
        return order;
    }

    public Order AddOrder(string studentId, string date, List<int> mealIds) =>
        _orderRepository.Create(studentId, date, mealIds);

    public bool DeleteOrder(int id) {
        if (_orderRepository.GetById(id) == null) throw new NotFoundException($"Order with id '{id}' not found");
        return _orderRepository.DeleteById(id);
    }
}
