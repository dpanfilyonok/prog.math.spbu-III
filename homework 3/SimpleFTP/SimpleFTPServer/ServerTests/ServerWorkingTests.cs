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
        private const string _ip = "localhost";
        private const int _port = 2121;

        [TestInitialize]
        public void Init()
        {
            Task.Run(() =>
            {
                _server = new SimpleFTPServer(_ip, _port);
                _server.RunAsync();
            });
        }

        [TestCleanup]
        public void Cleanup()
        {
            _server.Stop();
        }

        [TestMethod]
        public void CorrectnessOfStopMethod()
        {
            // ???
        }
    }
}