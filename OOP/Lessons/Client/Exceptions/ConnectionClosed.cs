namespace Client.Exceptions;

public class ConnectionClosed : Exception {
    public ConnectionClosed(string message) : base(message)  {
        
    }
}