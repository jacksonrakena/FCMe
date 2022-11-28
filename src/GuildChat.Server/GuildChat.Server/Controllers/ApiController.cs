using System.Buffers;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using GuildChat.Server.Structures;
using GuildChat.Server.Structures.Packets.Client;
using GuildChat.Server.Structures.Packets.Server;
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

    public async Task SendToSocketAsync<TData>(WebSocket socket, ClientPacket<TData> data) where TData : IPacketData
    {
        await socket.SendAsync(await data.SerializeAsync(), WebSocketMessageType.Text, true, CancellationToken.None);
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

        var helloPacket = new HelloPacket
        {
            Event = ClientEventType.Hello,
            Data = new HelloPacketData
            {
                HeartbeatIntervalMilliseconds = 45_000
            }
        };
        await SendToSocketAsync(socket, new HelloPacket());
        var authorized = false;
        using var memory = MemoryPool<byte>.Shared.Rent(1024 * 4);

        while (socket.State == WebSocketState.Open)
        {
            try
            {
                ValueWebSocketReceiveResult request = await socket.ReceiveAsync(memory.Memory, CancellationToken.None);
                switch (request.MessageType)
                {
                    case WebSocketMessageType.Text:
                        var packet = JsonDocument.Parse(memory.Memory[..request.Count]).Deserialize<ServerPacket>();
                        Console.WriteLine(packet.Event.ToString("F"));
                        switch (packet.Event)
                        {
                            case ServerEventType.Authorize:
                                var authorizeData = packet.Data.Deserialize<AuthorizePacketData>();
                                if (authorizeData.Username != "abyssal" && authorizeData.Password != "password")
                                {
                                    await SendToSocketAsync(socket, new GoodbyePacket
                                    {
                                        Data = new GoodbyePacketData { Reason = ConnectionTerminationReason.AuthorizeFailed },
                                        Event = ClientEventType.Goodbye
                                    });
                                    await socket.CloseAsync(
                                        WebSocketCloseStatus.NormalClosure,
                                        "Failed to authorize.",
                                        CancellationToken.None);
                                    return;
                                }

                                await SendToSocketAsync(socket, new ReadyPacket
                                {
                                    Event = ClientEventType.Ready,
                                    Data = new ReadyPacketData()
                                });
                                break;
                            default:
                                await SendToSocketAsync(socket, new GoodbyePacket
                                {
                                    Data = new GoodbyePacketData { Reason = ConnectionTerminationReason.LostHeartbeat },
                                    Event = ClientEventType.Goodbye
                                });
                                await socket.CloseAsync(
                                    WebSocketCloseStatus.NormalClosure,
                                    "Unknown event type.",
                                    CancellationToken.None);
                                return;
                        }
                        break;
                    default:
                        await SendToSocketAsync(socket, new GoodbyePacket
                        {
                            Data = new GoodbyePacketData { Reason = ConnectionTerminationReason.LostHeartbeat },
                            Event = ClientEventType.Goodbye
                        });
                        await socket.CloseAsync(
                            WebSocketCloseStatus.NormalClosure,
                            "Lost heartbeat.",
                            CancellationToken.None);
                        break;
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Error", e);
                await socket.CloseAsync(
                    WebSocketCloseStatus.NormalClosure,
                    "Client requested closure. Goodbye.",
                    CancellationToken.None);
                return;
            }
        }
    }
}