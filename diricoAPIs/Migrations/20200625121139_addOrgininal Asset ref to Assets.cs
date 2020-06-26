using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace diricoAPIs.Migrations
{
    public partial class addOrgininalAssetreftoAssets : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("6b60c4b8-dac2-48e3-b329-628a9c4a01be"));

            migrationBuilder.AddColumn<Guid>(
                name: "OriginalAssetRef",
                table: "Assets",
                nullable: true);

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "FirstName", "LastName", "UserName" },
                values: new object[] { new Guid("8334747e-5fb8-4011-ab5a-61a11bf6b892"), "FirstName", "LastName", "Admin" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("8334747e-5fb8-4011-ab5a-61a11bf6b892"));

            migrationBuilder.DropColumn(
                name: "OriginalAssetRef",
                table: "Assets");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "FirstName", "LastName", "UserName" },
                values: new object[] { new Guid("6b60c4b8-dac2-48e3-b329-628a9c4a01be"), "FirstName", "LastName", "Admin" });
        }
    }
}
