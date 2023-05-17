using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Maintenance.Data.Migrations
{
    public partial class AddPreviousTechnicianToReceiptItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReceiptItems_AspNetUsers_TechnicianId",
                table: "ReceiptItems");

            migrationBuilder.AddColumn<int>(
                name: "PreviousReceiptItemId",
                table: "ReceiptItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PreviousTechnicianId",
                table: "ReceiptItems",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptItems_PreviousReceiptItemId",
                table: "ReceiptItems",
                column: "PreviousReceiptItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptItems_PreviousTechnicianId",
                table: "ReceiptItems",
                column: "PreviousTechnicianId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReceiptItems_AspNetUsers_PreviousTechnicianId",
                table: "ReceiptItems",
                column: "PreviousTechnicianId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReceiptItems_AspNetUsers_TechnicianId",
                table: "ReceiptItems",
                column: "TechnicianId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReceiptItems_ReceiptItems_PreviousReceiptItemId",
                table: "ReceiptItems",
                column: "PreviousReceiptItemId",
                principalTable: "ReceiptItems",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReceiptItems_AspNetUsers_PreviousTechnicianId",
                table: "ReceiptItems");

            migrationBuilder.DropForeignKey(
                name: "FK_ReceiptItems_AspNetUsers_TechnicianId",
                table: "ReceiptItems");

            migrationBuilder.DropForeignKey(
                name: "FK_ReceiptItems_ReceiptItems_PreviousReceiptItemId",
                table: "ReceiptItems");

            migrationBuilder.DropIndex(
                name: "IX_ReceiptItems_PreviousReceiptItemId",
                table: "ReceiptItems");

            migrationBuilder.DropIndex(
                name: "IX_ReceiptItems_PreviousTechnicianId",
                table: "ReceiptItems");

            migrationBuilder.DropColumn(
                name: "PreviousReceiptItemId",
                table: "ReceiptItems");

            migrationBuilder.DropColumn(
                name: "PreviousTechnicianId",
                table: "ReceiptItems");

            migrationBuilder.AddForeignKey(
                name: "FK_ReceiptItems_AspNetUsers_TechnicianId",
                table: "ReceiptItems",
                column: "TechnicianId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
