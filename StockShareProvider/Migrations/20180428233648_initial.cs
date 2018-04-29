using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace StockShareProvider.Migrations
{
    // ReSharper disable once InconsistentNaming
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SellOrders",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SellOrders", x => x.ID);
                });

          
            migrationBuilder.Sql(
                "USE [Provider]\r\nGO\r\n\r\nCREATE USER [NT AUTHORITY\\NETWORK SERVICE] FOR LOGIN [NT AUTHORITY\\NETWORK SERVICE] WITH DEFAULT_SCHEMA=[db_owner]\r\nGO");
            migrationBuilder.Sql("ALTER ROLE db_owner\r\nADD MEMBER [NT AUTHORITY\\NETWORK SERVICE]");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SellOrders");
        }
    }
}
