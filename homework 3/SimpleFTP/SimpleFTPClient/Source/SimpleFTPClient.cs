using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Source.Exceptions;

namespace Source
{
    public class SimpleFTPClient
    {
        #region ListAsync
        public async Task<List<(string, bool)>> ListAsync(string hostIp, int hostPort, string path)
        {
            var host = SimpleFTPClientUtils.ConvertToEndPoint(hostIp, hostPort);
            return await ListAsync(host, path);
        }

        public async Task<List<(string, bool)>> ListAsync(IPEndPoint host, string path)
        {
            var request = SimpleFTPClientUtils.FormRequest(Methods.List, path);
            var response = "";

            using (var client = new TcpClient())
            {
                try
                {
                    client.Connect(host);
                }
                catch (SocketException)
                {
                    throw;
                }

                using (var stream = client.GetStream())
                {
                    var writer = new StreamWriter(stream) { AutoFlush = true };
                    await writer.WriteLineAsync(request);

                    var reader = new StreamReader(stream);
                    response = await reader.ReadLineAsync();
                }
            }

            return SimpleFTPClientUtils.ParseListResponse(response);
        }

        #endregion

        #region GetByteArrayAsync
        public async Task<byte[]> GetByteArrayAsync(string hostIp, int hostPort, string path)
        {
            var host = SimpleFTPClientUtils.ConvertToEndPoint(hostIp, hostPort);
            return await GetByteArrayAsync(host, path);
        }

        public async Task<byte[]> GetByteArrayAsync(IPEndPoint host, string path)
        {
            var request = SimpleFTPClientUtils.FormRequest(Methods.Get, path);
            byte[] response;

            using (var client = new TcpClient())
            {
                try
                {
                    client.Connect(host);
                }
                catch (SocketException)
                {
                    throw; 
                }

                using (var stream = client.GetStream())
                {
                    var writer = new StreamWriter(stream) { AutoFlush = true };
                    await writer.WriteLineAsync(request);

                    var reader = new BinaryReader(stream);
                    int size = reader.ReadInt32();
                    if (size == -1)
                    {
                        return null;
                    }

                    response = reader.ReadBytes(size);
                }
            }

            return response;
        }

        #endregion

        #region GetFileAsync
        public async Task GetFileAsync(string hostIp, int hostPort, string path, string pathToSave)
        {
            var host = SimpleFTPClientUtils.ConvertToEndPoint(hostIp, hostPort);
            await GetFileAsync(host, path, pathToSave);
        }

        public async Task GetFileAsync(IPEndPoint host, string path, string pathToSave)
        {
            byte[] byteContent = await GetByteArrayAsync(host, path);
            using (FileStream fstream = new FileStream(pathToSave, FileMode.CreateNew))
            {
                fstream.Write(byteContent, 0, byteContent.Length);
                Console.WriteLine("Текст записан в файл");
            }
        }

        #endregion
    }
}