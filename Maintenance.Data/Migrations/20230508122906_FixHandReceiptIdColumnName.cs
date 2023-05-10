using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Maintenance.Data.Migrations
{
    public partial class FixHandReceiptIdColumnName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HandReciptId",
                table: "ReturnHandReceipts");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HandReciptId",
                table: "ReturnHandReceipts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
