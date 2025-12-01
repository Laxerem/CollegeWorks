namespace Lesson.Interfaces;

public interface IBaseJsonable {
    public string ToJson();
}

public interface IJsonable<T> : IBaseJsonable {
    public abstract static T FromJson(string jsonString);
}