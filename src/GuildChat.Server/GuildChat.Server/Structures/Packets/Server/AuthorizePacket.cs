using System.Text.Json.Serialization;

namespace GuildChat.Server.Structures.Packets.Server;

public class AuthorizePacketData : IPacketData
{
    [JsonPropertyName("username")]
    public string Username { get; set; }
    
    [JsonPropertyName("password")]
    public string Password { get; set; }
    
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