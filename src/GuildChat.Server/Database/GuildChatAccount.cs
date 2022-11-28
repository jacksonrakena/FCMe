using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GuildChat.Server.Database;

[Table("users")]
[Index(nameof(ApiKey), IsUnique=true)]
public class GuildChatAccount
{
    [Key, Column("id")]
    public Guid Id { get; set; }

    [Column("api_key")]
    public string ApiKey { get; set; }
    
    public List<GuildChatGameCharacter> Characters { get; set; }
}

[Table("characters")]
public class GuildChatGameCharacter
{
    [Key, Column("id")]
    public ulong Id { get; set; }
    
    [Column("free_company_id")]
    public string FreeCompanyId { get; set; }
    
    [Column("account_id")]
    public Guid AccountId { get; set; }
    public GuildChatAccount Account { get; set; }
    
    [Column("secret_text")]
    public string? SecretText { get; set; }

    [NotMapped] public bool IsVerified => VerificationMethod != GuildChatVerificationMethod.Unverified;
    
    [Column("verified_at")] public DateTime? VerifiedAt { get; set; } = null;
    
    [Column("verification_method")] public GuildChatVerificationMethod VerificationMethod { get; set; } = GuildChatVerificationMethod.Unverified;
}

public enum GuildChatVerificationMethod
{
    Unverified,
    ByAdmin,
    ByTextMatch
}