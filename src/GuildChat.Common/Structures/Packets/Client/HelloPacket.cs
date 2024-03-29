﻿using System.Text.Json.Serialization;

namespace GuildChat.Common.Structures.Packets.Client;

public class HelloPacketData : IPacketData
{
    [JsonPropertyName("heartbeat_interval")]
    public ulong HeartbeatIntervalMilliseconds { get; set; }
}

public class HelloPacket : ClientPacket<HelloPacketData>
{
}