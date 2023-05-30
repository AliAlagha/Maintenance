using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Maintenance.Data.Migrations
{
    public partial class RemoveInstantMaintenanceTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InstantMaintenanceItems");

            migrationBuilder.DropTable(
                name: "InstantMaintenances");

            migrationBuilder.AddColumn<int>(
                name: "MaintenanceType",
                table: "HandReceipts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaintenanceType",
                table: "HandReceipts");

            migrationBuilder.CreateTable(
                name: "InstantMaintenances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstantMaintenances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InstantMaintenances_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InstantMaintenanceItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    InstantMaintenanceId = table.Column<int>(type: "int", nullable: false),
                    TechnicianId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CollectedAmount = table.Column<double>(type: "float", nullable: true),
                    Company = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    Item = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
        }
    }
}
