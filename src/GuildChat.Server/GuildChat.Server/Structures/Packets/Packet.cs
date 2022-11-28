using System.Text.Json;
using System.Text.Json.Serialization;

namespace GuildChat.Server.Structures.Packets;

public class Packet<TData> where TData: IPacketData
{
    [JsonPropertyName("data")]
    public TData Data { get; set; }

    public Task<ArraySegment<byte>> SerializeAsync()
    {
        return Task.FromResult(new ArraySegment<byte>(JsonSerializer.SerializeToUtf8Bytes(this, GetType())));
    }
}