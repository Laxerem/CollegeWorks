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

    public static string? ReadFirstStringValue(int pos, string jsonString) {
        for (int i = pos; i < jsonString.Length; i++) {
            if (jsonString[i] == '"') {
                var endIndex = IBaseJsonable.GetStringEndIndex(i, jsonString);
                if (!endIndex.HasValue) {
                    throw new Exception();
                }

                var startIndexValue = i + 1;
                var endIndexValue = endIndex.Value;

                return jsonString[startIndexValue..endIndexValue];
            }
        }
        return null;
    }
}