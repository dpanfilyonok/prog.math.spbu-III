namespace Source
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Diagnostics;
    using System.Text;

    public static class Md5HashSingleThread
    {
        public static string GetMd5FromDir(string pathToDir, MD5 md5Hash)
        {
            if (!Directory.Exists(pathToDir))
            {
                throw new DirectoryNotFoundException(pathToDir);
            }

            var strBuilder = new StringBuilder();
            strBuilder.Append(new DirectoryInfo(pathToDir).Name);

            var files = Directory.GetFiles(pathToDir);
            Array.Sort(files);
            foreach (var file in files)
            {
                strBuilder.Append(GetMd5HashFromString(File.ReadAllText(file) ,md5Hash));
            }

            var dirs = Directory.GetDirectories(pathToDir);
            Array.Sort(dirs);
            foreach (var dir in dirs)
            {
                strBuilder.Append(GetMd5FromDir(dir, md5Hash));
            }

            return GetMd5HashFromString(strBuilder.ToString(), md5Hash);
        }

        public static string GetMd5HashFromString(string source, MD5 md5Hash)
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