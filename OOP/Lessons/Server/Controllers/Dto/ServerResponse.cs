using Server.Interfaces;

namespace Objects.Dto;

public record ServerResponse(int code, string message) : IBaseJsonable {
    public string ToJson() => $"{{\"code\":{code},\"message\":\"{message}\"}}";
}
