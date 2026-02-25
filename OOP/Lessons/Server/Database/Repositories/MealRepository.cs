using Npgsql;
using Server.Database.Common;
using Server.Database.Entities;

namespace Server.Database.Repositories;

public class MealRepository : IRepository {
    private readonly DbContext _context;

    public MealRepository(DbContext context) {
        _context = context;
    }

    public void Initialize() {
        using var conn = _context.GetConnection();
        conn.Open();
        string sql = "CREATE TABLE IF NOT EXISTS Meals (Id SERIAL PRIMARY KEY, Title TEXT NOT NULL, Cost INTEGER NOT NULL)";
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.ExecuteNonQuery();
        Console.WriteLine("Meals repository initialized");
    }

    public void Delete() {
        _context.ExecuteWork("DROP TABLE IF EXISTS Meals");
    }

    public Meal Create(string title, int cost) {
        Meal? meal = null;
        var parameters = new List<NpgsqlParameter> {
            new NpgsqlParameter("title", title),
            new NpgsqlParameter("cost", cost)
        };
        _context.ExecuteWork(
            "INSERT INTO Meals (Title, Cost) VALUES (@title, @cost) RETURNING *",
            parameters,
            reader => {
                if (reader.Read()) {
                    meal = ReadMeal(reader);
                }
            }
        );
        return meal!;
    }

    public List<Meal> GetAll() {
        var meals = new List<Meal>();
        _context.ExecuteWork("SELECT * FROM Meals", null, reader => {
            while (reader.Read()) {
                meals.Add(ReadMeal(reader));
            }
        });
        return meals;
    }

    public Meal? GetById(int id) {
        Meal? meal = null;
        var parameters = new List<NpgsqlParameter> { new NpgsqlParameter("id", id) };
        _context.ExecuteWork("SELECT * FROM Meals WHERE Id = @id", parameters, reader => {
            if (reader.Read()) {
                meal = ReadMeal(reader);
            }
        });
        return meal;
    }

    public bool DeleteById(int id) {
        bool deleted = false;
        var parameters = new List<NpgsqlParameter> { new NpgsqlParameter("id", id) };
        _context.ExecuteWork("DELETE FROM Meals WHERE Id = @id RETURNING Id", parameters, reader => {
            deleted = reader.Read();
        });
        return deleted;
    }

    private static Meal ReadMeal(NpgsqlDataReader reader) {
        return new Meal(
            reader.GetInt32(reader.GetOrdinal("Id")),
            reader.GetString(reader.GetOrdinal("Title")),
            reader.GetInt32(reader.GetOrdinal("Cost"))
        );
    }
}
