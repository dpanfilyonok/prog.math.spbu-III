using System;

namespace ServerSource
{
    class Program
    {
        /// <summary>
        /// Running server on port
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            const string host = "localhost";
            const int port = 2121;

            var server = new SimpleFTPServer(host, port);
            try
            {
                server.RunAsync();
            }
            catch (Exception)
            {
                Console.WriteLine("O_O");
            }

            Console.ReadKey();
        }
    }
}