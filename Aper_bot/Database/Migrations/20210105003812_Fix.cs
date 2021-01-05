using Microsoft.EntityFrameworkCore.Migrations;

namespace Aper_bot.Database.Migrations
{
    public partial class Fix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quote_Guilds_GuildID",
                table: "Quote");

            migrationBuilder.AlterColumn<int>(
                name: "GuildID",
                table: "Quote",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Quote_Guilds_GuildID",
                table: "Quote",
                column: "GuildID",
                principalTable: "Guilds",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quote_Guilds_GuildID",
                table: "Quote");

            migrationBuilder.AlterColumn<int>(
                name: "GuildID",
                table: "Quote",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Quote_Guilds_GuildID",
                table: "Quote",
                column: "GuildID",
                principalTable: "Guilds",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
