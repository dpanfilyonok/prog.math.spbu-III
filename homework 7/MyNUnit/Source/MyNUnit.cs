using System.Collections.Generic;
using System.IO;

namespace Source
{
    public static class MyNUnit
    {
        public static void GetListOfAssembliesInDirRecursively(string pathToDir)
        {
            if (!Directory.Exists(pathToDir))
            {
                throw new DirectoryNotFoundException();
            }

            
        }

        private static void GetAssembliesInDir(string pathToDir)
        {
            var dirInfo = new DirectoryInfo(pathToDir);
            var assemblies = new List<FileInfo>();
            foreach (var file in dirInfo.EnumerateFiles())
            {
                if (file.Extension == "dll" || file.Extension == "exe")
                {

                }
            }
        }
    }
}