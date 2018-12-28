using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClientSource;
using ServerSource;
using System.Threading;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClientTests
{
    /// <summary>
    /// Тесты, проверяющие корректность взаимодействия клинта и сервера
    /// </summary>
    [TestClass]
    public class ServerIntegrationTests
    {
        private SimpleFTPClient _client;
        private SimpleFTPServer _server;
        private const string _ip = "localhost";
        private const int _port = 2120;

        [TestInitialize]
        public void Init()
        {
            _client = new SimpleFTPClient();
            var task = Task.Factory.StartNew(() =>
            {
                _server = new SimpleFTPServer(_ip, _port);
                _server.RunAsync();
            });

            Thread.Sleep(1000);
        }

        /// <summary>
        /// Обращение к несуществующей папке должно вызывать ошибку
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        [ExpectedException(typeof(DirectoryNotFoundException))]
        public async Task TryingToMakeRequestToNonexistentFolderShouldRaiseException()
        {
            await _client.ListAsync(_ip, _port, "../ServerTests/TestFolderNonexistent");
        }

        /// <summary>
        /// Попытка скачать несуществующий файл должна вызывать ошибку
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public async Task TryingToDownloadNonexistentFileShouldRaiseException()
        {
            await _client.DownloadFileAsync(_ip, _port, "../ServerTests/TestFolder/nonexistent", ".");
        }

        /// <summary>
        /// List должен возвращать список содержимого в папке
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task CorrectnessOfListMethodRequest()
        {
            var expected = new HashSet<(string, bool)>()
            {
                ("NestedFolder1", true),
                ("NestedFolder2", true),
                ("1", false),
                ("2", false),
                ("3", false)
            };

            var response = await _client.ListAsync(_ip, _port, @"../ServerTests/TestFolder");
            var actual = new HashSet<(string, bool)>(response);

            Assert.IsTrue(expected.IsSubsetOf(actual) && actual.IsSubsetOf(expected));
        }

        /// <summary>
        /// При скачивании файла он действительно создается
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task CorrectnessOfDownloadMethod()
        {
            var destinationPath = "../../../DownloadedFiles/123.jpg";
            await _client.DownloadFileAsync(
                _ip,
                _port,
                "../ServerTests/TestFolder/NestedFolder1/img.jpg",
                destinationPath);
            Assert.IsTrue(File.Exists(destinationPath));
            File.Delete(destinationPath);
        }

        /// <summary>
        /// Скачивание 1 и того же файла не приводит к ошибке
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public void DownloadingSingleFileFrom2RequestsShouldWorkCorrect()
        {
            var destinationPath1 = "../../../DownloadedFiles/1.jpg";
            var destinationPath2 = "../../../DownloadedFiles/2.jpg";

            var t1 = _client.DownloadFileAsync(
                _ip,
                _port,
                "../ServerTests/TestFolder/NestedFolder1/img.jpg",
                destinationPath1);
            var t2 = _client.DownloadFileAsync(
                _ip,
                _port,
                "../ServerTests/TestFolder/NestedFolder1/img.jpg",
                destinationPath2);

            Task.WaitAll(t1, t2);

            Assert.IsTrue(File.Exists(destinationPath1));
            Assert.IsTrue(File.Exists(destinationPath2));
            File.Delete(destinationPath1);
            File.Delete(destinationPath2);
        }

        /// <summary>
        /// Стресс тест
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task ServerStressTest()
        {
            var expectedLength = 1000;
            var listOfResponses = new List<List<(string, bool)>>();
            for (int i = 0; i < expectedLength; ++i)
            {
                listOfResponses.Add(await _client.ListAsync(_ip, _port, @"../ServerTests/TestFolder"));
            }

            Assert.AreEqual(expectedLength, listOfResponses.Count);
        }
    }
}
