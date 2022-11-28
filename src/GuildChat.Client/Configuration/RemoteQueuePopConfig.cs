using Dalamud.Configuration;

namespace RemoteQueuePop.Configuration
{
    public class RemoteQueuePopConfig : IPluginConfiguration
    {
        public int Version { get; set; } = 1;

        public string Token = "";
    }
}