using Npgsql;
using Server.Database.Common;
using Server.Database.Entities;
using Server.Entities;

namespace Server.Database.Repositories;

public class OrderRepository : IRepository {
    private readonly DbContext _context;
    private readonly MealRepository _mealRepository;

    public OrderRepository(DbContext context, MealRepository mealRepository) {
        _context = context;
        _mealRepository = mealRepository;
    }

    public void Initialize() {
        using var conn = _context.GetConnection();
        conn.Open();

        string createOrders = "CREATE TABLE IF NOT EXISTS Orders (Id SERIAL PRIMARY KEY, StudentId TEXT NOT NULL, Date TEXT NOT NULL)";
        using (var cmd = new NpgsqlCommand(createOrders, conn)) {
            cmd.ExecuteNonQuery();
        }

        string createOrderMeals = @"
            CREATE TABLE IF NOT EXISTS OrderMeals (
                OrderId INTEGER NOT NULL REFERENCES Orders(Id) ON DELETE CASCADE,
                MealId  INTEGER NOT NULL REFERENCES Meals(Id)  ON DELETE CASCADE,
                PRIMARY KEY (OrderId, MealId)
            )";
        using (var cmd = new NpgsqlCommand(createOrderMeals, conn)) {
            cmd.ExecuteNonQuery();
        }

        Console.WriteLine("Orders repository initialized");
    }

    public void Delete() {
        _context.ExecuteWork("DROP TABLE IF EXISTS OrderMeals");
        _context.ExecuteWork("DROP TABLE IF EXISTS Orders");
    }

    public Order Create(string studentId, string date, List<int> mealIds) {
        int orderId = 0;
        var insertParams = new List<NpgsqlParameter> {
            new NpgsqlParameter("studentId", studentId),
            new NpgsqlParameter("date", date)
        };
        _context.ExecuteWork(
            "INSERT INTO Orders (StudentId, Date) VALUES (@studentId, @date) RETURNING Id",
            insertParams,
            reader => {
                if (reader.Read()) orderId = reader.GetInt32(0);
            }
        );

        foreach (var mealId in mealIds) {
            var linkParams = new List<NpgsqlParameter> {
                new NpgsqlParameter("orderId", orderId),
                new NpgsqlParameter("mealId", mealId)
            };
            _context.ExecuteWork(
                "INSERT INTO OrderMeals (OrderId, MealId) VALUES (@orderId, @mealId)",
                linkParams
            );
        }

        var meals = mealIds
            .Select(id => _mealRepository.GetById(id))
            .Where(m => m != null)
            .Cast<Meal>()
            .ToList();

        return Order.Create(orderId, date, studentId, meals);
    }

    public List<Order> GetAll() {
        var orderDict = new Dictionary<int, Order>();

        string sql = @"
            SELECT o.Id, o.StudentId, o.Date, m.Id AS MealId, m.Title, m.Cost
            FROM Orders o
            LEFT JOIN OrderMeals om ON o.Id = om.OrderId
            LEFT JOIN Meals m       ON om.MealId = m.Id
            ORDER BY o.Id";

        _context.ExecuteWork(sql, null, reader => {
            while (reader.Read()) {
                int orderId = reader.GetInt32(reader.GetOrdinal("Id"));
                if (!orderDict.ContainsKey(orderId)) {
                    orderDict[orderId] = Order.Create(
                        orderId,
                        reader.GetString(reader.GetOrdinal("Date")),
                        reader.GetString(reader.GetOrdinal("StudentId")),
                        new List<Meal>()
                    );
                }
                if (!reader.IsDBNull(reader.GetOrdinal("MealId"))) {
                    orderDict[orderId].Meals.Add(ReadMeal(reader));
                }
            }
        });

        return orderDict.Values.ToList();
    }

    public Order? GetById(int id) {
        Order? order = null;
        var parameters = new List<NpgsqlParameter> { new NpgsqlParameter("id", id) };

        string sql = @"
            SELECT o.Id, o.StudentId, o.Date, m.Id AS MealId, m.Title, m.Cost
            FROM Orders o
            LEFT JOIN OrderMeals om ON o.Id = om.OrderId
            LEFT JOIN Meals m       ON om.MealId = m.Id
            WHERE o.Id = @id";

        _context.ExecuteWork(sql, parameters, reader => {
            while (reader.Read()) {
                if (order == null) {
                    order = Order.Create(
                        id,
                        reader.GetString(reader.GetOrdinal("Date")),
                        reader.GetString(reader.GetOrdinal("StudentId")),
                        new List<Meal>()
                    );
                }
                if (!reader.IsDBNull(reader.GetOrdinal("MealId"))) {
                    order.Meals.Add(ReadMeal(reader));
                }
            }
        });

        return order;
    }

    public bool DeleteById(int id) {
        bool deleted = false;
        var parameters = new List<NpgsqlParameter> { new NpgsqlParameter("id", id) };
        _context.ExecuteWork("DELETE FROM Orders WHERE Id = @id RETURNING Id", parameters, reader => {
            deleted = reader.Read();
        });
        return deleted;
    }

    private static Meal ReadMeal(NpgsqlDataReader reader) {
        return new Meal(
            reader.GetInt32(reader.GetOrdinal("MealId")),
            reader.GetString(reader.GetOrdinal("Title")),
            reader.GetInt32(reader.GetOrdinal("Cost"))
        );
    }
}
