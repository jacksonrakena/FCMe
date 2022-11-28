using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Lumina.Excel.GeneratedSheets;
using RemoteQueuePop.Configuration;

namespace RemoteQueuePop.Sinks;

public class DiscordSink : IRemoteSink
{
    public DiscordSocketClient Client { get; set; }

    public async Task StartAsync(RemoteQueuePopConfig config)
    {
        Client = new DiscordSocketClient();
        Client.Ready += () =>
        {
            Dalamud.Logging.PluginLog.LogInformation("Ready");
            return Task.CompletedTask;
        };
        if (!string.IsNullOrWhiteSpace(config.Token))
        {
            Client.LoginAsync(TokenType.Bot, config.Token).GetAwaiter().GetResult();
            Client.StartAsync();
        }
    }

    public async Task StopAsync(RemoteQueuePopConfig config)
    {
    }

    public Task HandleQueuePopAsync(ContentFinderCondition condition)
    {
        throw new System.NotImplementedException();
    }
}