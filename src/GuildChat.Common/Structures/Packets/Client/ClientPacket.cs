using System.Text.Json.Serialization;

namespace GuildChat.Common.Structures.Packets.Client;

public class ClientPacket<TData> : Packet<TData> where TData : IPacketData
{
    [JsonPropertyName("event")]
    public ClientEventType Event { get; set; }
}