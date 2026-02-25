namespace Objects.Exceptions;

public class BaseException : Exception {
    public readonly int Code;
    
    public BaseException(int code, string message) : base(message) {
        Code = code;
    }
}