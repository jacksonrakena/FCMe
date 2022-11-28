using System.Text.Json.Serialization;

namespace GuildChat.Common.Structures.Packets.Client;

public enum ClientEventType
{
    [JsonPropertyName("HELLO")]
    Hello,
    
    [JsonPropertyName("GOODBYE")]
    Goodbye,
    
    [JsonPropertyName("READY")]
    Ready,
}