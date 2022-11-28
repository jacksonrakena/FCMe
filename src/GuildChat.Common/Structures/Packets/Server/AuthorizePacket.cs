using System.Text.Json.Serialization;

namespace GuildChat.Common.Structures.Packets.Server;

public class AuthorizePacketData : IPacketData
{
    [JsonPropertyName("api_key")]
    public string ApiKey { get; set; }
    
    [JsonPropertyName("character_id")]
    public string CharacterId { get; set; }

    [JsonPropertyName("client_info")]
    public AuthorizeClientInformation ClientInformation { get; set; }
}

public class AuthorizeClientInformation
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonPropertyName("author")]
    public string Author { get; set; }
    
    [JsonPropertyName("version")]
    public string Version { get; set; }
}

public class AuthorizePacket : ServerPacket
{
    
}