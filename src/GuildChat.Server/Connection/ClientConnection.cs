using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Net.WebSockets;
using System.Text.Json;
using GuildChat.Common.Structures;
using GuildChat.Common.Structures.Packets.Client;
using GuildChat.Common.Structures.Packets.Server;
using GuildChat.Server.Connection.Grouped;
using GuildChat.Server.Database;
using GuildChat.Server.Services;
using Microsoft.EntityFrameworkCore;

namespace GuildChat.Server.Connection;

public class ClientConnection : IDisposable
{
    public ClientConnection(WebSocket socket, ConnectionManager owner, IServiceScope scope, ILogger<ClientConnection> logger, GuildChatContext database)
    {
        WebSocket = socket;
        Authorized = false;
        AuthorizedAccount = null;
        AuthorizedGameCharacter = null;
        _database = database;
        _logger = logger;
        _scope = scope;
        _owner = owner;
    }

    private readonly ILogger<ClientConnection> _logger;
    private readonly GuildChatContext _database;
    private readonly IServiceScope _scope;
    private readonly ConnectionManager _owner;
    public Guid Id { get; set; } = Guid.NewGuid();
    public WebSocket WebSocket { get; set; }
    
    public ConnectionGroup? Group { get; set; }
    public bool Authorized { get; set; }
    
    public DateTime Opened { get; set; } = DateTime.Now;

    public IMemoryOwner<byte> SocketMemoryBuffer { get; set; } = MemoryPool<byte>.Shared.Rent(1024 * 4);

    public GuildChatAccount? AuthorizedAccount { get; set; }
    public GuildChatGameCharacter? AuthorizedGameCharacter { get; set; }

    public async Task<bool> TryAuthorizeAsync(AuthorizePacketData request)
    {
        if (string.IsNullOrWhiteSpace(request.ApiKey)) return false;
        if (string.IsNullOrWhiteSpace(request.CharacterId) ||
            !ulong.TryParse(request.CharacterId, out var characterId)) return false;

        var account = await _database.Accounts
            .Include(d => d.Characters)
            .FirstOrDefaultAsync(e => e.ApiKey == request.ApiKey);
        
        if (account == null) return false;
        if (account.Characters.Count == 0) return false;

        AuthorizedGameCharacter = account.Characters.FirstOrDefault(d => d.Id == characterId);
        if (AuthorizedGameCharacter == null) return false;
        
        Authorized = true;
        AuthorizedAccount = account;
        return true;
    }

    public async Task CloseAsync(ConnectionTerminationReason reason)
    {
        _logger.LogInformation($"Closing connection {Id}: {reason:F}");
        await SendToSocketAsync(WebSocket, new GoodbyePacket
        {
            Data = new GoodbyePacketData { Reason = reason },
            Event = ClientEventType.Goodbye
        });
        await WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, reason.ToString("F"), CancellationToken.None);
        this.Dispose();
    }

    public async Task HandleAsync()
    {
        while (WebSocket.State == WebSocketState.Open)
        {
            try
            {
                var request = await WebSocket.ReceiveAsync(SocketMemoryBuffer.Memory, CancellationToken.None);

                if (WebSocket.State != WebSocketState.Open) return;
                
                if (request.MessageType != WebSocketMessageType.Text)
                {
                    await CloseAsync(ConnectionTerminationReason.InvalidData);
                    return;
                }
                
                var packet = JsonDocument.Parse(SocketMemoryBuffer.Memory[..request.Count]).Deserialize<ServerPacket>();
                if (packet == null)
                {
                    await CloseAsync(ConnectionTerminationReason.InvalidData);
                    return;
                }

                switch (packet.Event)
                {
                    // Authorize
                    case ServerEventType.Authorize:
                        // Re-authorization is not permitted.
                        if (Authorized)
                        {
                            await CloseAsync(ConnectionTerminationReason.NoReauthorization);
                            return;
                        }

                        // Attempt to authorize.
                        var authorizeData = packet.Data?.Deserialize<AuthorizePacketData>();
                        if (authorizeData == null || !await TryAuthorizeAsync(authorizeData))
                        {
                            await CloseAsync(ConnectionTerminationReason.AuthorizeFailed);
                            return;
                        }

                        _logger.LogInformation($"Successfully authorized connection {Id} as {AuthorizedAccount!.Id}");
                        _logger.LogInformation(
                            $"Authorized {Id} as {AuthorizedGameCharacter?.Id} with verification status {AuthorizedGameCharacter?.VerificationMethod:F}");

                        if (!_owner.Groups.TryGetValue(AuthorizedGameCharacter.FreeCompanyId, out var group))
                        {
                            group = new ConnectionGroup(AuthorizedGameCharacter.FreeCompanyId, this,
                                _scope.ServiceProvider.GetRequiredService<ILogger<ConnectionGroup>>());
                            _owner.Groups[AuthorizedGameCharacter.FreeCompanyId] = group;
                            _logger.LogInformation($"Formed group {group.Id}");
                        }
                        else
                        {
                            _logger.LogInformation($"{Id} has joined group {group.Id}.");
                            group.Connections.Add(this);
                        }

                        Group = group;

                        // Ready to receive chat messages.
                        await SendToSocketAsync(WebSocket, new ReadyPacket
                        {
                            Event = ClientEventType.Ready,
                            Data = new ReadyPacketData()
                        });
                        break;

                    case ServerEventType.Message:
                        if (Group == null) break;
                        if (Group.Host?.Id != Id) break;
                        
                        var messagePacketData = packet.Data?.Deserialize<MessagePacketData>();
                        if (messagePacketData == null)
                        {
                            await CloseAsync(ConnectionTerminationReason.InvalidData);
                            return;
                        }

                        await new HttpClient().PostAsync(
                            "",
                            JsonContent.Create(new
                            {
                                content = messagePacketData.Content,
                                username = messagePacketData.Author.Name,
                                avatar_url = messagePacketData.Author.AvatarUrl
                            }));

                break;
                    
                    // Unknown event type.
                    default:
                        await CloseAsync(ConnectionTerminationReason.UnknownEvent);
                        return;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Internal server error in client connection loop:");
                if (WebSocket.State == WebSocketState.Open || WebSocket.State == WebSocketState.CloseReceived) await CloseAsync(ConnectionTerminationReason.InternalServerError);
                return;
            }
        }
    }
    public static async Task SendToSocketAsync<TData>(WebSocket socket, ClientPacket<TData> data) where TData : IPacketData
    {
        await socket.SendAsync(await data.SerializeAsync(), WebSocketMessageType.Text, true, CancellationToken.None);
    }

    public void Dispose()
    {
        _scope.Dispose();
        _database.Dispose();
        WebSocket.Dispose();
        SocketMemoryBuffer.Dispose();
    }
}