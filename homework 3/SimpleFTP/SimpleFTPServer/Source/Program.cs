using System;

namespace Source
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new SimpleFTPServer("192.168.0.102", int.Parse(args[0]));
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