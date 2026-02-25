using Npgsql;
using NpgsqlTypes;
using Server.Database.Common;
using Server.Database.Entities;

namespace Server.Database.Repositories;

public class UserRepository : IRepository {
    private readonly DbContext _context;
    
    public UserRepository(DbContext context) {
        _context = context;
    }

    public void Initialize() {
        try {
            using (var conn = _context.GetConnection()) {
                conn.Open();
                string sqlCreate = "CREATE TABLE IF NOT EXISTS Users (Id SERIAL PRIMARY KEY, Name TEXT NOT NULL, Birthday TIMESTAMP)";
                using (var cmd = new NpgsqlCommand(sqlCreate, conn)) {
                    cmd.ExecuteNonQuery();
                }
                Console.WriteLine("UserRepository initialized");
            }
        }
        catch (Exception e) {
            Console.WriteLine(e);
        }
    }

    public void Delete() {
        _context.DeleteTable("Users");
    }

    public UserEntity Create(string name, DateTime birthday) {
        string sqlInsert = "INSERT INTO Users (Name, Birthday) VALUES (@name, @birthday) RETURNING *";
        List<NpgsqlParameter> parameters = [new NpgsqlParameter("name", name), new NpgsqlParameter("birthday", NpgsqlDbType.TimestampTz) {Value = birthday}];
        UserEntity? userEntity = null;
        _context.ExecuteWork(sqlInsert, parameters, reader => {
            if (reader.Read()) {
                var Id = reader.GetInt32(reader.GetOrdinal("Id"));
                var Name = reader.GetString(reader.GetOrdinal("Name"));
                var Birthday = reader.GetDateTime(reader.GetOrdinal("Birthday"));
            
                userEntity = new UserEntity(Id, Name, Birthday);   
            }
        });
        return userEntity;
    }

    public List<UserEntity> GetAll() {
        List<UserEntity> result = new List<UserEntity>();
        _context.ExecuteWork("SELECT * FROM Users", null, reader => {
            while (reader.Read()) {
                var user = new UserEntity(
                    id: reader.GetInt32(reader.GetOrdinal("Id")),
                    name: reader.GetString(reader.GetOrdinal("Name")),
                    birthday: reader.GetDateTime(reader.GetOrdinal("Birthday"))
                );
                result.Add(user);
            }
        });
        return result;
    }
}