using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace StockShareProvider.DbAccess.Migrations
{
    public partial class sellorder2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreateTime",
                table: "SellOrders",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<decimal>(
                name: "SellPrice",
                table: "SellOrders",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "StockID",
                table: "SellOrders",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserID",
                table: "SellOrders",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreateTime",
                table: "SellOrders");

            migrationBuilder.DropColumn(
                name: "SellPrice",
                table: "SellOrders");

            migrationBuilder.DropColumn(
                name: "StockID",
                table: "SellOrders");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "SellOrders");
        }
    }
}
