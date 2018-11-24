using System;
using System.Threading.Tasks;

namespace Source
{
    class Program
    {
        /// <summary>
        /// Make request to server depending on cli arguments
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        static async Task Main(string[] args)
        {
            const string ip = "192.168.0.102";
            const int port = 2120;
            const string path = "/home/anticnvm";
            const string pathToFile = "/home/anticnvm/Documents/3hgCRSwc4fU.png";

            var client = new SimpleFTPClient();

            switch (args[0])
            {
                case "--list":
                    {
                        var response = await client.ListAsync(ip, port, path);
                        foreach (var pair in response)
                        {
                            Console.WriteLine($"{pair.Item1} {pair.Item2}");
                        }
                        break;
                    }
                case "--get":
                    {
                        await client.GetFileAsync(ip, port, pathToFile, @"/home/anticnvm/2.png");
                        Console.WriteLine("LUL");
                        break;
                    }

                default:
                    break;
            }
        }
    }
}
