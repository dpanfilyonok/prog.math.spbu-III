namespace ClientTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Net.Sockets;
    using ClientSource;
    using System.Threading.Tasks;

    /// <summary>
    /// Тесты, проверяющие корректность работы клиента вне зависимости от состояния сервера
    /// </summary>
    [TestClass]
    public class ClientSideTests
    {
        private const string _ip = "127.0.0.1";
        private const int _port = 2121;

        /// <summary>
        /// Попытка подключения к неработающему серверу должны вызывать ошибку
        /// </summary>
        [TestMethod]
        public async Task TryingToConnectToDisabledServerShouldRaiseSocketExceptionAsync()
        {
            var client = new SimpleFTPClient();
            try
            {
                await client.ListAsync(_ip, _port, "soasla");
            }
            catch (SocketException) { }
        }
    }
}