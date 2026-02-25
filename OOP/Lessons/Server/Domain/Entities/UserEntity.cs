namespace Server.Database.Entities;

public class UserEntity {
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime Birthday { get; set; }

    public UserEntity(int id, string name, DateTime birthday) {
        Id = id;
        Name = name;
        Birthday = birthday;
    }

    public override string ToString() {
        return $"{Name} ({Id}) - > {Birthday}";
    }
}