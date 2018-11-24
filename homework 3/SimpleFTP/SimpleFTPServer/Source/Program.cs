using System;

namespace Source
{
    class Program
    {
        static void Main(string[] args)
        {
            string host = "192.168.0.102";
            int port = int.Parse(args[0]);

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