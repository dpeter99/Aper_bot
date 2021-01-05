using Microsoft.EntityFrameworkCore.Migrations;

namespace Aper_bot.Database.Migrations
{
    public partial class QuoteNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quote_Guilds_GuildID",
                table: "Quote");

            migrationBuilder.DropForeignKey(
                name: "FK_Quote_Image_ImageID",
                table: "Quote");

            migrationBuilder.DropForeignKey(
                name: "FK_Quote_Users_CreatorID",
                table: "Quote");

            migrationBuilder.DropForeignKey(
                name: "FK_Quote_Users_SourceID",
                table: "Quote");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Quote",
                table: "Quote");

            migrationBuilder.RenameTable(
                name: "Quote",
                newName: "Quotes");

            migrationBuilder.RenameIndex(
                name: "IX_Quote_SourceID",
                table: "Quotes",
                newName: "IX_Quotes_SourceID");

            migrationBuilder.RenameIndex(
                name: "IX_Quote_ImageID",
                table: "Quotes",
                newName: "IX_Quotes_ImageID");

            migrationBuilder.RenameIndex(
                name: "IX_Quote_GuildID",
                table: "Quotes",
                newName: "IX_Quotes_GuildID");

            migrationBuilder.RenameIndex(
                name: "IX_Quote_CreatorID",
                table: "Quotes",
                newName: "IX_Quotes_CreatorID");

            migrationBuilder.AddColumn<int>(
                name: "number",
                table: "Quotes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Quotes",
                table: "Quotes",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Quotes_Guilds_GuildID",
                table: "Quotes",
                column: "GuildID",
                principalTable: "Guilds",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Quotes_Image_ImageID",
                table: "Quotes",
                column: "ImageID",
                principalTable: "Image",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Quotes_Users_CreatorID",
                table: "Quotes",
                column: "CreatorID",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Quotes_Users_SourceID",
                table: "Quotes",
                column: "SourceID",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quotes_Guilds_GuildID",
                table: "Quotes");

            migrationBuilder.DropForeignKey(
                name: "FK_Quotes_Image_ImageID",
                table: "Quotes");

            migrationBuilder.DropForeignKey(
                name: "FK_Quotes_Users_CreatorID",
                table: "Quotes");

            migrationBuilder.DropForeignKey(
                name: "FK_Quotes_Users_SourceID",
                table: "Quotes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Quotes",
                table: "Quotes");

            migrationBuilder.DropColumn(
                name: "number",
                table: "Quotes");

            migrationBuilder.RenameTable(
                name: "Quotes",
                newName: "Quote");

            migrationBuilder.RenameIndex(
                name: "IX_Quotes_SourceID",
                table: "Quote",
                newName: "IX_Quote_SourceID");

            migrationBuilder.RenameIndex(
                name: "IX_Quotes_ImageID",
                table: "Quote",
                newName: "IX_Quote_ImageID");

            migrationBuilder.RenameIndex(
                name: "IX_Quotes_GuildID",
                table: "Quote",
                newName: "IX_Quote_GuildID");

            migrationBuilder.RenameIndex(
                name: "IX_Quotes_CreatorID",
                table: "Quote",
                newName: "IX_Quote_CreatorID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Quote",
                table: "Quote",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Quote_Guilds_GuildID",
                table: "Quote",
                column: "GuildID",
                principalTable: "Guilds",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Quote_Image_ImageID",
                table: "Quote",
                column: "ImageID",
                principalTable: "Image",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Quote_Users_CreatorID",
                table: "Quote",
                column: "CreatorID",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Quote_Users_SourceID",
                table: "Quote",
                column: "SourceID",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
