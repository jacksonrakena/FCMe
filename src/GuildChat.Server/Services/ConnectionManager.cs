﻿using System.Net.WebSockets;
using GuildChat.Common.Structures.Packets.Client;
using GuildChat.Server.Connection;
using GuildChat.Server.Connection.Grouped;
using GuildChat.Server.Database;

namespace GuildChat.Server.Services;

public class ConnectionManager
{
    private readonly IServiceProvider _services;
    private readonly ILogger<ConnectionManager> _logger;
    public ConnectionManager(IServiceProvider services, IHostApplicationLifetime lifetime)
    {
        _services = services;
        _logger = services.GetRequiredService<ILogger<ConnectionManager>>();

        lifetime.ApplicationStopping.Register(async () =>
        {
            var tasks = Connections.Select(d => d.CloseAsync(ConnectionTerminationReason.ServerUnavailable)).ToList();
            await Task.WhenAll(tasks);
            _logger.LogInformation($"Server shutdown: disconnected {tasks.Count} clients.");
        });
    }
    
    public List<ClientConnection> Connections { get; set; } = new List<ClientConnection>();
    public Dictionary<string, ConnectionGroup> Groups { get; set; } = new Dictionary<string, ConnectionGroup>();

    public async Task<ClientConnection> HandleIncomingWebSocketAsync(WebSocket socket)
    {
        var scope = _services.CreateScope();
        var connection = new ClientConnection(socket, this, scope, scope.ServiceProvider.GetRequiredService<ILogger<ClientConnection>>(), scope.ServiceProvider.GetRequiredService<GuildChatContext>());
        _logger.LogInformation("Adding connection " + connection.Id);
        Connections.Add(connection);
        await connection.HandleAsync();
        _logger.LogInformation("Removing connection " + connection.Id);
        Connections.Remove(connection);
        if (connection.Group != null)
        {
            await connection.Group.RemoveAsync(connection);
            if (connection.Group.Connections.Count == 0)
            {
                _logger.LogInformation($"Disbanding group {connection.Group.Id}");
                Groups.Remove(connection.Group.Id);
            }
        }
        return connection;
    }
}