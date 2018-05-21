using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace HartPR.Migrations
{
    public partial class madeplayertrueskilltablenullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MeleeTrueskill",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "PMTrueskill",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "Smash4Trueskill",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "Smash5Trueskill",
                table: "Players");

            migrationBuilder.AlterColumn<double>(
                name: "Trueskill",
                table: "Players",
                nullable: true,
                oldClrType: typeof(double));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Trueskill",
                table: "Players",
                nullable: false,
                oldClrType: typeof(double),
                oldNullable: true);

            migrationBuilder.AddColumn<double>(
                name: "MeleeTrueskill",
                table: "Players",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "PMTrueskill",
                table: "Players",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Smash4Trueskill",
                table: "Players",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Smash5Trueskill",
                table: "Players",
                nullable: true);
        }
    }
}
