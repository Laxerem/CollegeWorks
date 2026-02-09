using Server.Entities;
using Lesson.Utils;

namespace Server.Registers;

public class OrderRegistry {
    private Dictionary<string, Order> _orders;
    private readonly string _filePath;

    public OrderRegistry(string filePath) {
        _filePath = filePath;
        _orders = new Dictionary<string, Order>();
        LoadFromFile();
    }

    private void LoadFromFile() {
        try {
            if (!File.Exists(_filePath)) {
                // Создаём пустой файл с пустым JSON массивом
                JsonFileHelper.WriteJsonItemsList(_filePath, new List<Order>());
                return;
            }

            var orders = JsonFileHelper.ReadJsonItemsList<Order>(_filePath);
            foreach (var order in orders) {
                _orders[order.StudentId] = order;
            }
        }
        catch (Exception ex) {
            Console.WriteLine($"Ошибка при загрузке заказов из файла: {ex.Message}");
            // Если файл повреждён, создаём новый пустой
            _orders = new Dictionary<string, Order>();
        }
    }

    private void SaveToFile() {
        try {
            JsonFileHelper.WriteJsonItemsList(_filePath, _orders.Values.ToList());
        }
        catch (Exception ex) {
            Console.WriteLine($"Ошибка при сохранении заказов в файл: {ex.Message}");
        }
    }

    public Order? GetOrder(string studentId) {
        return _orders.TryGetValue(studentId, out var order) ? order : null;
    }

    public List<Order> GetAllOrders() {
        return _orders.Values.ToList();
    }

    public void AddOrder(Order order) {
        _orders[order.StudentId] = order;
        SaveToFile();
    }

    public bool RemoveOrder(string studentId) {
        var result = _orders.Remove(studentId);
        if (result) {
            SaveToFile();
        }
        return result;
    }
}