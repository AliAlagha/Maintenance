using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Maintenance.Data.Migrations
{
    public partial class AddInstantMaintenanceTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReceiptItems");

            migrationBuilder.CreateTable(
                name: "HandReceiptItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HandReceiptId = table.Column<int>(type: "int", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    Item = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Company = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SpecifiedCost = table.Column<double>(type: "float", nullable: true),
                    NotifyCustomerOfTheCost = table.Column<bool>(type: "bit", nullable: false),
                    CostNotifiedToTheCustomer = table.Column<double>(type: "float", nullable: true),
                    CostFrom = table.Column<double>(type: "float", nullable: true),
                    CostTo = table.Column<double>(type: "float", nullable: true),
                    FinalCost = table.Column<double>(type: "float", nullable: true),
                    Urgent = table.Column<bool>(type: "bit", nullable: false),
                    ItemBarcode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ItemBarcodeFilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WarrantyDaysNumber = table.Column<int>(type: "int", nullable: true),
                    CollectedAmount = table.Column<double>(type: "float", nullable: true),
                    CollectionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MaintenanceRequestStatus = table.Column<int>(type: "int", nullable: false),
                    RemoveFromMaintainedReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReasonForRefusingMaintenance = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaintenanceSuspensionReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReturnReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TechnicianId = table.Column<string>(type: "nvarchar(450)", nullable: true),
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
                        name: "FK_HandReceiptItems_AspNetUsers_TechnicianId",
                        column: x => x.TechnicianId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HandReceiptItems_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HandReceiptItems_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HandReceiptItems_HandReceipts_HandReceiptId",
                        column: x => x.HandReceiptId,
                        principalTable: "HandReceipts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InstantMaintenances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    TechnicianId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstantMaintenances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InstantMaintenances_AspNetUsers_TechnicianId",
                        column: x => x.TechnicianId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InstantMaintenances_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RecipientMaintenances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    CollectedAmount = table.Column<double>(type: "float", nullable: true),
                    TechnicianId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipientMaintenances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecipientMaintenances_AspNetUsers_TechnicianId",
                        column: x => x.TechnicianId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecipientMaintenances_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReturnHandReceiptItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReturnHandReceiptId = table.Column<int>(type: "int", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    Item = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Company = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemBarcode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ItemBarcodeFilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsReturnItemWarrantyValid = table.Column<bool>(type: "bit", nullable: false),
                    DeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MaintenanceRequestStatus = table.Column<int>(type: "int", nullable: false),
                    RemoveFromMaintainedReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaintenanceSuspensionReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReturnReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TechnicianId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    HandReceiptItemId = table.Column<int>(type: "int", nullable: false),
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
                        name: "FK_ReturnHandReceiptItems_AspNetUsers_TechnicianId",
                        column: x => x.TechnicianId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReturnHandReceiptItems_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReturnHandReceiptItems_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReturnHandReceiptItems_HandReceiptItems_HandReceiptItemId",
                        column: x => x.HandReceiptItemId,
                        principalTable: "HandReceiptItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReturnHandReceiptItems_ReturnHandReceipts_ReturnHandReceiptId",
                        column: x => x.ReturnHandReceiptId,
                        principalTable: "ReturnHandReceipts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InstantMaintenanceItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InstantMaintenanceId = table.Column<int>(type: "int", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    Item = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Company = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CollectedAmount = table.Column<double>(type: "float", nullable: true),
                    TechnicianId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstantMaintenanceItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InstantMaintenanceItems_AspNetUsers_TechnicianId",
                        column: x => x.TechnicianId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InstantMaintenanceItems_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InstantMaintenanceItems_InstantMaintenances_InstantMaintenanceId",
                        column: x => x.InstantMaintenanceId,
                        principalTable: "InstantMaintenances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HandReceiptItems_BranchId",
                table: "HandReceiptItems",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_HandReceiptItems_CustomerId",
                table: "HandReceiptItems",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_HandReceiptItems_HandReceiptId",
                table: "HandReceiptItems",
                column: "HandReceiptId");

            migrationBuilder.CreateIndex(
                name: "IX_HandReceiptItems_TechnicianId",
                table: "HandReceiptItems",
                column: "TechnicianId");

            migrationBuilder.CreateIndex(
                name: "IX_InstantMaintenanceItems_BranchId",
                table: "InstantMaintenanceItems",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_InstantMaintenanceItems_InstantMaintenanceId",
                table: "InstantMaintenanceItems",
                column: "InstantMaintenanceId");

            migrationBuilder.CreateIndex(
                name: "IX_InstantMaintenanceItems_TechnicianId",
                table: "InstantMaintenanceItems",
                column: "TechnicianId");

            migrationBuilder.CreateIndex(
                name: "IX_InstantMaintenances_BranchId",
                table: "InstantMaintenances",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_InstantMaintenances_TechnicianId",
                table: "InstantMaintenances",
                column: "TechnicianId");

            migrationBuilder.CreateIndex(
                name: "IX_RecipientMaintenances_BranchId",
                table: "RecipientMaintenances",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_RecipientMaintenances_TechnicianId",
                table: "RecipientMaintenances",
                column: "TechnicianId");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnHandReceiptItems_BranchId",
                table: "ReturnHandReceiptItems",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnHandReceiptItems_CustomerId",
                table: "ReturnHandReceiptItems",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnHandReceiptItems_HandReceiptItemId",
                table: "ReturnHandReceiptItems",
                column: "HandReceiptItemId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReturnHandReceiptItems_ReturnHandReceiptId",
                table: "ReturnHandReceiptItems",
                column: "ReturnHandReceiptId");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnHandReceiptItems_TechnicianId",
                table: "ReturnHandReceiptItems",
                column: "TechnicianId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InstantMaintenanceItems");

            migrationBuilder.DropTable(
                name: "RecipientMaintenances");

            migrationBuilder.DropTable(
                name: "ReturnHandReceiptItems");

            migrationBuilder.DropTable(
                name: "InstantMaintenances");

            migrationBuilder.DropTable(
                name: "HandReceiptItems");

            migrationBuilder.CreateTable(
                name: "ReceiptItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    HandReceiptId = table.Column<int>(type: "int", nullable: true),
                    PreviousReceiptItemId = table.Column<int>(type: "int", nullable: true),
                    PreviousTechnicianId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ReturnHandReceiptId = table.Column<int>(type: "int", nullable: true),
                    TechnicianId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CollectedAmount = table.Column<double>(type: "float", nullable: true),
                    CollectionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Company = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CostFrom = table.Column<double>(type: "float", nullable: true),
                    CostNotifiedToTheCustomer = table.Column<double>(type: "float", nullable: true),
                    CostTo = table.Column<double>(type: "float", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FinalCost = table.Column<double>(type: "float", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    IsReturnItemWarrantyExpired = table.Column<bool>(type: "bit", nullable: false),
                    Item = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ItemBarcode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ItemBarcodeFilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaintenanceRequestStatus = table.Column<int>(type: "int", nullable: false),
                    MaintenanceSuspensionReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NotifyCustomerOfTheCost = table.Column<bool>(type: "bit", nullable: false),
                    ReasonForRefusingMaintenance = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceiptItemType = table.Column<int>(type: "int", nullable: false),
                    RemoveFromMaintainedReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReturnReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SpecifiedCost = table.Column<double>(type: "float", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Urgent = table.Column<bool>(type: "bit", nullable: false),
                    WarrantyDaysNumber = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceiptItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReceiptItems_AspNetUsers_PreviousTechnicianId",
                        column: x => x.PreviousTechnicianId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReceiptItems_AspNetUsers_TechnicianId",
                        column: x => x.TechnicianId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReceiptItems_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                        name: "FK_ReceiptItems_ReceiptItems_PreviousReceiptItemId",
                        column: x => x.PreviousReceiptItemId,
                        principalTable: "ReceiptItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ReceiptItems_ReturnHandReceipts_ReturnHandReceiptId",
                        column: x => x.ReturnHandReceiptId,
                        principalTable: "ReturnHandReceipts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptItems_BranchId",
                table: "ReceiptItems",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptItems_CustomerId",
                table: "ReceiptItems",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptItems_HandReceiptId",
                table: "ReceiptItems",
                column: "HandReceiptId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptItems_PreviousReceiptItemId",
                table: "ReceiptItems",
                column: "PreviousReceiptItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptItems_PreviousTechnicianId",
                table: "ReceiptItems",
                column: "PreviousTechnicianId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptItems_ReturnHandReceiptId",
                table: "ReceiptItems",
                column: "ReturnHandReceiptId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptItems_TechnicianId",
                table: "ReceiptItems",
                column: "TechnicianId");
        }
    }
}
