using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace diricoAPIs.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<Guid>(nullable: false),
                    UserName = table.Column<string>(maxLength: 30, nullable: false),
                    FirstName = table.Column<string>(maxLength: 100, nullable: false),
                    LastName = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Assets",
                columns: table => new
                {
                    AssetId = table.Column<Guid>(nullable: false),
                    AssetType = table.Column<int>(nullable: false),
                    Parent = table.Column<Guid>(nullable: false),
                    AssetFileName = table.Column<string>(nullable: false),
                    AssetFilePath = table.Column<string>(nullable: false),
                    Datetime = table.Column<DateTime>(nullable: false),
                    MetaData = table.Column<string>(nullable: true),
                    UserRef = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assets", x => x.AssetId);
                    table.ForeignKey(
                        name: "FK_Assets_Users_UserRef",
                        column: x => x.UserRef,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "FirstName", "LastName", "UserName" },
                values: new object[] { new Guid("cc3cfd90-a036-425e-90dc-745a48e38aba"), "FirstName", "LastName", "Admin" });

            migrationBuilder.CreateIndex(
                name: "IX_Assets_UserRef",
                table: "Assets",
                column: "UserRef");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Assets");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
