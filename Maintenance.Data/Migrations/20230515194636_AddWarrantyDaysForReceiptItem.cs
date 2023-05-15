using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Maintenance.Data.Migrations
{
    public partial class AddWarrantyDaysForReceiptItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WarrantyExpiryDate",
                table: "ReceiptItems");

            migrationBuilder.AddColumn<int>(
                name: "WarrantyDaysNumber",
                table: "ReceiptItems",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WarrantyDaysNumber",
                table: "ReceiptItems");

            migrationBuilder.AddColumn<DateTime>(
                name: "WarrantyExpiryDate",
                table: "ReceiptItems",
                type: "datetime2",
                nullable: true);
        }
    }
}
