using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Maintenance.Data.Migrations
{
    public partial class RemoveTechnicianIdFromRecipientMaintenance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecipientMaintenances_AspNetUsers_TechnicianId",
                table: "RecipientMaintenances");

            migrationBuilder.DropIndex(
                name: "IX_RecipientMaintenances_TechnicianId",
                table: "RecipientMaintenances");

            migrationBuilder.DropColumn(
                name: "TechnicianId",
                table: "RecipientMaintenances");

            migrationBuilder.AddColumn<int>(
                name: "CollectedAmountFor",
                table: "RecipientMaintenances",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CollectedAmountFor",
                table: "RecipientMaintenances");

            migrationBuilder.AddColumn<string>(
                name: "TechnicianId",
                table: "RecipientMaintenances",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_RecipientMaintenances_TechnicianId",
                table: "RecipientMaintenances",
                column: "TechnicianId");

            migrationBuilder.AddForeignKey(
                name: "FK_RecipientMaintenances_AspNetUsers_TechnicianId",
                table: "RecipientMaintenances",
                column: "TechnicianId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
