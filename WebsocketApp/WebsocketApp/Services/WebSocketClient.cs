using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;

namespace WebsocketApp.Services
{
    public static class WebSocketClient
    {
        public static void SendReturnMessage(WebSocket socket, string json)
        {
            byte[] buffer;
            buffer = Encoding.UTF8.GetBytes(json);
            socket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}
