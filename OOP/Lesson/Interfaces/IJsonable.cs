namespace Lesson.Interfaces;

public interface IBaseJsonable {
    public string ToJson();
    
    public static int? GetElementEndIndex(int pos, char startChar, char endChar, string str) {
        if (pos >= str.Length || str[pos] != startChar)
            return null;

        int depth = 0;
        bool inString = false;
        for (int i = pos; i < str.Length; i++) {
            char c = str[i];
            
            if (c == '"') {
                inString = !inString;
                continue;
            }
            
            if (inString)
                continue;
            
            if (c == startChar) {
                depth++;
            }
            
            else if (c == endChar) {
                depth--;
                if (depth == 0)
                    return i;
            }
        }
        
        return null;
    }

    public static int? GetStringEndIndex(int pos, string str) {
        bool lastCharIsSlash = false;
        for (int i = pos + 1; i < str.Length; i++) {
            if (str[i] == '\\') {
                lastCharIsSlash = true;
                continue;
            }
            if (str[i] == '"' && !lastCharIsSlash) {
                return i;
            }
            lastCharIsSlash = false;
        }
        return null;
    }
}

public interface IJsonable<T> : IBaseJsonable {
    public abstract static T? FromJson(string jsonString);
}