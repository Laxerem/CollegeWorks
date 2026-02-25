using System.Data.Common;
using Npgsql;
using Server.Database.Repositories;

namespace Server.Database;

public class DbContext {
    private readonly string _connectionString;
    public DbContext(string connectionString) {
        _connectionString = connectionString;
    }

    public NpgsqlConnection GetConnection() {
      var conn = new NpgsqlConnection(_connectionString);
      return conn;
    }

    public bool DeleteTable(string tableName) {
        try {
            using (var conn = GetConnection()) {
                conn.Open();
                string sqlCreate = "DROP TABLE IF EXISTS @name";
                using (var cmd = new NpgsqlCommand(sqlCreate, conn)) {
                    cmd.Parameters.Add(new NpgsqlParameter("name", tableName));
                    cmd.ExecuteNonQuery();
                }
                Console.WriteLine($"{tableName} was deleted");
                return true;
            }
        }
        catch (Exception e) {
            Console.WriteLine(e);
            return false;
        }
    }

    public void ExecuteWork(string sql, List<NpgsqlParameter>? parameters = null, Action<NpgsqlDataReader>? work = null) {
        using (var conn = GetConnection()) {
            conn.Open();
            using var transcation = conn.BeginTransaction();
            try {
                using (var cmd = new NpgsqlCommand(sql, conn)) {
                    if (parameters != null) {
                        parameters.ForEach(p => cmd.Parameters.Add(p));
                    }
                    if (work != null) {
                        using (var reader = cmd.ExecuteReader()) {
                            work(reader);
                        }
                    }
                    else {
                        cmd.ExecuteNonQuery();
                    }
                    transcation.Commit();
                }
            }
            catch {
                transcation.Rollback();
                throw;
            }
        }
    }
}