using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Objects.Dto;
using Objects.Exceptions;
using Server.Controllers;
using Server.Entities;
using Server.Utils;

namespace Server.Dispatcher;

public class ClientDispatcher {
    private readonly ClientController _controller;
    public event Action StopCommand;
    
    public ClientDispatcher(ClientController controller) {
        _controller = controller;
    }

    private async Task SendException(Exception ex, StreamWriter streamWriter) {
        var response = string.Empty;
        if (ex is BaseException) {
            var exception = (BaseException)ex;
            response = JsonSerializer.Serialize(new ServerResponse(exception.Code, exception.Message));
            await streamWriter.WriteLineAsync(response);
            return;
        }
        Console.WriteLine($"ERROR: {ex.GetType().Name}: {ex.Message}");
        Console.WriteLine($"STACK TRACE: {ex.StackTrace}");
        response = JsonSerializer.Serialize(new ServerResponse(400, $"Unknown error: {ex.Message}"));
        await streamWriter.WriteLineAsync(response);
    }

    public async Task StartDispatchAsync(TcpClient client) {
        var stream = client.GetStream();
        using var streamReader = new StreamReader(stream);
        using var streamWriter = new StreamWriter(stream);
        streamWriter.AutoFlush = true;

        try {
            while (true) {
                var message = await streamReader.ReadLineAsync();
                Console.WriteLine("MESSAGE: " + message);
                
                var tokens =  message.Split(' ');
                
                switch (tokens[0]) {
                    case "help":
                        Console.WriteLine("КОМАНДА              ПАРАМЕТРЫ                                           ОПИСАНИЕ");
                        Console.WriteLine("─────────────────────────────────────────────────────────────────────────────────────────");
                        Console.WriteLine("get_orders           —                                                   Получить список всех заказов");
                        Console.WriteLine("get_meals            —                                                   Получить список всех блюд");
                        Console.WriteLine("get_order_by_id      {\"id\": \"<id>\"}                                     Получить заказ по ID");
                        Console.WriteLine("get_meal_by_id       {\"id\": \"<id>\"}                                     Получить блюдо по ID");
                        Console.WriteLine("add_meal             {\"id\": \"<id>\", \"title\": \"<title>\", \"cost\": <cost>}   Добавить новое блюдо");
                        Console.WriteLine("add_order            {\"StudentID\": \"<id>\", \"date\": \"<date>\", \"meals\": [...]}  Добавить новый заказ");
                        Console.WriteLine("delete_meal          {\"id\": \"<id>\"}                                     Удалить блюдо по ID");
                        Console.WriteLine("delete_order         {\"id\": \"<id>\"}                                     Удалить заказ по ID");
                        Console.WriteLine("exit                 —                                                   Отключиться от сервера");
                        Console.WriteLine("stop                 —                                                   Остановить сервер");
                        await streamWriter.WriteLineAsync("");
                        break;
                    case "get_orders":
                        var orders = _controller.GetAllOrders();
                        var stringResponse1 = JsonHelper.SerializeJsonList(orders);
                        
                        await streamWriter.WriteLineAsync(stringResponse1);
                        break;
                    case "get_meals":
                        var meals = _controller.GetAllMeals();
                        var stringResponse2 = JsonHelper.SerializeJsonList(meals);

                        await streamWriter.WriteLineAsync(stringResponse2);
                        break;
                    case "get_order_by_id":
                        try {
                            var orderJsonStr = string.Join(" ", tokens.Skip(1));
                            using (var orderDoc = JsonDocument.Parse(orderJsonStr)) {
                                var orderId = orderDoc.RootElement.GetProperty("id").GetString();
                                var order = _controller.GetOrderById(orderId);
                                var orderResponse = order.ToJson();

                                await streamWriter.WriteLineAsync(orderResponse);
                            }
                        }
                        catch (Exception ex) {
                            await SendException(ex, streamWriter);
                        }
                        break;
                    case "get_meal_by_id":
                        try {
                            var mealJsonStr = string.Join(" ", tokens.Skip(1));
                            using (var mealDoc = JsonDocument.Parse(mealJsonStr)) {
                                var mealId = mealDoc.RootElement.GetProperty("id").GetString();
                                var meal = _controller.GetMealById(mealId);
                                var mealResponse = meal.ToJson();

                                await streamWriter.WriteLineAsync(mealResponse);
                            }
                        }
                        catch (Exception ex) {
                            await SendException(ex, streamWriter);
                        }
                        break;
                    case "add_meal":
                        try {
                            var mealJson = string.Join(" ", tokens.Skip(1));
                            var addMealResult = _controller.AddMeal(mealJson);
                            await streamWriter.WriteLineAsync(addMealResult ? "Meal added successfully" : "Failed to add meal");
                        }
                        catch (Exception ex) {
                            await SendException(ex, streamWriter);
                        }
                        break;
                    case "add_order":
                        try {
                            var orderJson = string.Join(" ", tokens.Skip(1));
                            var addOrderResult = _controller.AddOrder(orderJson);
                            await streamWriter.WriteLineAsync(addOrderResult ? "Order added successfully" : "Failed to add order");
                        }
                        catch (Exception ex) {
                            await SendException(ex, streamWriter);
                        }
                        break;
                    case "delete_meal":
                        try {
                            var deleteMealJsonStr = string.Join(" ", tokens.Skip(1));
                            using (var deleteMealDoc = JsonDocument.Parse(deleteMealJsonStr)) {
                                var deleteMealId = deleteMealDoc.RootElement.GetProperty("id").GetString();
                                var deleteMealResult = _controller.DeleteMeal(deleteMealId);
                                await streamWriter.WriteLineAsync(deleteMealResult ? "Meal deleted successfully" : "Failed to delete meal");
                            }
                        }
                        catch (Exception ex) {
                            await SendException(ex, streamWriter);
                        }
                        break;
                    case "delete_order":
                        try {
                            var deleteOrderJsonStr = string.Join(" ", tokens.Skip(1));
                            using (var deleteOrderDoc = JsonDocument.Parse(deleteOrderJsonStr)) {
                                var deleteOrderId = deleteOrderDoc.RootElement.GetProperty("id").GetString();
                                var deleteOrderResult = _controller.DeleteOrder(deleteOrderId);
                                await streamWriter.WriteLineAsync(deleteOrderResult ? "Order deleted successfully" : "Failed to delete order");
                            }
                        }
                        catch (Exception ex) {
                            await SendException(ex, streamWriter);
                        }
                        break;
                    case "exit":
                        return;
                    case "stop":
                        StopCommand?.Invoke();
                        return;
                    default:
                        await streamWriter.WriteLineAsync(message);
                        break;
                }
            }
        }
        catch (Exception ex) {
            await SendException(ex, streamWriter);
        }
        finally {
            client.Close();
        }
    }
}