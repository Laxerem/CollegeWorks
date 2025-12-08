using System.Text;
using Lesson.Entities;
using Lesson.Interfaces;

namespace Lesson.Utils;

public class JsonFileHelper : BaseFileHelper {
    public static int WriteJson(string path, IBaseJsonable obj) {
        return WriteToFile(path, obj.ToJson());
    }

    public T ReadJson<T>(string path) where T : IJsonable<T> {
        var jsonString = ReadFile(path);
        return T.FromJson(jsonString);
    }

    public static int WriteJsonItemsList(string path, IEnumerable<IBaseJsonable> jsonList) {
        var objStringList = JsonHelper.SerializeJsonList(jsonList);
        return WriteToFile(path, objStringList);
    }

    public static List<T> ReadJsonItemsList<T>(string path) where T : IJsonable<T> {
        var jsonString = ReadFile(path);
        var objList = JsonHelper.DeserializeJsonList<T>(jsonString);
        return objList;
    }
}