using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Source
{
    internal static class SimpleFTPServerUtils
    {
        internal static (Methods, string) ParseRequest(string request)
        {
            if (request == null)
            {
                throw new ArgumentNullException();
            }

            var splited = request.Split();
            return ((Methods)Convert.ToInt32(splited[0]), splited[1]);
        }

        internal static List<(string, bool)> GetListOfElementsInDir(string pathToDir)
        {
            if (!Directory.Exists(pathToDir))
            {
                throw new DirectoryNotFoundException(pathToDir);
            }

            var root = new DirectoryInfo(pathToDir);
            var files = root.EnumerateFiles();
            var dirs = root.EnumerateDirectories();
            var listOfContent = new List<(string, bool)>();

            foreach (var file in files)
            {
                listOfContent.Add((file.Name, false));
            }

            foreach (var dir in dirs)
            {
                listOfContent.Add((dir.Name, true));
            }

            return listOfContent;
        }

        internal static string CreateResponseOfListMethod(List<(string, bool)> content)
        {
            var response = new StringBuilder();
            response.Append(content.Count);
            response.Append('&');
            foreach (var pair in content)
            {
                response.Append($"{pair.Item1}&{pair.Item2}");
                response.Append('&');
            }

            response.AppendLine();
            return response.ToString();
        }

        internal static FileStream GetReadableFileStream(string pathToFile)
        {
            if (!File.Exists(pathToFile))
            {
                throw new FileNotFoundException(pathToFile);
            }

            return File.OpenRead(pathToFile);
        }
    }
}