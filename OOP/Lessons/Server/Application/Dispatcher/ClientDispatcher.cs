using System.Net.Sockets;
using Objects.Dto;
using Objects.Exceptions;
using Server.Controllers;
using Server.Interfaces;
using Server.Utils;

namespace Server.Dispatcher;

public class ClientDispatcher {
    private readonly ClientController _controller;
    public event Action? StopCommand;

    public ClientDispatcher(ClientController controller) {
        _controller = controller;
    }

    public async Task StartDispatchAsync(TcpClient client) {
        var stream = client.GetStream();
        using var reader = new StreamReader(stream);
        using var writer = new StreamWriter(stream) { AutoFlush = true };

        try {
            while (true) {
                var message = await reader.ReadLineAsync();
                if (message == null) break;

                Console.WriteLine("MESSAGE: " + message);

                var parts = message.Split(' ', 2);
                var command = parts[0];
                var json = parts.Length > 1 ? parts[1] : "";

                await HandleCommand(command, json, writer);
            }
        } catch (Exception ex) {
            await SendException(ex, writer);
        } finally {
            client.Close();
        }
    }

    private async Task HandleCommand(string command, string json, StreamWriter writer) {
        switch (command) {
            case "help":
                await writer.WriteLineAsync(GetHelpText());
                break;

            case "get_meals": {
                var meals = _controller.GetAllMeals();
                await writer.WriteLineAsync(SerializeList(meals));
                break;
            }

            case "get_meal_by_id": {
                try {
                    var id = JsonHelper.ReadIntByKey("id", json)
                        ?? throw new ArgumentException("Missing 'id' in request");
                    var meal = _controller.GetMealById(id);
                    await writer.WriteLineAsync(JsonHelper.Serialize(meal));
                } catch (Exception ex) {
                    await SendException(ex, writer);
                }
                break;
            }

            case "add_meal": {
                try {
                    var title = JsonHelper.ReadStringByKey("Title", json)
                        ?? throw new ArgumentException("Missing 'Title' in request");
                    var cost = JsonHelper.ReadIntByKey("Cost", json)
                        ?? throw new ArgumentException("Missing 'Cost' in request");
                    var meal = _controller.AddMeal(title, cost);
                    await writer.WriteLineAsync(JsonHelper.Serialize(meal));
                } catch (Exception ex) {
                    await SendException(ex, writer);
                }
                break;
            }

            case "delete_meal": {
                try {
                    var id = JsonHelper.ReadIntByKey("id", json)
                        ?? throw new ArgumentException("Missing 'id' in request");
                    _controller.DeleteMeal(id);
                    await SendResponse(200, "Meal deleted successfully", writer);
                } catch (Exception ex) {
                    await SendException(ex, writer);
                }
                break;
            }

            case "get_orders": {
                var orders = _controller.GetAllOrders();
                await writer.WriteLineAsync(SerializeList(orders));
                break;
            }

            case "get_order_by_id": {
                try {
                    var id = JsonHelper.ReadIntByKey("id", json)
                        ?? throw new ArgumentException("Missing 'id' in request");
                    var order = _controller.GetOrderById(id);
                    await writer.WriteLineAsync(JsonHelper.Serialize(order));
                } catch (Exception ex) {
                    await SendException(ex, writer);
                }
                break;
            }

            case "add_order": {
                try {
                    var studentId = JsonHelper.ReadStringByKey("StudentID", json)
                        ?? throw new ArgumentException("Missing 'StudentID' in request");
                    var date = JsonHelper.ReadStringByKey("date", json)
                        ?? throw new ArgumentException("Missing 'date' in request");
                    var mealIds = JsonHelper.ReadIntArrayByKey("mealIds", json);
                    var order = _controller.AddOrder(studentId, date, mealIds);
                    await writer.WriteLineAsync(JsonHelper.Serialize(order));
                } catch (Exception ex) {
                    await SendException(ex, writer);
                }
                break;
            }

            case "delete_order": {
                try {
                    var id = JsonHelper.ReadIntByKey("id", json)
                        ?? throw new ArgumentException("Missing 'id' in request");
                    _controller.DeleteOrder(id);
                    await SendResponse(200, "Order deleted successfully", writer);
                } catch (Exception ex) {
                    await SendException(ex, writer);
                }
                break;
            }

            case "exit":
                return;

            case "stop":
                StopCommand?.Invoke();
                return;

            default:
                await SendResponse(400, $"Unknown command: '{command}'", writer);
                break;
        }
    }

    private static string SerializeList<T>(List<T> items) where T : IBaseJsonable {
        if (items.Count == 0) return "[]";
        return JsonHelper.SerializeJsonList(items.Cast<IBaseJsonable>());
    }

    private static async Task SendResponse(int code, string message, StreamWriter writer) {
        await writer.WriteLineAsync(JsonHelper.Serialize(new ServerResponse(code, message)));
    }

    private static async Task SendException(Exception ex, StreamWriter writer) {
        if (ex is BaseException baseEx) {
            await SendResponse(baseEx.Code, baseEx.Message, writer);
            return;
        }
        Console.WriteLine($"ERROR: {ex.GetType().Name}: {ex.Message}");
        Console.WriteLine($"STACK TRACE: {ex.StackTrace}");
        await SendResponse(400, $"Unknown error: {ex.Message}", writer);
    }

    private static string GetHelpText() => """
        КОМАНДА              ПАРАМЕТРЫ                                                                ОПИСАНИЕ
        ────────────────────────────────────────────────────────────────────────────────────────────────────────
        get_meals            —                                                                        Получить список всех блюд
        get_meal_by_id       {"id": <id>}                                                             Получить блюдо по ID
        add_meal             {"Title": "<Title>", "Cost": <Cost>}                                     Добавить новое блюдо
        delete_meal          {"id": <id>}                                                             Удалить блюдо по ID
        get_orders           —                                                                        Получить список всех заказов
        get_order_by_id      {"id": <id>}                                                             Получить заказ по ID
        add_order            {"StudentID": "<id>", "date": "<date>", "mealIds": [<id1>, <id2>]}      Добавить новый заказ
        delete_order         {"id": <id>}                                                             Удалить заказ по ID
        exit                 —                                                                        Отключиться от сервера
        stop                 —                                                                        Остановить сервер
        """;
}
