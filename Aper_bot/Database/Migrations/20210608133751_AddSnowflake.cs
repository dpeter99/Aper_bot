using Microsoft.EntityFrameworkCore.Migrations;

namespace Aper_bot.Database.Migrations
{
    public partial class AddSnowflake : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "GuildID",
                schema: "Aper_Bot",
                table: "Guild",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "GuildID",
                schema: "Aper_Bot",
                table: "Guild",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
