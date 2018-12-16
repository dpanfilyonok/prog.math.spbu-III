using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClientSource;
using ServerSource;
using System.Threading;
using System.IO;

namespace ClientTests
{
    [TestClass]
    public class ClientTests
    {
        private SimpleFTPClient _client;
        private SimpleFTPServer _server;
        private Thread _serverThread;
        private const string _ip = "127.0.0.1";
        private const int _port = 2121;

        [TestInitialize]
        public void Init()
        {
            _client = new SimpleFTPClient();
            _serverThread = new Thread(() => {
                _server = new SimpleFTPServer(_ip, _port);
                _server.Start();
            });

            _serverThread.Start();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _server.Stop();
            _serverThread.Abort();
        }

        [TestMethod]
        [ExpectedException(typeof(DirectoryNotFoundException))]
        public async void TryingToMakeRequestToNonexistentFolderShouldRaiseException()
        {
            await _client.ListAsync(_ip, _port, "../../../TestFolderNonexistent");
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public async void TryingToDownloadNonexistentFileShouldRaiseException()
        {
            await _client.GetFileAsync(_ip, _port, "../../../TestFolder/nonexistent", ".");
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public async void TryingToGetByteArrayOfNonexistentFileShouldRaiseException()
        {
            await _client.GetByteArrayAsync(_ip, _port, "../../../TestFolder/nonexistent");
        }
    }
}
