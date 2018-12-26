using System;
using System.IO;

namespace Source
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("At least 1 argument should be passed");
                return;
            }

            var path = args[0];
            var testLauncher = new TestLauncher();
            try
            {
                testLauncher.LaunchTestingIn(path);
            }
            catch (DirectoryNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }

            testLauncher.PrintResults();
        }
    }
}
