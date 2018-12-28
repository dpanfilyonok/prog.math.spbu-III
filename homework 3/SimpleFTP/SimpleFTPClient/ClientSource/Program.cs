using System;
using System.Threading.Tasks;

namespace ClientSource
{
    class Program
    {
        private const string _ip = "127.0.0.1";
        private const int _port = 2121;

        static async Task Main(string[] args)
        {
            var client = new SimpleFTPClient();
            var response = await client.ListAsync(_ip, 2222, @"../ServerTests/TestFolderNonexistent");
            Console.WriteLine(response);
        }
    }
}
