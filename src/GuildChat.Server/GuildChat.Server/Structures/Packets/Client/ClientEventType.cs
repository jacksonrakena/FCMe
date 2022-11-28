using System.Text.Json.Serialization;

namespace GuildChat.Server.Structures;

public enum ClientEventType
{
    [JsonPropertyName("HELLO")]
    Hello,
    
    [JsonPropertyName("GOODBYE")]
    Goodbye,
    
    [JsonPropertyName("READY")]
    Ready,
}