using System.Text;
using Lesson.Interfaces;

namespace Lesson.Utils;

public class BaseFileHelper {
    public static int WriteToFile(string path, string content) {
        StreamWriter sw = new StreamWriter(path);
        try {
            Console.WriteLine($"Writing to {path}");
            sw.Write(content);
            return 0;
        }
        catch (Exception e) {
            Console.WriteLine(e.Message);
            return 1;
        }
        finally {
            sw.Close();
        }
    }
    
    public static int UpdateFile(string path, string content) {
        try {
            File.AppendAllText(path, content);
            return 0;
        }
        catch (Exception e) {
            return 1;
        }
    }

    public static int ClearFile(string path) {
        StreamWriter sw = new StreamWriter(path);
        try {
            Console.WriteLine($"Clear file: {path}");
            sw.Write("");
            return 0;
        }
        catch (Exception e) {
            Console.WriteLine(e.Message);
            return 1;
        }
        finally {
            sw.Close();
        }
    }
}

public class JsonFileHelper : BaseFileHelper {
    public static int WriteJson(string path, IJsonable json) {
        return WriteToFile(path, json.ToJson());
    }

    public static int WriteJsonItemsList(string path, List<IJsonable> jsonList) {
        var sb = new StringBuilder();
        sb.Append("{\n");
        
        for (int i = 0; i < jsonList.Count; i++) {
            sb.Append($"\"{i}\":" + jsonList[i].ToJson());
            if (i + 1 < jsonList.Count) {
                sb.Append(",\n");
            }
        }
        sb.Append("\n}");
        return WriteToFile(path, sb.ToString());
    }
}