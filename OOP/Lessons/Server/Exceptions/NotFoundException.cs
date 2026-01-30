using Objects.Exceptions;

namespace Server.Exceptions;

public class NotFoundException : BaseException {
    public NotFoundException(string message = "Not found exception") : base(404, message) {
        
    }
}