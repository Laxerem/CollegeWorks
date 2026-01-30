using System.Text;
using Server.Interfaces;
using Server.Utils;
using Server.Validators;

namespace Lesson.Utils;

public class JsonFileHelper : BaseFileHelper {
    private static readonly JsonValidator _validator = new();

    private static void ValidateJson(string jsonString) {
        var result = _validator.Validate(jsonString);
        if (!result.IsValid) {
            var errors = string.Join("; ", result.Errors.Select(e => e.ErrorMessage));
            throw new ArgumentException($"Invalid JSON: {errors}");
        }
    }
    public static int WriteJson(string path, IBaseJsonable obj) {
        return WriteToFile(path, obj.ToJson());
    }

    public T ReadJson<T>(string path) where T : IJsonable<T> {
        var jsonString = ReadFile(path);
        ValidateJson(jsonString);
        return T.FromJson(jsonString);
    }

    public static int WriteJsonItemsList(string path, IEnumerable<IBaseJsonable> jsonList) {
        var objStringList = JsonHelper.SerializeJsonList(jsonList);
        return WriteToFile(path, objStringList);
    }

    public static List<T> ReadJsonItemsList<T>(string path) where T : IJsonable<T> {
        var jsonString = ReadFile(path);
        ValidateJson(jsonString);
        var objList = JsonHelper.DeserializeJsonList<T>(jsonString);
        return objList;
    }
}