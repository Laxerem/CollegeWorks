using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;
using Objects;

public class Program {
    public async static Task Main(string[] args) {
        var sockServer = new SockServer(4004);
        await sockServer.StartAsync();
        sockServer.Stop();
    }
}