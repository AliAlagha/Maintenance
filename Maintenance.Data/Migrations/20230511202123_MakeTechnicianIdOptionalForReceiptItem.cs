using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Maintenance.Data.Migrations
{
    public partial class MakeTechnicianIdOptionalForReceiptItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReceiptItems_AspNetUsers_TechnicianId",
                table: "ReceiptItems");

            migrationBuilder.AlterColumn<string>(
                name: "TechnicianId",
                table: "ReceiptItems",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_ReceiptItems_AspNetUsers_TechnicianId",
                table: "ReceiptItems",
                column: "TechnicianId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReceiptItems_AspNetUsers_TechnicianId",
                table: "ReceiptItems");

            migrationBuilder.AlterColumn<string>(
                name: "TechnicianId",
                table: "ReceiptItems",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ReceiptItems_AspNetUsers_TechnicianId",
                table: "ReceiptItems",
                column: "TechnicianId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
