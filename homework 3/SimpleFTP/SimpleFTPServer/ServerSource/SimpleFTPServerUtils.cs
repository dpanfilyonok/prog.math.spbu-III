using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ProtocolSource;

namespace ServerSource
{
    /// <summary>
    /// Utils methods for <see cref="SimpleFTPServer"/>
    /// </summary>
    internal static class SimpleFTPServerUtils
    {
        /// <summary>
        /// Parse request in format "int string" to pair of request method and path
        /// </summary>
        /// <exception cref="ArgumentNullException">'request' is null</exception>
        /// <returns>(request method, path)</returns>
        internal static (Methods, string) ParseRequest(string request)
        {
            if (request == null)
            {
                throw new ArgumentNullException();
            }

            var splited = request.Split();
            return ((Methods)Convert.ToInt32(splited[0]), splited[1]);
        }

        /// <summary>
        /// Returns content of mentioned directory as list of (string, bool)
        /// </summary>
        /// <param name="pathToDir">Path to diectory</param>
        /// <exception cref="DirectoryNotFoundException"></exception>
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

        /// <summary>
        /// Create response string from list of (string, bool)
        /// </summary>
        /// <returns>Response string for List method</returns>
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

        /// <summary>
        /// Returns filestream for reading
        /// </summary>
        /// <param name="pathToFile">File for reading</param>
        /// <exception cref="FileNotFoundException"></exception>
        /// <returns>File stream</returns>
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