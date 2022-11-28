﻿using System.Text.Json;
using System.Text.Json.Serialization;

namespace GuildChat.Server.Structures.Packets.Server;

public class ServerPacket
{
    [JsonPropertyName("event")]
    public ServerEventType Event { get; set; }
    
    [JsonPropertyName("data")]
    public JsonDocument Data { get; set; }
}