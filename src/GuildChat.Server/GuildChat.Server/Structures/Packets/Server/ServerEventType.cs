using System.Text.Json.Serialization;

namespace GuildChat.Server.Structures.Packets.Server;

public enum ServerEventType
{
    [JsonPropertyName("AUTHORIZE")]
    Authorize
}