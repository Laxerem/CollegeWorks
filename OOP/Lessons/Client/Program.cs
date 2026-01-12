using System;
using System.Net.Sockets;
using System.IO;
using System.Text;

public class Client {
    private static TcpClient clientSocket; // сокет для общения
    private static StreamReader streamReader; // поток чтения из сокета
    private static StreamWriter streamWriter; // поток записи в сокет

    public static void Main(string[] args) {
        try {
            try {
                // подключаемся к серверу на localhost:4004
                clientSocket = new TcpClient("localhost", 4004);
                
                // создаём потоки для работы с сокетом
                NetworkStream stream = clientSocket.GetStream();
                streamReader = new StreamReader(stream, Encoding.UTF8);
                streamWriter = new StreamWriter(stream, Encoding.UTF8);

                Console.WriteLine("Вы что-то хотели сказать? Введите это здесь:");
                
                // читаем ввод пользователя с консоли
                string word = Console.ReadLine();
                
                // отправляем сообщение на сервер
                streamWriter.WriteLine(word);
                streamWriter.Flush();
                
                // ждём ответ от сервера
                string serverWord = streamReader.ReadLine();
                Console.WriteLine(serverWord); // выводим ответ на экран
            }
            finally {
                // закрываем потоки и сокет
                Console.WriteLine("Клиент был закрыт...");
                streamReader?.Close();
                streamWriter?.Close();
                clientSocket?.Close();
            }
        }
        catch (Exception e) {
            Console.Error.WriteLine(e);
        }
    }
}