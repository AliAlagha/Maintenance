using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Maintenance.Data.Migrations
{
    public partial class AddBranchToReceiptItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Branches_BranchId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_ReturnHandReceipts_HandReceiptId",
                table: "ReturnHandReceipts");

            migrationBuilder.AddColumn<int>(
                name: "BranchId",
                table: "ReturnHandReceipts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BranchId",
                table: "ReceiptItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BranchId",
                table: "HandReceipts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "BranchId",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReturnHandReceipts_BranchId",
                table: "ReturnHandReceipts",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnHandReceipts_HandReceiptId",
                table: "ReturnHandReceipts",
                column: "HandReceiptId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptItems_BranchId",
                table: "ReceiptItems",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_HandReceipts_BranchId",
                table: "HandReceipts",
                column: "BranchId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Branches_BranchId",
                table: "AspNetUsers",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HandReceipts_Branches_BranchId",
                table: "HandReceipts",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReceiptItems_Branches_BranchId",
                table: "ReceiptItems",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReturnHandReceipts_Branches_BranchId",
                table: "ReturnHandReceipts",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Branches_BranchId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_HandReceipts_Branches_BranchId",
                table: "HandReceipts");

            migrationBuilder.DropForeignKey(
                name: "FK_ReceiptItems_Branches_BranchId",
                table: "ReceiptItems");

            migrationBuilder.DropForeignKey(
                name: "FK_ReturnHandReceipts_Branches_BranchId",
                table: "ReturnHandReceipts");

            migrationBuilder.DropIndex(
                name: "IX_ReturnHandReceipts_BranchId",
                table: "ReturnHandReceipts");

            migrationBuilder.DropIndex(
                name: "IX_ReturnHandReceipts_HandReceiptId",
                table: "ReturnHandReceipts");

            migrationBuilder.DropIndex(
                name: "IX_ReceiptItems_BranchId",
                table: "ReceiptItems");

            migrationBuilder.DropIndex(
                name: "IX_HandReceipts_BranchId",
                table: "HandReceipts");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "ReturnHandReceipts");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "ReceiptItems");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "HandReceipts");

            migrationBuilder.AlterColumn<int>(
                name: "BranchId",
                table: "AspNetUsers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnHandReceipts_HandReceiptId",
                table: "ReturnHandReceipts",
                column: "HandReceiptId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Branches_BranchId",
                table: "AspNetUsers",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id");
        }
    }
}
