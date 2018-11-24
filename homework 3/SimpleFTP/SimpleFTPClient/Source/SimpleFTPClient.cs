using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Source
{
    using ListResponseType = Task<List<(string, bool)>>;
    using GetByteArrayResponseType = Task<byte[]>;
    using GetFileResponseType = Task;

    public class SimpleFTPClient
    {
        #region ListAsync
        public async ListResponseType ListAsync(string hostIp, int hostPort, string path)
        {
            var host = SimpleFTPClientUtils.ConvertToEndPoint(hostIp, hostPort);
            return await ListAsync(host, path);
        }

        public async ListResponseType ListAsync(IPEndPoint host, string path)
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
        public async GetByteArrayResponseType GetByteArrayAsync(string hostIp, int hostPort, string path)
        {
            var host = SimpleFTPClientUtils.ConvertToEndPoint(hostIp, hostPort);
            return await GetByteArrayAsync(host, path);
        }

        public async GetByteArrayResponseType GetByteArrayAsync(IPEndPoint host, string path)
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
                        throw new FileNotFoundException("File don`t exist on server", path);
                    }

                    response = reader.ReadBytes(size);
                }
            }

            return response;
        }

        #endregion
    
        #region GetFileAsync
        public async GetFileResponseType GetFileAsync(string hostIp, int hostPort, string path, string pathToSave)
        {
            var host = SimpleFTPClientUtils.ConvertToEndPoint(hostIp, hostPort);
            await GetFileAsync(host, path, pathToSave);
        }

        public async GetFileResponseType GetFileAsync(IPEndPoint host, string path, string pathToSave)
        {
            var request = SimpleFTPClientUtils.FormRequest(Methods.Get, path);
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
                        throw new FileNotFoundException("File don`t exist on server", path);
                    }
                    
                    using (var fstream = new FileStream(pathToSave, FileMode.CreateNew))
                    {
                        await stream.CopyToAsync(fstream);
                        await fstream.FlushAsync();
                    }
                }
            }
        }

        #endregion
    }
}