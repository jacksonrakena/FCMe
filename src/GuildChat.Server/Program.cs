using System.Text.Json;
using System.Text.Json.Serialization;
using GuildChat.Server.Database;
using GuildChat.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<GuildChatContext>(o =>
{
    o.UseNpgsql("Host=localhost;UserName=guildchat;Password=guildchat;Database=guildchat;");
});
builder.Services.AddSingleton<ConnectionManager>();

var app = builder.Build();

app.UseWebSockets(new WebSocketOptions { KeepAliveInterval =  TimeSpan.FromMilliseconds((2^32)-2) });

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // app.UseSwagger();
    // app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();