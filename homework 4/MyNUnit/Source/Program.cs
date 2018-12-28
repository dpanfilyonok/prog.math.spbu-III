using System;
using System.IO;

namespace Source
{  
    /// <summary>
    /// Программа, выполняющая тестирование указанной директории и печатающая результат в консоль
    /// </summary>
    public class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("At least 1 argument should be passed");
                return;
            }

            var path = args[0];
            var testLauncher = new TestLauncher(path);
            try
            {
                testLauncher.LaunchTesting();
            }
            catch (DirectoryNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }

            testLauncher.PrintResults();
        }
    }
}
