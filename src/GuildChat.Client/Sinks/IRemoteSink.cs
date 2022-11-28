using System.Threading.Tasks;
using GuildChat.Client.Configuration;
using Lumina.Excel.GeneratedSheets;

namespace GuildChat.Client.Sinks;

public interface IRemoteSink
{
    public Task StartAsync(RemoteQueuePopConfig config);

    public Task StopAsync(RemoteQueuePopConfig config);
    
    public Task HandleQueuePopAsync(ContentFinderCondition condition);
}