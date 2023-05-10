using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Maintenance.Data.Migrations
{
    public partial class AddReceiptItemsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HandReceipts_Customers_CustomerId",
                table: "HandReceipts");

            migrationBuilder.DropTable(
                name: "HandReceiptItems");

            migrationBuilder.DropTable(
                name: "ReturnHandReceiptItems");

            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "ReturnHandReceipts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ReceiptItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HandReceiptId = table.Column<int>(type: "int", nullable: true),
                    ReturnHandReceiptId = table.Column<int>(type: "int", nullable: true),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
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
                    DeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MaintenanceRequestStatus = table.Column<int>(type: "int", nullable: false),
                    ReasonForRefusingMaintenance = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaintenanceSuspensionReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReturnReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceiptItemType = table.Column<int>(type: "int", nullable: false),
                    TechnicianId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceiptItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReceiptItems_AspNetUsers_TechnicianId",
                        column: x => x.TechnicianId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReceiptItems_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReceiptItems_HandReceipts_HandReceiptId",
                        column: x => x.HandReceiptId,
                        principalTable: "HandReceipts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ReceiptItems_ReturnHandReceipts_ReturnHandReceiptId",
                        column: x => x.ReturnHandReceiptId,
                        principalTable: "ReturnHandReceipts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReturnHandReceipts_CustomerId",
                table: "ReturnHandReceipts",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptItems_CustomerId",
                table: "ReceiptItems",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptItems_HandReceiptId",
                table: "ReceiptItems",
                column: "HandReceiptId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptItems_ReturnHandReceiptId",
                table: "ReceiptItems",
                column: "ReturnHandReceiptId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptItems_TechnicianId",
                table: "ReceiptItems",
                column: "TechnicianId");

            migrationBuilder.AddForeignKey(
                name: "FK_HandReceipts_Customers_CustomerId",
                table: "HandReceipts",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReturnHandReceipts_Customers_CustomerId",
                table: "ReturnHandReceipts",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HandReceipts_Customers_CustomerId",
                table: "HandReceipts");

            migrationBuilder.DropForeignKey(
                name: "FK_ReturnHandReceipts_Customers_CustomerId",
                table: "ReturnHandReceipts");

            migrationBuilder.DropTable(
                name: "ReceiptItems");

            migrationBuilder.DropIndex(
                name: "IX_ReturnHandReceipts_CustomerId",
                table: "ReturnHandReceipts");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "ReturnHandReceipts");

            migrationBuilder.CreateTable(
                name: "HandReceiptItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HandReceiptId = table.Column<int>(type: "int", nullable: false),
                    CollectedAmount = table.Column<double>(type: "float", nullable: true),
                    CollectionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Company = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CostFrom = table.Column<double>(type: "float", nullable: true),
                    CostNotifiedToTheCustomer = table.Column<double>(type: "float", nullable: true),
                    CostTo = table.Column<double>(type: "float", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Delivered = table.Column<bool>(type: "bit", nullable: false),
                    DeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    Item = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ItemBarcode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaintenanceRequestStatus = table.Column<int>(type: "int", nullable: false),
                    NotifyCustomerOfTheCost = table.Column<bool>(type: "bit", nullable: false),
                    ReasonForRefusingMaintenance = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SpecifiedCost = table.Column<double>(type: "float", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Urgent = table.Column<bool>(type: "bit", nullable: false),
                    WarrantyExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true)
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

            migrationBuilder.CreateTable(
                name: "ReturnHandReceiptItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReturnHandReceiptId = table.Column<int>(type: "int", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Company = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Delivered = table.Column<bool>(type: "bit", nullable: false),
                    DeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    Item = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ItemBarcode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReturnReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WarrantyExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true)
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
                name: "IX_HandReceiptItems_HandReceiptId",
                table: "HandReceiptItems",
                column: "HandReceiptId");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnHandReceiptItems_ReturnHandReceiptId",
                table: "ReturnHandReceiptItems",
                column: "ReturnHandReceiptId");

            migrationBuilder.AddForeignKey(
                name: "FK_HandReceipts_Customers_CustomerId",
                table: "HandReceipts",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
