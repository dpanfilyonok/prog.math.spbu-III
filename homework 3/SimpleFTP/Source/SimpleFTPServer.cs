namespace Source
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using System.Text;

    public class SimpleFTPServer
    {
        private TcpListener _tcpServer;
        private const int _port = 2121;
        private enum Methods
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
                // поток, в которос происходит общение
                using (var networkStream = user.GetStream())
                {
                    // todo: обработать завершение сеанса
                    while (true)
                    {
                        var request = await ProcessRequestAsync(networkStream);
                        ProcessResponseAsync(networkStream, request);
                        // * пойдет принимать следующий запрос => может быть ситуация,
                        // * когда ц запрос выполниться раньше первого
                    }
                }
            });
        }

        private async Task<(Methods, string)> ProcessRequestAsync(NetworkStream stream)
        {
            string request;
            using (var reader = new StreamReader(stream))
            {
                request = await reader.ReadToEndAsync();
            }

            return ParseRequest(request);
        }

        private (Methods, string) ParseRequest(string request)
        {
            var splited = request.Split();
            return ((Methods)Convert.ToInt32(splited[0]), splited[1]);
        }

        private async void ProcessResponseAsync(NetworkStream stream, (Methods method, string path) request)
        {
            byte[] response;
            switch (request.method)
            {
                case Methods.List:
                    {
                        try
                        {
                            var content = GetListOfElementsInDir(request.path);
                            response = CreateResponseOfListMethod(content);
                        }
                        catch (DirectoryNotFoundException)
                        {
                            response = BitConverter.GetBytes(-1);
                        }

                        break;
                    }
                case Methods.Get:
                    {
                        try
                        {
                            var content = await BinaryContentOfFileAsync(request.path);
                            response = CreateResponseOfGetMethod(content);
                        }
                        catch (FileNotFoundException)
                        {
                            response = BitConverter.GetBytes(-1);
                        }

                        break;
                    }
                default:
                    throw new WebException("Invalid method");

            }

            using (var writer = new BinaryWriter(stream))
            {
                writer.Write(response);
            }
        }

        #region ListStuff
        private List<(string, bool)> GetListOfElementsInDir(string pathToDir)
        {
            if (!Directory.Exists(pathToDir))
            {
                throw new DirectoryNotFoundException(pathToDir);
            }

            var root = new DirectoryInfo(pathToDir);
            var files = root.EnumerateFiles();
            var dirs = root.EnumerateDirectories();
            var listOfContent = new List<(string, bool)>();

            foreach (var file in files)
            {
                listOfContent.Add((file.Name, false));
            }

            foreach (var dir in dirs)
            {
                listOfContent.Add((dir.Name, true));
            }

            return listOfContent;
        }

        private byte[] CreateResponseOfListMethod(List<(string, bool)> content)
        {
            var response = new StringBuilder();
            response.Append(content.Count);
            response.Append(' ');
            foreach (var pair in content)
            {
                response.AppendJoin(' ', pair);
                response.Append(' ');
            }

            response.AppendLine();
            return Encoding.Default.GetBytes(response.ToString());
        }

        #endregion

        #region GetStuff
        private async Task<byte[]> BinaryContentOfFileAsync(string pathToFile)
        {
            if (!File.Exists(pathToFile))
            {
                throw new FileNotFoundException(pathToFile);
            }

            return await File.ReadAllBytesAsync(pathToFile);
        }

        private byte[] CreateResponseOfGetMethod(byte[] content)
        {
            var contentSize = content.LongLength;
            var space = ' ';

            var sizeConverted = BitConverter.GetBytes(contentSize);
            var spaceConverted = BitConverter.GetBytes(space);

            var response = new byte[sizeConverted.Length + spaceConverted.Length + content.Length];

            sizeConverted.CopyTo(response, 0);
            spaceConverted.CopyTo(response, sizeConverted.Length);
            content.CopyTo(response, spaceConverted.Length);

            return response;
        }

        #endregion
    }
}