using Microsoft.EntityFrameworkCore.Migrations;

namespace Aper_bot.Database.Migrations
{
    public partial class AddGuildRulesMessage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RulesChannelId",
                table: "Guilds",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RulesMessageId",
                table: "Guilds",
                type: "longtext",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RulesChannelId",
                table: "Guilds");

            migrationBuilder.DropColumn(
                name: "RulesMessageId",
                table: "Guilds");
        }
    }
}
