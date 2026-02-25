using System.Text;
using Server.Interfaces;
using Server.Validators;

namespace Server.Utils;

public class JsonHelper {
    

    public static string NormalizeJson(string jsonString) {
        var jsonValidator = new JsonValidator();
        var result = jsonValidator.Validate(jsonString);
        if (!result.IsValid) {
            throw new Exception(result.Errors[0].ErrorMessage);
        }

        var normalizedJson = jsonString.Trim();
        normalizedJson = normalizedJson.Replace("\n", "");

        bool isString = false;
        for (int i = 0; i < normalizedJson.Length; i++) {
            if (normalizedJson[i] == '"') {
                isString = !isString;
            }
            if (normalizedJson[i] == ' ' && !isString) {
                normalizedJson = normalizedJson.Remove(i, 1);
                i--;
            }
        }

        return normalizedJson;
    }


    public static string Serialize(IBaseJsonable entity) => entity.ToJson();

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
    
    public static T? Deserialize<T>(string jsonString) where T : IJsonable<T> =>
        T.FromJson(jsonString);
    
    public static List<T> DeserializeJsonList<T>(string jsonString) where T : IJsonable<T> {
        var formatedString = NormalizeJson(jsonString)[1..^1];
        var list = new List<T>();

        for (int i = 0; i < formatedString.Length; i++) {
            if (formatedString[i] == '{') {
                var endIndex = IBaseJsonable.GetElementEndIndex(i, '{', '}', formatedString);
                if (endIndex.HasValue) {
                    int end = endIndex.Value + 1;
                    var element = T.FromJson(formatedString[i..end]);
                    if (element != null) {
                        list.Add(element);
                    }
                    i = end;
                }
            }
        }
        return list;
    }
    
    public static string? ReadStringByKey(string key, string jsonString) {
        var normalized = NormalizeJson(jsonString);
        var keyPattern = $"\"{key}\":";
        var keyIndex = normalized.IndexOf(keyPattern, StringComparison.Ordinal);
        if (keyIndex == -1) return null;
        return ReadFirstStringValue(keyIndex + keyPattern.Length, normalized);
    }
    public static int? ReadIntByKey(string key, string jsonString) {
        var normalized = NormalizeJson(jsonString);
        var keyPattern = $"\"{key}\":";
        var keyIndex = normalized.IndexOf(keyPattern, StringComparison.Ordinal);
        if (keyIndex == -1) return null;

        int start = keyIndex + keyPattern.Length;
        int end = start;
        while (end < normalized.Length && (char.IsDigit(normalized[end]) || normalized[end] == '-')) {
            end++;
        }
        if (end == start) return null;
        return int.Parse(normalized[start..end]);
    }
    
    public static List<int> ReadIntArrayByKey(string key, string jsonString) {
        var normalized = NormalizeJson(jsonString);
        var keyPattern = $"\"{key}\":";
        var keyIndex = normalized.IndexOf(keyPattern, StringComparison.Ordinal);
        if (keyIndex == -1) return [];

        int valueStart = keyIndex + keyPattern.Length;
        for (int i = valueStart; i < normalized.Length; i++) {
            if (normalized[i] == '[') {
                var arrayEnd = IBaseJsonable.GetElementEndIndex(i, '[', ']', normalized);
                if (!arrayEnd.HasValue) return [];

                var inner = normalized[(i + 1)..arrayEnd.Value];
                return inner.Split(',')
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Select(s => int.Parse(s.Trim()))
                    .ToList();
            }
        }
        return [];
    }

    public static string? ReadFirstStringValue(int pos, string jsonString) {
        for (int i = pos; i < jsonString.Length; i++) {
            if (jsonString[i] == '"') {
                var endIndex = IBaseJsonable.GetStringEndIndex(i, jsonString);
                if (!endIndex.HasValue) {
                    throw new Exception();
                }
                return jsonString[(i + 1)..endIndex.Value];
            }
        }
        return null;
    }
}
