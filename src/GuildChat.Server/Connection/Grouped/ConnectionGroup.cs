using System.Net.WebSockets;

namespace GuildChat.Server.Connection.Grouped;

public class ConnectionGroup
{
    public string Id { get; set; }
    public List<ClientConnection> Connections { get; set; }
    public ClientConnection? Host { get; set; } = null;

    private readonly ILogger<ConnectionGroup> _logger;

    public ConnectionGroup(string id, ClientConnection initial, ILogger<ConnectionGroup> logger)
    {
        Id = id;
        Connections = new List<ClientConnection> { initial };
        Host = initial;
        _logger = logger;
    }

    public async Task FindNewHostAsync()
    {
        foreach (var connection in Connections)
        {
            if (connection.Authorized && connection.WebSocket.State == WebSocketState.Open)
            {
                Host = connection;
                _logger.LogInformation($"Group {Id} has found new host {Host?.Id}.");
            }
        }
    }
    
    public async Task RemoveAsync(ClientConnection connection)
    {
        Connections.Remove(connection);
        // Only find a new host if this group has more than one person.
        // Otherwise, disband.
        if (Host?.Id == connection.Id && Connections.Count > 0)
        {
            _logger.LogInformation($"Host {Host?.Id} has left group {Id}. Searching for new host.");
            await FindNewHostAsync();
        }
        else
        {
        }
    }
}