using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Maintenance.Data.Migrations
{
    public partial class AddIsReturnItemWarrantyExpiredToReceiptItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsReturnItemWarrantyExpired",
                table: "ReceiptItems",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsReturnItemWarrantyExpired",
                table: "ReceiptItems");
        }
    }
}
