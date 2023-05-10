using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Maintenance.Data.Migrations
{
    public partial class AddReturnHandReceiptTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalCollectedAmount",
                table: "HandReceipts");

            migrationBuilder.CreateTable(
                name: "ReturnHandReceipts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HandReciptId = table.Column<int>(type: "int", nullable: false),
                    HandReceiptId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReturnHandReceipts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReturnHandReceipts_HandReceipts_HandReceiptId",
                        column: x => x.HandReceiptId,
                        principalTable: "HandReceipts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReturnHandReceiptItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReturnHandReceiptId = table.Column<int>(type: "int", nullable: false),
                    Item = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Company = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemBarcode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WarrantyExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReturnReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Delivered = table.Column<bool>(type: "bit", nullable: false),
                    DeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReturnHandReceiptItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReturnHandReceiptItems_ReturnHandReceipts_ReturnHandReceiptId",
                        column: x => x.ReturnHandReceiptId,
                        principalTable: "ReturnHandReceipts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReturnHandReceiptItems_ReturnHandReceiptId",
                table: "ReturnHandReceiptItems",
                column: "ReturnHandReceiptId");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnHandReceipts_HandReceiptId",
                table: "ReturnHandReceipts",
                column: "HandReceiptId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReturnHandReceiptItems");

            migrationBuilder.DropTable(
                name: "ReturnHandReceipts");

            migrationBuilder.AddColumn<double>(
                name: "TotalCollectedAmount",
                table: "HandReceipts",
                type: "float",
                nullable: true);
        }
    }
}
