namespace Source
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;

    public class SimpleFTPClient
    {
        private const int _port = 2121;

        // public Task<SimpleFTPResponseMessage> ListAsync(string ip, string path)
        // {
        //     using (var tcpClient = new TcpClient(ip, _port))
        //     {
        //         var stream = tcpClient.GetStream();
        //         var writer = new StreamWriter(stream);
        //     }
        // }

        private bool GetFilesInDir(string path, out string[] files)
        {
            if (!Directory.Exists(path))
            {   
                files = null;
                return false;
            }

            files = Directory.GetFiles(path);
            return true;
        }

        // public Task<SimpleFTPResponseMessage> GetAsync(string requestUrl, string pathToFile)
        // {
        //     Task<string> text;
        //     using (var file = new StreamReader(pathToFile, Encoding.UTF8))
        //     {
        //         text = file.ReadToEndAsync();
        //     }

        //     using (var client = new TcpClient(requestUrl, _port))
        //     {
        //         var stream = client.GetStream();
        //         var writer = new StreamWriter(stream);
        //         writer.WriteAsync(text.Result);
        //     }
        // }
    }
}