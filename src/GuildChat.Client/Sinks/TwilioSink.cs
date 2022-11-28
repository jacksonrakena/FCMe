using System.Threading.Tasks;
using GuildChat.Client.Configuration;
using Lumina.Excel.GeneratedSheets;

namespace GuildChat.Client.Sinks;

public class TwilioSink : IRemoteSink
{
    public async Task StartAsync(RemoteQueuePopConfig config)
    {
    }

    public async Task StopAsync(RemoteQueuePopConfig config)
    {
    }

    public async Task HandleQueuePopAsync(ContentFinderCondition condition)
    {
    }
}