using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace GuildChat.Server.Database;

public class GuildChatContext : DbContext
{
    public DbSet<GuildChatAccount> Accounts { get; set; }
    
    public GuildChatContext(DbContextOptions<GuildChatContext> options)
        : base(options)
    {
    }
}