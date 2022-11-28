using System.Threading.Tasks;
using Lumina.Excel.GeneratedSheets;
using RemoteQueuePop.Configuration;

namespace RemoteQueuePop.Sinks;

public interface IRemoteSink
{
    public Task StartAsync(RemoteQueuePopConfig config);

    public Task StopAsync(RemoteQueuePopConfig config);
    
    public Task HandleQueuePopAsync(ContentFinderCondition condition);
}