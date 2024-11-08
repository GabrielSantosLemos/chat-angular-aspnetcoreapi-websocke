using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Collections.Concurrent;

namespace Chat.WebAPI.Controllers
{
    [Route("/ws/v2")]
    [ApiController]
    public class WebSocketControllerV2 : ControllerBase
    {
        private static ConcurrentDictionary<string, WebSocket> ConnectedUsers = new ConcurrentDictionary<string, WebSocket>();

        [HttpGet("{username}")]
        public async Task Get(string username)
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using WebSocket webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                ConnectedUsers.TryAdd(username, webSocket);

                // Notifica todos os usuários sobre o novo chat online
                await NotifyAllUsers();

                await HandleMessages(webSocket, username);

                ConnectedUsers.TryRemove(username, out _);

                // Notifica todos os usuários sobre a saída do chat
                await NotifyAllUsers();
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }

        private static async Task HandleMessages(WebSocket webSocket, string username)
        {
            var buffer = new byte[1024 * 4];
            while (webSocket.State == WebSocketState.Open)
            {
                var receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (receiveResult.MessageType == WebSocketMessageType.Close)
                    break;

                // Deserializa a mensagem recebida
                var messageJson = Encoding.UTF8.GetString(buffer, 0, receiveResult.Count);
                var message = JsonSerializer.Deserialize<ChatMessage>(messageJson);

                if (message != null && ConnectedUsers.TryGetValue(message.TargetUser, out WebSocket targetSocket))
                {
                    var messageToSend = JsonSerializer.Serialize(new { From = username, Text = message.Text });
                    var messageBuffer = Encoding.UTF8.GetBytes(messageToSend);

                    await targetSocket.SendAsync(new ArraySegment<byte>(messageBuffer), WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }

            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Connection closed", CancellationToken.None);
        }

        private static async Task NotifyAllUsers()
        {
            // Constrói a lista de usuários online
            var onlineUsers = ConnectedUsers.Keys.ToList();

            // Cria a mensagem JSON com a lista de usuários online
            var userListMessage = JsonSerializer.Serialize(new { Type = "userList", Users = onlineUsers });
            var buffer = Encoding.UTF8.GetBytes(userListMessage);

            // Envia a lista para todos os WebSockets conectados
            foreach (var socket in ConnectedUsers.Values)
            {
                if (socket.State == WebSocketState.Open)
                {
                    await socket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }
    }
}
