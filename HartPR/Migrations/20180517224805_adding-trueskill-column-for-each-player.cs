using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace HartPR.Migrations
{
    public partial class addingtrueskillcolumnforeachplayer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "MeleeTrueskill",
                table: "Players",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "PMTrueskill",
                table: "Players",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Smash4Trueskill",
                table: "Players",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Smash5Trueskill",
                table: "Players",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
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
        }
    }
}
