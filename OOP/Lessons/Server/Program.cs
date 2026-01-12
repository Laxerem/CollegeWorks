using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;

public class Server {
    private static TcpClient clientSocket; // сокет для общения
    private static TcpListener server; // серверный сокет
    private static StreamReader streamReader; // поток чтения из сокета
    private static StreamWriter streamWriter; // поток записи в сокет

    public static void Main(string[] args) {
        try {
            try {
                server = new TcpListener(IPAddress.Any, 4004); // прослушиваем порт 4004
                server.Start();
                Console.WriteLine("Сервер запущен!"); // объявляем о запуске
                
                clientSocket = server.AcceptTcpClient(); // ждём подключения клиента
                
                try
                {
                    // создаём потоки ввода/вывода
                    NetworkStream stream = clientSocket.GetStream();
                    streamReader = new StreamReader(stream, Encoding.UTF8);
                    streamWriter = new StreamWriter(stream, Encoding.UTF8);

                    string word = streamReader.ReadLine(); // ждём сообщение от клиента
                    Console.WriteLine(word);
                    
                    // отвечаем клиенту
                    streamWriter.WriteLine("Привет, это Сервер! Подтверждаю, вы написали : " + word);
                    streamWriter.Flush(); // выталкиваем всё из буфера
                }
                finally
                {
                    // закрываем потоки и сокет
                    streamReader?.Close();
                    streamWriter?.Close();
                    clientSocket?.Close();
                }
            }
            finally
            {
                Console.WriteLine("Сервер закрыт!");
                server?.Stop();
            }
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e);
        }
    }
}