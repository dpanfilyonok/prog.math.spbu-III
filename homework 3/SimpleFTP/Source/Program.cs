namespace Source
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;

    class Program
    {
        public static void Main(string[] args)
        {
            var thread = new Thread(() =>
            {
                var server = new SimpleFTPServer();
                server.Start();
            })
            { IsBackground = false };

            thread.Start();

            using (var client = new TcpClient("127.0.0.1", 2121))
            {
                Console.WriteLine($"Sending to port 2121 ...");
                var stream = client.GetStream();
                var writer = new StreamWriter(stream);
                writer.Write("Hello, world!");
                writer.Flush();
            }

            using (var client = new TcpClient("127.0.0.1", 2121))
            {
                Console.WriteLine($"Sending to port 2121 ...");
                var stream = client.GetStream();
                var writer = new StreamWriter(stream);
                writer.Write("123");
                writer.Flush();
            }
        }

        // private static async Task Main(string[] args)
        // {
        //     var httpClient = new HttpClient();
        //     var response = await httpClient.GetAsync("http://hwproj.me/");
        //     if (response.IsSuccessStatusCode)
        //     {
        //         var content = response.Content.;
        //         var a = response.RequestMessage;
        //         Console.WriteLine(a);
        //     }
        // }
    }
}

