using System.Net.Sockets;

namespace Client.Objects;

public class SockClient {
    private int _port;

    public SockClient(int port) {
        _port = port;
    }

    public async Task StartAsync() {
        Console.WriteLine($"Введите ip адрес сервера: ");
        var hostName = Console.ReadLine();
        var client = new TcpClient(hostName, _port);
        using var stream = client.GetStream();

        using var streamReader = new StreamReader(stream);
        using var streamWriter = new StreamWriter(stream);
        streamWriter.AutoFlush = true;
        
        while (true) {
            Console.WriteLine("Введите сообщение: ");
            var requestMessage = Console.ReadLine();
            await streamWriter.WriteLineAsync(requestMessage);
        
            var responseMessage = await streamReader.ReadLineAsync();
            if (responseMessage is null) {
                Console.WriteLine("Сервер закрыл соединение");
                return;
            }
            
            Console.WriteLine("MESSAGE: " + responseMessage);   
        }
    }
}