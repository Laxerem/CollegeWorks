using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Server.Controllers;
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
        while (true) {
            if (_running) {
                var client = await WaitForConnectionAsync();
                try {
                    await _dispatcher.StartDispatchAsync(client);
                }
                catch (Exception ex) {
                    Stop();
                    return;
                }   
            }
            else {
                return;   
            }
        }
    }

    private async Task<TcpClient> WaitForConnectionAsync() {
        Console.WriteLine("Waiting for a connection...");
        var dispatcher =  await _server.AcceptTcpClientAsync();
        Console.WriteLine("Connected");
        return dispatcher;
    }

    // private async Task HandleMessagesAsync(StreamReader streamReader, StreamWriter streamWriter) {
    //     while (true) {
    //         var message = await streamReader.ReadLineAsync();
    //         Console.WriteLine("MESSAGE: " + message);
    //         
    //         switch (message) {
    //             case "exit":
    //                 return;
    //             case "stop":
    //                 throw new Exception();
    //         }
    //         
    //         await streamWriter.WriteLineAsync(message);
    //     }
    // }

    public void Stop() {
        Console.WriteLine("Shutting down...");
        _server.Stop();
        _running = false;
    }
}