using System.Net;
using System.Net.Sockets;
using Server.Dispatcher;

namespace Objects;

public class SockServer {
    private readonly TcpListener _server;
    private readonly ClientDispatcher _dispatcher;
    private bool _running;

    public SockServer(int port, ClientDispatcher dispatcher) {
        _server = new TcpListener(IPAddress.Any, port);
        _dispatcher = dispatcher;
        _dispatcher.StopCommand += Stop;
    }

    public async Task StartAsync() {
        _server.Start();
        _running = true;
        Console.WriteLine($"Server started. Listening on port {_server.LocalEndpoint}");

        while (_running) {
            var client = await _server.AcceptTcpClientAsync();
            Console.WriteLine("Client connected");
            _ = _dispatcher.StartDispatchAsync(client);
        }
    }

    public void Stop() {
        Console.WriteLine("Shutting down...");
        _running = false;
        _server.Stop();
    }
}
