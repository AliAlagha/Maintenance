using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Maintenance.Data.Migrations
{
    public partial class RemovePhoneOrderFromBranchPhoneNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhoneOrder",
                table: "BranchPhoneNumbers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PhoneOrder",
                table: "BranchPhoneNumbers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
