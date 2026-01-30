using System.Net.Sockets;
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
        response = JsonSerializer.Serialize(new ServerResponse(400, "Unknown error."));
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
                        Console.WriteLine("КОМАНДА              ПАРАМЕТРЫ    ОПИСАНИЕ");
                        Console.WriteLine("──────────────────────────────────────────────────────────");
                        Console.WriteLine("get_orders           —            Получить список всех заказов");
                        Console.WriteLine("get_meals            —            Получить список всех блюд");
                        Console.WriteLine("get_order_by_id      <id>         Получить заказ по ID");
                        Console.WriteLine("get_meal_by_id       <id>         Получить блюдо по ID");
                        Console.WriteLine("exit                 —            Отключиться от сервера");
                        Console.WriteLine("stop                 —            Остановить сервер");
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
                        var order = _controller.GetOrderById(tokens[1]);
                        var orderResponse = order.ToJson();

                        await streamWriter.WriteLineAsync(orderResponse);
                        break;
                    case "get_meal_by_id":
                        var meal = _controller.GetMealById(tokens[1]);
                        var mealResponse = meal.ToJson();

                        await streamWriter.WriteLineAsync(mealResponse);
                        break;
                    case "add_meal":
                        var mealJson = string.Join(" ", tokens.Skip(1));
                        var addMealResult = _controller.AddMeal(mealJson);
                        await streamWriter.WriteLineAsync(addMealResult ? "Meal added" : "Failed to add meal");
                        break;
                    case "add_order":
                        var orderJson = string.Join(" ", tokens.Skip(1));
                        var addOrderResult = _controller.AddOrder(orderJson);
                        await streamWriter.WriteLineAsync(addOrderResult ? "Order added" : "Failed to add order");
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