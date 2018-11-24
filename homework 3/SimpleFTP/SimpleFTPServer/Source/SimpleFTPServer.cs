namespace Source
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading.Tasks;
    using System.Threading;
    using Source.Exceptions;

    public class SimpleFTPServer
    {
        private TcpListener _tcpServer;
        private CancellationTokenSource _cts;
        private int _amountOfActualConnections;
        private ManualResetEvent _lackOfActualConnectionsEvent;
        private object _lockObject;

        public string Address { get; }
        public int Port { get; }

        public SimpleFTPServer(string address, int port = 2121)
        {
            Port = port;
            Address = address;
            _tcpServer = new TcpListener(IPAddress.Parse(address), port);
            _cts = new CancellationTokenSource();
            _amountOfActualConnections = 0;
            _lackOfActualConnectionsEvent = new ManualResetEvent(true);
            _lockObject = new object();

            _tcpServer.Start();
        }

        public void Start()
        {
            Console.WriteLine($"Server started, listening port {Port} ...");

            while (!_cts.IsCancellationRequested)
            {
                Console.WriteLine("Waiting for a connections...");
                var client = _tcpServer.AcceptTcpClient();

                Console.WriteLine($"Client on {client.Client.RemoteEndPoint} connected. Executing request async...");
                lock (_lockObject)
                {
                    _amountOfActualConnections++;
                }

                ServeRequestAsync(client);
            }

            _lackOfActualConnectionsEvent.WaitOne();
            _tcpServer.Stop();
        }

        private async void ServeRequestAsync(TcpClient client)
        {
            try
            {
                await Task.Run(async () =>
                {
                    var clientEP = client.Client.RemoteEndPoint.ToString();
                    using (var networkStream = client.GetStream())
                    {
                        var request = await ProcessRequestAsync(networkStream);
                        await ProcessResponseAsync(networkStream, request);
                    }

                    Console.WriteLine($"Client on {clientEP} served.");
                    DisconnectClient(client);
                });
            }
            catch (ConnectionRefusedException e)
            {
                Console.WriteLine(e.Message);
                DisconnectClient(client);
            }
        }

        /// <exception cref="ConnectionRefusedException"> => client close net stream</exception>
        private async Task<(Methods, string)> ProcessRequestAsync(NetworkStream stream)
        {
            var reader = new StreamReader(stream);
            var request = await reader.ReadLineAsync();

            if (request == null)
            {
                throw new ConnectionRefusedException(
                    "Connection were refused via client by closing network stream."
                );
            }

            return SimpleFTPServerUtils.ParseRequest(request);
        }

        /// <exception cref="WebException"> => invalid method in response</exception>
        private async Task ProcessResponseAsync(NetworkStream stream, (Methods method, string path) request)
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

        private void DisconnectClient(TcpClient client)
        {
            if (client.Connected)
            {
                Console.WriteLine("Closing connection...");
                client.Close();
            }
            else
            {
                Console.WriteLine("Connection were closed by client side.");
            }

            lock (_lockObject)
            {
                _amountOfActualConnections--;
            }

            if (_amountOfActualConnections == 0)
            {
                _lackOfActualConnectionsEvent.Set();
            }
        }

        public void Stop()
        {
            _cts.Cancel();
        }
    }
}