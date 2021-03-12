using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Aper_bot.Database.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Aper_Bot");

            migrationBuilder.CreateTable(
                name: "Guild",
                schema: "Aper_Bot",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: false),
                    GuildID = table.Column<string>(type: "longtext", nullable: false),
                    RulesMessageId = table.Column<long>(type: "bigint", nullable: true),
                    RulesChannelId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guild", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Images",
                schema: "Aper_Bot",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ImageData = table.Column<byte[]>(type: "longblob", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "Aper_Bot",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    UserID = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "GuildPermissionLevels",
                schema: "Aper_Bot",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    GuildID = table.Column<int>(type: "int", nullable: false),
                    RoleID = table.Column<string>(type: "longtext", nullable: false),
                    PermissionLevel = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildPermissionLevels", x => x.ID);
                    table.ForeignKey(
                        name: "FK_GuildPermissionLevels_Guild_GuildID",
                        column: x => x.GuildID,
                        principalSchema: "Aper_Bot",
                        principalTable: "Guild",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GuildRules",
                schema: "Aper_Bot",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Number = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "longtext", nullable: false),
                    GuildID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildRules", x => x.ID);
                    table.ForeignKey(
                        name: "FK_GuildRules_Guild_GuildID",
                        column: x => x.GuildID,
                        principalSchema: "Aper_Bot",
                        principalTable: "Guild",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Quotes",
                schema: "Aper_Bot",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    number = table.Column<int>(type: "int", nullable: false),
                    CreatorID = table.Column<int>(type: "int", nullable: false),
                    SourceID = table.Column<int>(type: "int", nullable: true),
                    GuildID = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    EventTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Text = table.Column<string>(type: "longtext", nullable: false),
                    ImageID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quotes", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Quotes_Guild_GuildID",
                        column: x => x.GuildID,
                        principalSchema: "Aper_Bot",
                        principalTable: "Guild",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Quotes_Images_ImageID",
                        column: x => x.ImageID,
                        principalSchema: "Aper_Bot",
                        principalTable: "Images",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Quotes_Users_CreatorID",
                        column: x => x.CreatorID,
                        principalSchema: "Aper_Bot",
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Quotes_Users_SourceID",
                        column: x => x.SourceID,
                        principalSchema: "Aper_Bot",
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GuildPermissionLevels_GuildID",
                schema: "Aper_Bot",
                table: "GuildPermissionLevels",
                column: "GuildID");

            migrationBuilder.CreateIndex(
                name: "IX_GuildRules_GuildID",
                schema: "Aper_Bot",
                table: "GuildRules",
                column: "GuildID");

            migrationBuilder.CreateIndex(
                name: "IX_Quotes_CreatorID",
                schema: "Aper_Bot",
                table: "Quotes",
                column: "CreatorID");

            migrationBuilder.CreateIndex(
                name: "IX_Quotes_GuildID",
                schema: "Aper_Bot",
                table: "Quotes",
                column: "GuildID");

            migrationBuilder.CreateIndex(
                name: "IX_Quotes_ImageID",
                schema: "Aper_Bot",
                table: "Quotes",
                column: "ImageID");

            migrationBuilder.CreateIndex(
                name: "IX_Quotes_SourceID",
                schema: "Aper_Bot",
                table: "Quotes",
                column: "SourceID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GuildPermissionLevels",
                schema: "Aper_Bot");

            migrationBuilder.DropTable(
                name: "GuildRules",
                schema: "Aper_Bot");

            migrationBuilder.DropTable(
                name: "Quotes",
                schema: "Aper_Bot");

            migrationBuilder.DropTable(
                name: "Guild",
                schema: "Aper_Bot");

            migrationBuilder.DropTable(
                name: "Images",
                schema: "Aper_Bot");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "Aper_Bot");
        }
    }
}
