using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace diricoAPIs.Migrations
{
    public partial class RemoveUserRefandAllownullParentinAsset : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assets_Users_UserRef",
                table: "Assets");

            migrationBuilder.DropIndex(
                name: "IX_Assets_UserRef",
                table: "Assets");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("cc3cfd90-a036-425e-90dc-745a48e38aba"));

            migrationBuilder.DropColumn(
                name: "UserRef",
                table: "Assets");

            migrationBuilder.AlterColumn<Guid>(
                name: "Parent",
                table: "Assets",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "FirstName", "LastName", "UserName" },
                values: new object[] { new Guid("6b60c4b8-dac2-48e3-b329-628a9c4a01be"), "FirstName", "LastName", "Admin" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("6b60c4b8-dac2-48e3-b329-628a9c4a01be"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Parent",
                table: "Assets",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserRef",
                table: "Assets",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "FirstName", "LastName", "UserName" },
                values: new object[] { new Guid("cc3cfd90-a036-425e-90dc-745a48e38aba"), "FirstName", "LastName", "Admin" });

            migrationBuilder.CreateIndex(
                name: "IX_Assets_UserRef",
                table: "Assets",
                column: "UserRef");

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_Users_UserRef",
                table: "Assets",
                column: "UserRef",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
