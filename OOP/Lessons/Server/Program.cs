using Server.Database;
using Server.Database.Repositories;

public class Program {
    public async static Task Main(string[] args) {
        // var orderRegistry = new OrderRegistry("Orders1.json");
        // var mealRegistry = new MealsRegister("Menu1.json");
        //
        // var clientController = new ClientController(orderRegistry, mealRegistry);
        //
        // var sockServer = new SockServer(4004, new ClientDispatcher(clientController));
        // await sockServer.StartAsync();
        string connectionString = "UserID=postgres;Password=12345;Host=localhost;Port=5432;Database=lesson;";
        var dbContext = new DbContext(connectionString);
        var userRepository = new UserRepository(dbContext);
        userRepository.Initialize();
        var users = userRepository.GetAll();
        
        foreach (var user in users) {
            Console.WriteLine(user.ToString());
        }
    }
}
