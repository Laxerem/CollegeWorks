using Npgsql;

namespace Server.Database.Common;

public interface IRepository {
    public void Initialize();
    public void Delete();
}