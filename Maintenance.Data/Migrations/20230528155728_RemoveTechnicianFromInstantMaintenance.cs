using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Maintenance.Data.Migrations
{
    public partial class RemoveTechnicianFromInstantMaintenance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InstantMaintenances_AspNetUsers_TechnicianId",
                table: "InstantMaintenances");

            migrationBuilder.DropIndex(
                name: "IX_InstantMaintenances_TechnicianId",
                table: "InstantMaintenances");

            migrationBuilder.DropColumn(
                name: "TechnicianId",
                table: "InstantMaintenances");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TechnicianId",
                table: "InstantMaintenances",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_InstantMaintenances_TechnicianId",
                table: "InstantMaintenances",
                column: "TechnicianId");

            migrationBuilder.AddForeignKey(
                name: "FK_InstantMaintenances_AspNetUsers_TechnicianId",
                table: "InstantMaintenances",
                column: "TechnicianId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
