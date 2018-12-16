namespace ServerTests
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ServerSource;

    /// <summary>
    /// Тесты, проверяющие работу запущенного сервера
    /// </summary>
    [TestClass]
    public class ServerWorkingTests
    {
        private SimpleFTPServer _server;
        private Thread _serverThread;
        private const string _ip = "127.0.0.1";
        private const int _port = 2121;

        [TestInitialize]
        public void Init()
        {
            _serverThread = new Thread(() =>
            {
                _server = new SimpleFTPServer(_ip, _port);
                _server.Start();
            });

            _serverThread.Start();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _server.Stop();
            _serverThread.Join();
        }

        [TestMethod]
        public void CorrectnessOfStopMethod()
        {
            // ???
        }
    }
}