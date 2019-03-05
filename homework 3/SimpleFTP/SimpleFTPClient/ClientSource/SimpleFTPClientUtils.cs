using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using ClientSource.Exceptions;
using ProtocolSource;

namespace ClientSource
{
    /// <summary>
    /// Utils for <see cref="SimpleFTPClient"/>
    /// </summary>
    internal static class SimpleFTPClientUtils
    {
        /// <summary>
        /// Form string request from method and path
        /// </summary>
        /// <param name="method">Request method</param>
        /// <param name="path">Path</param>
        /// <returns>Request string</returns>
        internal static string FormRequest(Methods method, string path)
        {
            return $"{(int)method} {path}";
        }

        /// <summary>
        /// Convert string IP and int port to IPEndPoint object
        /// </summary>
        /// <param name="hostIp">Converted ip</param>
        /// <param name="hostPort">Converted port</param>
        internal static IPEndPoint ConvertToEndPoint(string hostIp, int hostPort)
        {
            if (!IPAddress.TryParse(hostIp, out IPAddress ip))
            {
                ip = Dns.GetHostEntry(hostIp).AddressList[0];
            }

            if (hostPort < UInt16.MinValue || hostPort > UInt16.MaxValue)
            {
                throw new FormatException($"{nameof(hostPort)} is invalid");
            }

            return new IPEndPoint(ip, hostPort);
        }

        /// <summary>
        /// Parses string response from list method to list of (string, bool)
        /// </summary>
        /// <exception cref="DirectoryNotFoundException">If response == -1</exception>
        /// <exception cref="InvalidResponseFormatException">If response has invalid format</exception>
        internal static List<(string, bool)> ParseListResponse(string content)
        {
            var splited = content.Trim().Split('&');
            if (!int.TryParse(splited[0], out int size))
            {
                throw new InvalidResponseFormatException(
                    @"Response should be in following form:
                        <size: Int> (<name: String> <isDir: Boolean>)*,
                        but first argument is not typeof int"
                );
            }

            if (size == -1)
            {
                throw new DirectoryNotFoundException("Directory don`t exist on server");
            }

            var listOfContent = new List<(string, bool)>();

            bool convertToBool(string flag)
            {
                if (!Boolean.TryParse(flag, out bool result))
                {
                    throw new InvalidResponseFormatException(
                        @"Response should be in following form:
                            <size: Int> (<name: String> <isDir: Boolean>)*,
                            but third argument is not typeof bool"
                    );
                }

                return result;
            };

            for (int i = 1; i < size * 2 + 1; i += 2)
            {
                listOfContent.Add((splited[i], convertToBool(splited[i + 1])));
            }

            return listOfContent;
        }
    }
}
