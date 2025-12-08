using System.Text;
using Lesson.Entities;
using Lesson.Interfaces;

namespace Lesson.Utils;

public class JsonHelper {

    public static string NormalizeJson(string jsonString) {
        var normalizedJson = jsonString.Trim();
        normalizedJson = normalizedJson.Replace("\n", "");
        return normalizedJson;
    }
    
    public static string SerializeJsonList(IEnumerable<IBaseJsonable> itemsList) {
        var sb = new StringBuilder();
        sb.Append("[");
        foreach (var item in itemsList) {
            sb.Append(item.ToJson());
            sb.Append(",");
        }
        
        sb.Length--;
        sb.Append("]");
        
        return sb.ToString();
    }

    public static List<T> DeserializeJsonList<T>(string jsonString) where T : IJsonable<T> {
        var formatedString = jsonString.Trim().Replace("\n", "")[1..^1];
        var list = new List<T>();

        for (int i = 0; i < formatedString.Length; i++) {
            if (formatedString[i] == '{') {
                var endIndex = IJsonable<T>.GetElementEndIndex(i, '{', '}', formatedString);
                if (endIndex.HasValue) {
                    int end = endIndex.Value + 1;
                    list.Add(T.FromJson(formatedString[i..end]));
                    i = end;
                }
            }
        }
        
        return list;
    }
}