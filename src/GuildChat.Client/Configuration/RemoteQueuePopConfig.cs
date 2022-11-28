using Dalamud.Configuration;

namespace GuildChat.Client.Configuration
{
    public class RemoteQueuePopConfig : IPluginConfiguration
    {
        public int Version { get; set; } = 1;

        public string Token = "";
    }
}