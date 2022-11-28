using System.Text.Json.Serialization;
using GuildChat.Server.Structures.Packets;

namespace GuildChat.Server.Structures;

public class ClientPacket<TData> : Packet<TData> where TData : IPacketData
{
    [JsonPropertyName("event")]
    public ClientEventType Event { get; set; }
}