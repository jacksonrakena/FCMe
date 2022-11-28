using System.Text.Json.Serialization;

namespace GuildChat.Server.Structures.Packets.Client;

public enum ConnectionTerminationReason
{
    Ratelimited,
    Banned,
    LostHeartbeat,
    ServerUnavailable,
    Maintenance,
    AuthorizeFailed
}
public class GoodbyePacketData : IPacketData
{
    [JsonPropertyName("reason")]
    public ConnectionTerminationReason Reason { get; set; }
}
public class GoodbyePacket : ClientPacket<GoodbyePacketData>
{
    
}