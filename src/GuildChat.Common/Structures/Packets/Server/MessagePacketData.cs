using System.Text.Json.Serialization;

namespace GuildChat.Common.Structures.Packets.Server;

public class MessageAuthorData
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonPropertyName("avatar_url")]
    public string AvatarUrl { get; set; }
}
public class MessagePacketData
{
    [JsonPropertyName("author")]
    public MessageAuthorData Author { get; set; }
    
    [JsonPropertyName("content")]
    public string Content { get; set; }
}