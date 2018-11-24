namespace Source
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;

    public class SimpleFTPServer
    {
        private TcpListener _tcpServer;
        private CancellationTokenSource _cts;
        public string Address { get; }
        public int Port { get; }

        public SimpleFTPServer(string address, int port = 2121)
        {
            Port = port;
            Address = address;
            _tcpServer = new TcpListener(IPAddress.Parse(address), port);
            _cts = new CancellationTokenSource();

            _tcpServer.Start();
        }

        public void Start()
        {
            Console.WriteLine($"Server started, listening port {Port} ...");
            while (!_cts.IsCancellationRequested)
            {
                Console.WriteLine("Waiting for a connections...");
                var client = _tcpServer.AcceptTcpClient();

                Console.WriteLine($"Client on {client.Client.AddressFamily} connected. Executing request async...");
                try
                {
                    ServeRequestAsync(client);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                finally
                {
                    client.Close();
                    _tcpServer.Stop();
                }
            }
        }

        private async void ServeRequestAsync(TcpClient user)
        {
            await Task.Run(async () =>
            {
                using (var networkStream = user.GetStream())
                {
                    var request = await ProcessRequestAsync(networkStream);
                    ProcessResponseAsync(networkStream, request);
                }
            });
        }

        private async Task<(Methods, string)> ProcessRequestAsync(NetworkStream stream)
        {
            var reader = new StreamReader(stream);
            var request = await reader.ReadLineAsync();

            return SimpleFTPServerUtils.ParseRequest(request);
        }

        private async void ProcessResponseAsync(NetworkStream stream, (Methods method, string path) request)
        {
            var writer = new StreamWriter(stream) { AutoFlush = true };
            switch (request.method)
            {
                case Methods.List:
                    {
                        string response;
                        try
                        {
                            var content = SimpleFTPServerUtils.GetListOfElementsInDir(request.path);
                            response = SimpleFTPServerUtils.CreateResponseOfListMethod(content);
                        }
                        catch (DirectoryNotFoundException)
                        {
                            response = "-1";
                        }

                        await writer.WriteLineAsync(response);
                        break;
                    }
                case Methods.Get:
                    {
                        try
                        {
                            using (var fstream = SimpleFTPServerUtils.GetReadableFileStream(request.path))
                            {
                                await writer.WriteAsync(fstream.Length.ToString() + ' ');
                                await fstream.CopyToAsync(writer.BaseStream);
                            }

                        }
                        catch (FileNotFoundException)
                        {
                            await writer.WriteLineAsync("-1");
                        }

                        break;
                    }
                default:
                    throw new WebException("Invalid method");

            }
        }

        public void Stop()
        {
            _cts.Cancel();
        }
    }
}