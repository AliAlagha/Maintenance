using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Maintenance.Data.Migrations
{
    public partial class AddHandReceiptTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "HandReceipts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalCollectedAmount = table.Column<double>(type: "float", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HandReceipts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HandReceipts_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HandReceiptItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HandReceiptId = table.Column<int>(type: "int", nullable: false),
                    Item = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Company = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SpecifiedCost = table.Column<double>(type: "float", nullable: true),
                    NotifyCustomerOfTheCost = table.Column<bool>(type: "bit", nullable: false),
                    CostNotifiedToTheCustomer = table.Column<double>(type: "float", nullable: true),
                    CostFrom = table.Column<double>(type: "float", nullable: true),
                    CostTo = table.Column<double>(type: "float", nullable: true),
                    Urgent = table.Column<bool>(type: "bit", nullable: false),
                    ItemBarcode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WarrantyExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CollectedAmount = table.Column<double>(type: "float", nullable: true),
                    CollectionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Delivered = table.Column<bool>(type: "bit", nullable: false),
                    DeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MaintenanceRequestStatus = table.Column<int>(type: "int", nullable: false),
                    ReasonForRefusingMaintenance = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HandReceiptItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HandReceiptItems_HandReceipts_HandReceiptId",
                        column: x => x.HandReceiptId,
                        principalTable: "HandReceipts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HandReceiptItems_HandReceiptId",
                table: "HandReceiptItems",
                column: "HandReceiptId");

            migrationBuilder.CreateIndex(
                name: "IX_HandReceipts_CustomerId",
                table: "HandReceipts",
                column: "CustomerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HandReceiptItems");

            migrationBuilder.DropTable(
                name: "HandReceipts");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
