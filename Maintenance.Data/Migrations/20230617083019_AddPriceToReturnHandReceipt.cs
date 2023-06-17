using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Maintenance.Data.Migrations
{
    public partial class AddPriceToReturnHandReceipt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "CollectedAmount",
                table: "ReturnHandReceiptItems",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CollectionDate",
                table: "ReturnHandReceiptItems",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "CostFrom",
                table: "ReturnHandReceiptItems",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "CostNotifiedToTheCustomer",
                table: "ReturnHandReceiptItems",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "CostTo",
                table: "ReturnHandReceiptItems",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "FinalCost",
                table: "ReturnHandReceiptItems",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "NotifyCustomerOfTheCost",
                table: "ReturnHandReceiptItems",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ReasonForRefusingMaintenance",
                table: "ReturnHandReceiptItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "SpecifiedCost",
                table: "ReturnHandReceiptItems",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StatusBeforeSuspense",
                table: "ReturnHandReceiptItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Urgent",
                table: "ReturnHandReceiptItems",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CollectedAmount",
                table: "ReturnHandReceiptItems");

            migrationBuilder.DropColumn(
                name: "CollectionDate",
                table: "ReturnHandReceiptItems");

            migrationBuilder.DropColumn(
                name: "CostFrom",
                table: "ReturnHandReceiptItems");

            migrationBuilder.DropColumn(
                name: "CostNotifiedToTheCustomer",
                table: "ReturnHandReceiptItems");

            migrationBuilder.DropColumn(
                name: "CostTo",
                table: "ReturnHandReceiptItems");

            migrationBuilder.DropColumn(
                name: "FinalCost",
                table: "ReturnHandReceiptItems");

            migrationBuilder.DropColumn(
                name: "NotifyCustomerOfTheCost",
                table: "ReturnHandReceiptItems");

            migrationBuilder.DropColumn(
                name: "ReasonForRefusingMaintenance",
                table: "ReturnHandReceiptItems");

            migrationBuilder.DropColumn(
                name: "SpecifiedCost",
                table: "ReturnHandReceiptItems");

            migrationBuilder.DropColumn(
                name: "StatusBeforeSuspense",
                table: "ReturnHandReceiptItems");

            migrationBuilder.DropColumn(
                name: "Urgent",
                table: "ReturnHandReceiptItems");
        }
    }
}
