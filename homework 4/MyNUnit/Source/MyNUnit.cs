using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Source.Attributes;

namespace Source
{
    public static class MyNUnit
    {
        public static void RunTests(string pathToDir)
        {
            var types = GetAssembliesInDir(pathToDir);
            Parallel.ForEach(types);
        }

        private static IEnumerable<Type> GetAssembliesInDir(string pathToDir)
        {
            var dirInfo = new DirectoryInfo(pathToDir);
            return dirInfo.EnumerateFiles()
              .Where(fileInfo => fileInfo.Extension == "dll")
              .Select(fileInfo => Assembly.LoadFrom(fileInfo.FullName))
              .ToHashSet()
              .SelectMany(assembly => assembly.ExportedTypes);
        }
    }
}