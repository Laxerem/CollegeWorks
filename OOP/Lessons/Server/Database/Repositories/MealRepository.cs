using Npgsql;
using NpgsqlTypes;
using Server.Database.Common;
using Server.Database.Entities;
using Server.Entities;

namespace Server.Database.Repositories;

public class MealRepository : IRepository {
    private readonly DbContext _context;
    
    public MealRepository(DbContext context) {
        _context = context;
    }
    
    public void Initialize() {
        try {
            using (var conn = _context.GetConnection()) {
                conn.Open();
                string sqlCreate = "CREATE TABLE IF NOT EXISTS Meals (Id SERIAL PRIMARY KEY, Title TEXT NOT NULL, Cost INTEGER NOT NULL)";
                using (var cmd = new NpgsqlCommand(sqlCreate, conn)) {
                    cmd.ExecuteNonQuery();
                }
                Console.WriteLine($"Meals repository initialized");
            }
        }
        catch (Exception e) {
            Console.WriteLine(e);
        }
    }

    public Meal Create(string title, int cost) {
        var sqlRequest = "INSERT INTO Meals (Title, Cost) VALUES (@title, @cost) RETURNING *";
        List <NpgsqlParameter> parameters = [new NpgsqlParameter("Title", title), new NpgsqlParameter("Cost", cost)];
        Meal? meal = null;
        _context.ExecuteWork(sqlRequest, parameters, reader => {
            if (reader.Read()) {
                var Id = reader.GetInt32(reader.GetOrdinal("Id"));
                var Title = reader.GetString(reader.GetOrdinal("Title"));
                var Cost = reader.GetInt32(reader.GetOrdinal("Cost"));
                meal = new Meal(Id, Title, Cost);
            }
        });
        return meal;
    }

    public void Delete() {
        try {
            using (var conn = _context.GetConnection()) {
                conn.Open();
                string sqlCreate = "DROP TABLE IF EXISTS Meals";
                using (var cmd = new NpgsqlCommand(sqlCreate, conn)) {
                    cmd.ExecuteNonQuery();
                }
                Console.WriteLine($"Meals repository deleted");
            }
        }
        catch (Exception e) {
            Console.WriteLine(e);
        }
    }
}