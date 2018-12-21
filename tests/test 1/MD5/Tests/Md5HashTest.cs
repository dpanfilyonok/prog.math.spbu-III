using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Diagnostics;
using System.Text;
using Source;

namespace Tests
{
    [TestClass]
    public class Md5HashTest
    {
        private static string ManualMd5FromStr(MD5 hash)
        {
            var pathToDir = @"./Tests/resources/dir1";
            var strBuilder = new StringBuilder();

            strBuilder.Append(Md5HashSingleThread.GetMd5HashFromString(pathToDir, hash));
            strBuilder.Append(Md5HashSingleThread.GetMd5HashFromString(pathToDir + "/1.txt", hash));
            strBuilder.Append(Md5HashSingleThread.GetMd5HashFromString(pathToDir + "/2.txt", hash));
            strBuilder.Append(Md5HashSingleThread.GetMd5HashFromString(pathToDir + "/1", hash));
            strBuilder.Append(Md5HashSingleThread.GetMd5HashFromString(pathToDir + "/1/1.txt", hash));
            strBuilder.Append(Md5HashSingleThread.GetMd5HashFromString(pathToDir + "/2", hash));
            return Md5HashSingleThread.GetMd5HashFromString(strBuilder.ToString(), hash);

        }

        [TestMethod]
        public void CorrectnessCheck()
        {
            var pathToDir = @"./Tests/resources/dir1";
            using (MD5 md5Hash = MD5.Create())
            {
                Assert.AreEqual(ManualMd5FromStr(md5Hash), Md5HashSingleThread.GetMd5FromDir(pathToDir, md5Hash));
            }
        }
    }
}
