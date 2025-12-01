using System.Text;
using Lesson.Entities;
using Lesson.Interfaces;

namespace Lesson.Utils;

public class JsonFileHelper : BaseFileHelper {
    public static int WriteJson(string path, IBaseJsonable json) {
        return WriteToFile(path, json.ToJson());
    }

    public static int WriteJsonItemsList(string path, List<IBaseJsonable> jsonList) {
        var sb = new StringBuilder();
        sb.Append("[\n");
        
        for (int i = 0; i < jsonList.Count; i++) {
            sb.Append(jsonList[i].ToJson());
            if (i + 1 < jsonList.Count) {
                sb.Append(",\n");
            }
        }
        sb.Append("\n]");
        return WriteToFile(path, sb.ToString());
    }

    public static List<T> ReadJsonItemsList<T>(string path) where T : IJsonable<T> {
        var jsonString = BaseFileHelper.ReadFile(path);
        
        var formatedString = jsonString.Trim().Replace("\n", "")[1..^1];
        var list = new List<T>();

        foreach (var mealString in formatedString.Split("}")) {
            if (mealString == string.Empty) break;           
            
            string formattedMealString;
            if (mealString.StartsWith(',')) {
                formattedMealString = mealString[1..] + '}';   
            }
            else {
                formattedMealString = mealString + '}'; 
            }
            
            list.Add(T.FromJson(formattedMealString));   
        }
        
        return list;
    }
}