using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PayStationSW
{
    public static class WebSocketHandler
{
    private static ConcurrentDictionary<string, WebSocket> _sockets = new ConcurrentDictionary<string, WebSocket>();

    public static async Task Handle(HttpContext context)
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
            string socketId = Guid.NewGuid().ToString();
            _sockets.TryAdd(socketId, webSocket);

            await Receive(webSocket, async (result, buffer) =>
            {
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    // Handle incoming messages if needed
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    _sockets.TryRemove(socketId, out _);
                    await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
                }
            });
        }
        else
        {
            context.Response.StatusCode = 400;
        }
    }

    private static async Task Receive(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
    {
        var buffer = new byte[1024 * 4];
        while (socket.State == WebSocketState.Open)
        {
            var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            handleMessage(result, buffer);
        }
    }

    public static async Task SendMessage(string message)
    {
        var buffer = Encoding.UTF8.GetBytes(message);
        foreach (var socket in _sockets.Values)
        {
            if (socket.State == WebSocketState.Open)
            {
                await socket.SendAsync(new ArraySegment<byte>(buffer, 0, buffer.Length), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }
}
}