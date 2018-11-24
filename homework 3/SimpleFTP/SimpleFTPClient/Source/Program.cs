using System;
using System.Threading.Tasks;

namespace Source
{
    class Program
    {
        static async Task Main(string[] args)
        {
            const string ip = "192.168.0.102";
            const int port = 2121;
            const string path = "/home/anticnvm";

            var client = new SimpleFTPClient();

            var response = await client.ListAsync(ip, port, path);
            foreach (var pair in response)
            {
                Console.WriteLine($"{pair.Item1} {pair.Item2}");
            }
        }
    }
}
