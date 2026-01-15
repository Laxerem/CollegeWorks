using System.Net.Sockets;

namespace Client.Objects;

public class SockClient {
    private int _port;

    public SockClient(int port) {
        _port = port;
    }

    public async Task StartAsync() {
        while (true) {
            try {
                await ExecuteProgramAsync();
            }
            catch (Exception e) {
                Console.WriteLine("Shutting down...");
                return;
            }
        }
    }

    private async Task ExecuteProgramAsync() {
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
                if (requestMessage == "stop") {
                    throw new Exception();
                }
                return;
            }
            
            Console.WriteLine("MESSAGE: " + responseMessage);   
        }
    }
}