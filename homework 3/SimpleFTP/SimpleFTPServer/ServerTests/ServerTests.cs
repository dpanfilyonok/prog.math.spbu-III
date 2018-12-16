using System;
using System.Collections.Generic;
// using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServerSource;

namespace ServerTests
{
    [TestClass]
    public class ServerTests
    {
        private SimpleFTPServer _server;
        private const string _ip = "127.0.0.1";

        [TestMethod]
        public void ServerOnLocalHostShouldWork()
        {
            _server = new SimpleFTPServer("localhost");
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void TryingToUsePort0ShouldRaiseInvalidFormatException()
        {
            _server = new SimpleFTPServer(_ip, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void TryingToUseBigPortShouldRaiseInvalidFormatException()
        {
            _server = new SimpleFTPServer(_ip, 65535 + 1);
        }

        [TestMethod]
        public void GetListOfElementsInDirShouldReturnCorrectData()
        {
            var expected = new HashSet<(string, bool)>()
            {
                ("NestedFolder1", true),
                ("NestedFolder2", true),
                ("1", false),
                ("2", false),
                ("3", false)
            };
            
            var data = SimpleFTPServerUtils.GetListOfElementsInDir(@"../../../TestFolder");
            var actual = new HashSet<(string, bool)>(data);

            Assert.IsTrue(expected.IsSubsetOf(actual) && actual.IsSubsetOf(expected));
        }
    }
}
