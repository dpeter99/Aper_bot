using Microsoft.EntityFrameworkCore.Migrations;

namespace Aper_bot.Database.Migrations
{
    public partial class AddSchemas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GuildPermissionLevel_Guilds_GuildID",
                table: "GuildPermissionLevel");

            migrationBuilder.DropForeignKey(
                name: "FK_GuildRule_Guilds_GuildID",
                table: "GuildRule");

            migrationBuilder.DropForeignKey(
                name: "FK_Quotes_Guilds_GuildID",
                table: "Quotes");

            migrationBuilder.DropForeignKey(
                name: "FK_Quotes_Image_ImageID",
                table: "Quotes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Image",
                table: "Image");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Guilds",
                table: "Guilds");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GuildRule",
                table: "GuildRule");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GuildPermissionLevel",
                table: "GuildPermissionLevel");

            migrationBuilder.EnsureSchema(
                name: "Aper_Bot");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "Users",
                newSchema: "Aper_Bot");

            migrationBuilder.RenameTable(
                name: "Quotes",
                newName: "Quotes",
                newSchema: "Aper_Bot");

            migrationBuilder.RenameTable(
                name: "Image",
                newName: "Images",
                newSchema: "Aper_Bot");

            migrationBuilder.RenameTable(
                name: "Guilds",
                newName: "Guild",
                newSchema: "Aper_Bot");

            migrationBuilder.RenameTable(
                name: "GuildRule",
                newName: "GuildRules",
                newSchema: "Aper_Bot");

            migrationBuilder.RenameTable(
                name: "GuildPermissionLevel",
                newName: "GuildPermissionLevels",
                newSchema: "Aper_Bot");

            migrationBuilder.RenameIndex(
                name: "IX_GuildRule_GuildID",
                schema: "Aper_Bot",
                table: "GuildRules",
                newName: "IX_GuildRules_GuildID");

            migrationBuilder.RenameIndex(
                name: "IX_GuildPermissionLevel_GuildID",
                schema: "Aper_Bot",
                table: "GuildPermissionLevels",
                newName: "IX_GuildPermissionLevels_GuildID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Images",
                schema: "Aper_Bot",
                table: "Images",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Guild",
                schema: "Aper_Bot",
                table: "Guild",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GuildRules",
                schema: "Aper_Bot",
                table: "GuildRules",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GuildPermissionLevels",
                schema: "Aper_Bot",
                table: "GuildPermissionLevels",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_GuildPermissionLevels_Guild_GuildID",
                schema: "Aper_Bot",
                table: "GuildPermissionLevels",
                column: "GuildID",
                principalSchema: "Aper_Bot",
                principalTable: "Guild",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GuildRules_Guild_GuildID",
                schema: "Aper_Bot",
                table: "GuildRules",
                column: "GuildID",
                principalSchema: "Aper_Bot",
                principalTable: "Guild",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Quotes_Guild_GuildID",
                schema: "Aper_Bot",
                table: "Quotes",
                column: "GuildID",
                principalSchema: "Aper_Bot",
                principalTable: "Guild",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Quotes_Images_ImageID",
                schema: "Aper_Bot",
                table: "Quotes",
                column: "ImageID",
                principalSchema: "Aper_Bot",
                principalTable: "Images",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GuildPermissionLevels_Guild_GuildID",
                schema: "Aper_Bot",
                table: "GuildPermissionLevels");

            migrationBuilder.DropForeignKey(
                name: "FK_GuildRules_Guild_GuildID",
                schema: "Aper_Bot",
                table: "GuildRules");

            migrationBuilder.DropForeignKey(
                name: "FK_Quotes_Guild_GuildID",
                schema: "Aper_Bot",
                table: "Quotes");

            migrationBuilder.DropForeignKey(
                name: "FK_Quotes_Images_ImageID",
                schema: "Aper_Bot",
                table: "Quotes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Images",
                schema: "Aper_Bot",
                table: "Images");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GuildRules",
                schema: "Aper_Bot",
                table: "GuildRules");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GuildPermissionLevels",
                schema: "Aper_Bot",
                table: "GuildPermissionLevels");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Guild",
                schema: "Aper_Bot",
                table: "Guild");

            migrationBuilder.RenameTable(
                name: "Users",
                schema: "Aper_Bot",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "Quotes",
                schema: "Aper_Bot",
                newName: "Quotes");

            migrationBuilder.RenameTable(
                name: "Images",
                schema: "Aper_Bot",
                newName: "Image");

            migrationBuilder.RenameTable(
                name: "GuildRules",
                schema: "Aper_Bot",
                newName: "GuildRule");

            migrationBuilder.RenameTable(
                name: "GuildPermissionLevels",
                schema: "Aper_Bot",
                newName: "GuildPermissionLevel");

            migrationBuilder.RenameTable(
                name: "Guild",
                schema: "Aper_Bot",
                newName: "Guilds");

            migrationBuilder.RenameIndex(
                name: "IX_GuildRules_GuildID",
                table: "GuildRule",
                newName: "IX_GuildRule_GuildID");

            migrationBuilder.RenameIndex(
                name: "IX_GuildPermissionLevels_GuildID",
                table: "GuildPermissionLevel",
                newName: "IX_GuildPermissionLevel_GuildID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Image",
                table: "Image",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GuildRule",
                table: "GuildRule",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GuildPermissionLevel",
                table: "GuildPermissionLevel",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Guilds",
                table: "Guilds",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_GuildPermissionLevel_Guilds_GuildID",
                table: "GuildPermissionLevel",
                column: "GuildID",
                principalTable: "Guilds",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GuildRule_Guilds_GuildID",
                table: "GuildRule",
                column: "GuildID",
                principalTable: "Guilds",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

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
        }
    }
}
