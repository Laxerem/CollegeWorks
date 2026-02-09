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
        var orderRegistry = new OrderRegistry("Orders1.json");
        var mealRegistry = new MealsRegister("Menu1.json");

        var clientController = new ClientController(orderRegistry, mealRegistry);

        var sockServer = new SockServer(4004, new ClientDispatcher(clientController));
        await sockServer.StartAsync();
    }
}
