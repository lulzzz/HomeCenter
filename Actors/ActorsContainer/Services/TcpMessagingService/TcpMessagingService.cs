﻿using HomeCenter.CodeGeneration;
using HomeCenter.Model.Actors;
using HomeCenter.Model.Core;
using HomeCenter.Model.Messages.Commands.Service;
using HomeCenter.Model.Messages.Queries.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace HomeCenter.Services.Networking
{
    [ProxyCodeGenerator]
    public class TcpMessagingService : Service
    {
        [Subscibe]
        protected async Task Handle(TcpCommand tcpCommand)
        {
            using (var socket = new TcpClient())
            {
                var uri = new Uri($"tcp://{tcpCommand.Address}");
                await socket.ConnectAsync(uri.Host, uri.Port).ConfigureAwait(false);
                using (var stream = socket.GetStream())
                {
                    await stream.WriteAsync(tcpCommand.Body, 0, tcpCommand.Body.Length).ConfigureAwait(false);
                }
            }
        }

        [Subscibe]
        protected async Task<string> Handle(TcpQuery tcpCommand)
        {
            using (var socket = new TcpClient())
            {
                var uri = new Uri($"tcp://{tcpCommand.Address}");
                await socket.ConnectAsync(uri.Host, uri.Port).ConfigureAwait(false);
                using (var stream = socket.GetStream())
                {
                    await stream.WriteAsync(tcpCommand.Body, 0, tcpCommand.Body.Length).ConfigureAwait(false);
                    return await ReadString(stream).ConfigureAwait(false);
                }
            }
        }

        private static async Task<string> ReadString(NetworkStream stream)
        {
            var bytesRead = 0;
            var buffer = new byte[256];
            var result = new List<byte>();

            do
            {
                bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
                result.AddRange(buffer.Take(bytesRead));
            }
            while (bytesRead == buffer.Length);

            return System.Text.Encoding.UTF8.GetString(result.ToArray());
        }
    }
}