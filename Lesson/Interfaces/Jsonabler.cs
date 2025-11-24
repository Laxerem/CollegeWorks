namespace Lesson.Interfaces;

public interface IJsonable {
    public string ToJson();
    public static IJsonable FromJson(string jsonString) {
        return null;
    }
}