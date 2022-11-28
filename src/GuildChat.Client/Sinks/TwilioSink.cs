using System.Threading.Tasks;
using Lumina.Excel.GeneratedSheets;
using RemoteQueuePop.Configuration;

namespace RemoteQueuePop.Sinks;

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