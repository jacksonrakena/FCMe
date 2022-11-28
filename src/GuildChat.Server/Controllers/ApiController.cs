using System.Buffers;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using GuildChat.Common.Structures;
using GuildChat.Common.Structures.Packets.Client;
using GuildChat.Common.Structures.Packets.Server;
using GuildChat.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace GuildChat.Server.Controllers;

[ApiController]
[Route("api")]
public class ApiController : ControllerBase
{
    private readonly ILogger<ApiController> _logger;
    private readonly ConnectionManager _connectionManager;

    public ApiController(ILogger<ApiController> logger, ConnectionManager connectionManager)
    {
        _connectionManager = connectionManager;
        _logger = logger;
    }

    [Route("start")]
    public async Task<IActionResult> Get()
    {
        if (!HttpContext.WebSockets.IsWebSocketRequest)
            return BadRequest(new { error = "Expected a WebSocket request. " });
        
        var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
        await _connectionManager.HandleIncomingWebSocketAsync(webSocket);
        return new EmptyResult();
    }
}