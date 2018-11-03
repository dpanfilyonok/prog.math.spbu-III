namespace Source
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading.Tasks;
    using System.Collections.Generic;

    public class SimpleFTPServer
    {
        private TcpListener _tcpServer;
        private const int _port = 2121;
        enum Methods
        {
            List = 1,
            Get = 2
        }

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
                var request = await reader.ReadToEndAsync();
                var (method, path) = ParseRequest(request);
                switch (method)
                {
                    case (int)Methods.List:
                    // формируем ответ из контента:
                        var content = GetListOfContentsInDir(path);
                        break;
                    case (int)Methods.Get:
                        break;
                    default:
                        throw new WebException("Invalid method");
                }
                
                // тут нужно просто асинхроно дать ответ
            });
        }

        private List<Tuple<string, bool>> GetListOfContentsInDir(string pathToDir)
        {
            if (!Directory.Exists(pathToDir))
            {
                throw new DirectoryNotFoundException(pathToDir);
            }

            var root = new DirectoryInfo(pathToDir);
            var files = root.EnumerateFiles();
            var dirs = root.EnumerateDirectories();
            var listOfContent = new List<Tuple<string, bool>>();
            
            foreach (var file in files)
            {
                listOfContent.Add(new Tuple<string, bool>(file.Name, false));
            }

            foreach (var dir in dirs)
            {
                listOfContent.Add(new Tuple<string, bool>(dir.Name, true));
            }

            return listOfContent;
        }

        private (int method, string path) ParseRequest(string request)
        {
            var splited = request.Split();
            return (Convert.ToInt32(splited[0]), splited[1]);
        }
    }
}