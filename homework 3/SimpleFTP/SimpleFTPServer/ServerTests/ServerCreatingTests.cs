using System;
using System.Collections.Generic;
// using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServerSource;

namespace ServerTests
{
    /// <summary>
    /// Тесты, проверяющие корректность создания сервера
    /// </summary>
    [TestClass]
    public class ServerCreatingTests
    {
        private SimpleFTPServer _server;
        private const string _ip = "127.0.0.1";

        /// <summary>
        /// Проверяет возможность запустить сервер на localhost
        /// </summary>
        [TestMethod]
        public void ServerOnLocalHostShouldWork()
        {
            _server = new SimpleFTPServer("localhost");
        }


        /// <summary>
        /// Номер порта должен быть меньше 65535
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void TryingToUseBigPortShouldRaiseInvalidFormatException()
        {
            _server = new SimpleFTPServer(_ip, 65535 + 1);
        }
    }
}
