using System.Text.Json.Serialization;

namespace GuildChat.Common.Structures.Packets.Server;

public enum ServerEventType
{
    [JsonPropertyName("AUTHORIZE")]
    Authorize,
    
    [JsonPropertyName("MESSAGE")]
    Message
}