using System.Text.Json.Serialization;

namespace GuildChat.Common.Structures.Packets.Client;

public enum ConnectionTerminationReason
{
    Ratelimited,
    Banned,
    LostHeartbeat,
    ServerUnavailable,
    Maintenance,
    AuthorizeFailed,
    NoReauthorization,
    UnknownEvent,
    InvalidData,
    InternalServerError
}
public class GoodbyePacketData : IPacketData
{
    [JsonPropertyName("reason")]
    public ConnectionTerminationReason Reason { get; set; }
}
public class GoodbyePacket : ClientPacket<GoodbyePacketData>
{
    
}