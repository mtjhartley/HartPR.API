using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace HartPR.Migrations
{
    public partial class addedcharactertwittertouserremovedcharacterfromplayer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Character",
                table: "Players");

            migrationBuilder.AddColumn<byte>(
                name: "Character",
                table: "Users",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<string>(
                name: "Twitter",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Players",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Character",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Twitter",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Players");

            migrationBuilder.AddColumn<byte>(
                name: "Character",
                table: "Players",
                nullable: false,
                defaultValue: (byte)0);
        }
    }
}
