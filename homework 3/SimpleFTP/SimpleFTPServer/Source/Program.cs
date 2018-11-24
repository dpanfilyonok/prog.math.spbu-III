using System;

namespace Source
{
    class Program
    {
        /// <summary>
        /// Running server on port
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            const string host = "192.168.0.102";
            const int port = 2121;

            var server = new SimpleFTPServer(host, port);
            try
            {
                server.Start();
            }
            catch (Exception)
            {
                Console.WriteLine("O_O");
            }

            Console.ReadKey();
        }
    }
}