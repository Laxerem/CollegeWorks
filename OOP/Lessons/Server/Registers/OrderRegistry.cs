using Server.Entities;

namespace Server.Registers;

public class OrderRegistry {
    private Dictionary<string, Order> _orders;

    public OrderRegistry(List<Order> orders) {
        _orders = orders.ToDictionary(order => order.StudentId);
    }

    public Order? GetOrder(string studentId) {
        return _orders.TryGetValue(studentId, out var order) ? order : null;
    }

    public List<Order> GetAllOrders() {
        return _orders.Values.ToList();
    }

    public void AddOrder(Order order) {
        _orders[order.StudentId] = order;
    }
}