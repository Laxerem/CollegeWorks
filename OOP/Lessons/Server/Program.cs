using Objects;
using Server.Controllers;
using Server.Database;
using Server.Database.Repositories;
using Server.Dispatcher;

string connectionString = "UserID=postgres;Password=12345;Host=localhost;Port=5432;Database=lesson;";

var dbContext = new DbContext(connectionString);

var mealRepository = new MealRepository(dbContext);
var orderRepository = new OrderRepository(dbContext, mealRepository);

mealRepository.Initialize();
orderRepository.Initialize();

var controller = new ClientController(mealRepository, orderRepository);
var dispatcher = new ClientDispatcher(controller);
var server = new SockServer(4004, dispatcher);

await server.StartAsync();
