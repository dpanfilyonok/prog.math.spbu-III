using System;
using System.IO;
using System.Security.Cryptography;
using System.Diagnostics;
using System.Text;

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
            if (!Directory.Exists(pathToDir))
            {
                Console.WriteLine("нет такой директории");
            }

            string dirHash;
            using (MD5 md5Hash = MD5.Create())
            {
                dirHash = GetMd5FromDir(pathToDir, md5Hash);
            }
            
            Console.WriteLine(dirHash);
        }

        public static string GetMd5FromDir(string pathToDir, MD5 md5Hash)
        {
            var strBuilder = new StringBuilder();
            strBuilder.Append(new DirectoryInfo(pathToDir).Name);
            foreach (var file in Directory.GetFiles(pathToDir))
            {
                strBuilder.Append(GetMd5HashFromString(file, md5Hash));
            }
            
            foreach (var dir in Directory.GetDirectories(pathToDir))
            {
                strBuilder.Append(GetMd5FromDir(dir, md5Hash));
            }
            
            return GetMd5HashFromString(strBuilder.ToString(), md5Hash);
        }

        private static string GetMd5HashFromString(string source, MD5 md5Hash)
        {
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(source));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }
    }
}

