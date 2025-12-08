namespace Lesson.Interfaces;

public interface IBaseJsonable {
    public string ToJson();
    
    public static int? GetElementEndIndex(int pos, char startChar, char endChar, string str) {
        if (str[pos] != startChar)
            return null;

        int depth = 0;
        bool inString = false;

        for (int i = pos; i < str.Length; i++) {
            char c = str[i];

            // Обработка строк — чтобы не реагировать на {, }, [, ] внутри строк
            if (c == '"' && (i == 0 || str[i - 1] != '\\'))
                inString = !inString;

            if (inString)
                continue;

            if (c == startChar)
                depth++;

            else if (c == endChar) {
                depth--;
                if (depth == 0)
                    return i;
            }
        }
        return null;
    }
}

public interface IJsonable<T> : IBaseJsonable {
    public abstract static T FromJson(string jsonString);
}