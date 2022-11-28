using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace GuildChat.Server.Controllers;

[ApiController]
[Route("api")]
public class ApiController : ControllerBase
{
    private readonly ILogger<ApiController> _logger;

    public ApiController(ILogger<ApiController> logger)
    {
        _logger = logger;
    }

    [Route("start")]
    public async Task<IActionResult> Get()
    {
        _logger.LogInformation("Received request.");
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            _logger.LogInformation("Received WebSocket request");
            await Accept(webSocket);
            return new EmptyResult();
        }
        else
        {
            return BadRequest(new { error = "Expected a WebSocket request. " });
        }
    }

    public async Task SendToSocketAsync(WebSocket socket, string data)
    {
        
    }

    public async Task Accept(WebSocket socket)
    {
        dynamic data = new
        {
            @e = "HELLO",
            d = new
            {
                reply = new Random().Next(0,100)
            }
        };

        await socket.SendAsync(new ArraySegment<byte>(JsonSerializer.SerializeToUtf8Bytes(data)), WebSocketMessageType.Text, true, CancellationToken.None);

        var buffer = new byte[1024 * 4];
        var receiveResult = await socket.ReceiveAsync(
            new ArraySegment<byte>(buffer), CancellationToken.None);

        dynamic obj = JsonSerializer.Deserialize<dynamic>(buffer);
        Console.Write(obj.t);
        
        while (!receiveResult.CloseStatus.HasValue)
        {
            await socket.SendAsync(
                new ArraySegment<byte>(buffer, 0, receiveResult.Count),
                receiveResult.MessageType,
                receiveResult.EndOfMessage,
                CancellationToken.None);

            receiveResult = await socket.ReceiveAsync(
                new ArraySegment<byte>(buffer), CancellationToken.None);
        }

        await socket.CloseAsync(
            receiveResult.CloseStatus.Value,
            receiveResult.CloseStatusDescription,
            CancellationToken.None);
    }
}