namespace Server.Utils;

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

    public static string ReadFile(string path) {
        StreamReader sr = new StreamReader(path);
        try {
            return sr.ReadToEnd();
        }
        catch (Exception e) {
            Console.WriteLine(e.Message);
            throw new Exception();
        }
        finally {
            sr.Close();
        }
    }
}