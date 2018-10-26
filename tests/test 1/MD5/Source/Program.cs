using System;
using System.IO;
using System.Security.Cryptography;

namespace Source
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Введите путь до директории аргументом");
            }

            var pathToDir = args[0];

            string dirHash;
            using (MD5 md5Hash = MD5.Create())
            {
                try
                {
                    dirHash = Md5HashSingleThread.GetMd5FromDir(pathToDir, md5Hash);
                    Console.WriteLine(dirHash);
                }
                catch (DirectoryNotFoundException e)
                {
                    Console.WriteLine($"Нет такой директории {e.Message}");
                }
            }
        }
    }
}

