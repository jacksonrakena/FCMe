using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GuildChat.Server.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    apikey = table.Column<string>(name: "api_key", type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "characters",
                columns: table => new
                {
                    id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    freecompanyid = table.Column<string>(name: "free_company_id", type: "text", nullable: false),
                    accountid = table.Column<Guid>(name: "account_id", type: "uuid", nullable: false),
                    secrettext = table.Column<string>(name: "secret_text", type: "text", nullable: false),
                    verifiedat = table.Column<DateTime>(name: "verified_at", type: "timestamp with time zone", nullable: true),
                    verificationmethod = table.Column<int>(name: "verification_method", type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_characters", x => x.id);
                    table.ForeignKey(
                        name: "FK_characters_users_account_id",
                        column: x => x.accountid,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_characters_account_id",
                table: "characters",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_api_key",
                table: "users",
                column: "api_key",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "characters");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
