using System;
using System.Net.Sockets;
using System.IO;
using System.Text;
using Client.Objects;

public class Program {
    public async static Task Main(string[] args) {
        var client = new SockClient(4004);
        await client.StartAsync();
    }
}