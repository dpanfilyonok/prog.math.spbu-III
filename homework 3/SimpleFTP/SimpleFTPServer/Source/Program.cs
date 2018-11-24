using System;

namespace Source
{
    class Program
    {
        static void Main(string[] args)
        {
            const string host = "192.168.0.102";
            const int port = 2121;

            var server = new SimpleFTPServer(host, port);
            try
            {
                server.Start();
            }
            catch (ArgumentNullException)
            {
                Console.WriteLine("kek");
            }

            Console.ReadKey();
        }
    }
}