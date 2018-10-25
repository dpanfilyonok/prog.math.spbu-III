namespace Source
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading.Tasks;

    public class SimpleFTPServer
    {
        private TcpListener _tcpServer;
        private const int _port = 2121;

        public SimpleFTPServer()
        {
            IPAddress localAddress = IPAddress.Parse("127.0.0.1");
            _tcpServer = new TcpListener(localAddress, _port);
            _tcpServer.Start();
        }

        public void Start()
        {
            while (true)
            {
                Console.WriteLine("Ожидание подключений... ");
                var client = _tcpServer.AcceptTcpClient();
                ServeUserAsync(client);
            }
        }

        private async void ServeUserAsync(TcpClient user)
        {
            await Task.Run(async () =>
            {
                Console.WriteLine("Подключен клиент. Выполнение запроса...");
                var networkStream = user.GetStream();
                var reader = new StreamReader(networkStream);
                var data = await reader.ReadToEndAsync();
                // тут нужно просто асинхроно дать ответ
            });
        }
    }
}