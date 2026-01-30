using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;
using Lesson.Utils;
using Objects;
using Server.Controllers;
using Server.Dispatcher;
using Server.Entities;
using Server.Registers;

public class Program {
    public async static Task Main(string[] args) {
        var orders = JsonFileHelper.ReadJsonItemsList<Order>("Orders1.json");
        var meals = JsonFileHelper.ReadJsonItemsList<Meal>("Menu1.json");
        var orderRegistry = new OrderRegistry(orders);
        var mealRegistry = new MealsRegister(meals);
        
        var clientController = new ClientController(orderRegistry, mealRegistry);
        
        var sockServer = new SockServer(4004, new ClientDispatcher(clientController));
        await sockServer.StartAsync();
    }
}