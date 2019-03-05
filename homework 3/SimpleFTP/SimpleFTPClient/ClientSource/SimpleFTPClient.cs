﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using ProtocolSource;

namespace ClientSource
{
    /// <summary>
    /// Class, that provides SimpleFTP methods 
    /// </summary>
    public class SimpleFTPClient
    {
        /// <summary>
        /// Returns content of directory on server
        /// </summary>
        /// <param name="hostIp">Server remote IP</param>
        /// <param name="hostPort">Server remote port</param>
        /// <param name="path">Path to dir</param>
        /// <exception cref="SocketException"></exception>
        /// <returns>List of content</returns>
        public async Task<List<(string, bool)>> ListAsync(string hostIp, int hostPort, string path)
        {
            var request = SimpleFTPClientUtils.FormRequest(Methods.List, path);
            var response = "";

            using (var client = new TcpClient())
            {
                await client.ConnectAsync(hostIp, hostPort);
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

        /// <summary>
        /// Download file from remote server
        /// </summary>
        /// <param name="hostIp">Server remote IP</param>
        /// <param name="hostPort">Server remote port</param>
        /// <param name="path">Path to file on server</param>
        /// <param name="pathToSave">Path where to download file</param>
        /// <exception cref="SocketException"></exception>
        public async Task DownloadFileAsync(string hostIp, int hostPort, string path, string pathToSave)
        {
            var request = SimpleFTPClientUtils.FormRequest(Methods.Get, path);
            using (var client = new TcpClient())
            {
                await client.ConnectAsync(hostIp, hostPort);
                using (var stream = client.GetStream())
                {
                    var writer = new StreamWriter(stream) { AutoFlush = true };
                    await writer.WriteLineAsync(request);

                    var reader = new StreamReader(stream);
                    if (!int.TryParse(await reader.ReadLineAsync(), out int size))
                    {
                        throw new Exception("LUL");
                    }

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
    }
}