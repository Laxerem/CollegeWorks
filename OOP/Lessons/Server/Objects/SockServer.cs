using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Objects;

public class SockServer {
    private readonly TcpListener _server;
    
    public SockServer(int port) {
        _server = new TcpListener(IPAddress.Any, port);
    }

    public async Task StartAsync() {
        _server.Start();
        while (true) {
            var client = await WaitForConnectionAsync();
            try {
                NetworkStream stream = client.GetStream();
                using var streamReader = new StreamReader(stream, Encoding.UTF8);
                using var streamWriter = new StreamWriter(stream, Encoding.UTF8);
                streamWriter.AutoFlush = true;

                await HandleMessagesAsync(streamReader, streamWriter);
            }
            catch (Exception ex) {
                return;
            }
            finally {
                client.Close();
            }
        }
        _server.Stop();
    }

    private async Task<TcpClient> WaitForConnectionAsync() {
        Console.WriteLine("Waiting for a connection...");
        var client =  await _server.AcceptTcpClientAsync();
        Console.WriteLine("Connected");
        return client;
    }

    private async Task HandleMessagesAsync(StreamReader streamReader, StreamWriter streamWriter) {
        while (true) {
            var message = await streamReader.ReadLineAsync();
            Console.WriteLine("MESSAGE: " + message);
            
            switch (message) {
                case "exit":
                    return;
                case "stop":
                    throw new Exception();
            }
            
            await streamWriter.WriteLineAsync(message);
        }
    }

    public void Stop() {
        Console.WriteLine("Shutting down...");
        _server.Stop();
    }
}