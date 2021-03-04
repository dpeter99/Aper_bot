using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Aper_bot.Modules.DiscordSlash.Database.Migrations
{
    public partial class InitCommands : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "SlashCommands");

            migrationBuilder.CreateTable(
                name: "Commands",
                schema: "SlashCommands",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: false),
                    CommandID = table.Column<string>(type: "longtext", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    GuildID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Commands", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Commands_Guild_GuildID",
                        column: x => x.GuildID,
                        principalSchema: "Aper_Bot",
                        principalTable: "Guild",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Commands_GuildID",
                schema: "SlashCommands",
                table: "Commands",
                column: "GuildID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Commands",
                schema: "SlashCommands");
        }
    }
}
