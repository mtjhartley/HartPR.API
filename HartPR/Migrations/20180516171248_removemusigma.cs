using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace HartPR.Migrations
{
    public partial class removemusigma : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TrueskillMu",
                table: "TrueskillHistories");

            migrationBuilder.DropColumn(
                name: "TrueskillMu",
                table: "Players");

            migrationBuilder.RenameColumn(
                name: "TrueskillSigma",
                table: "TrueskillHistories",
                newName: "Trueskill");

            migrationBuilder.RenameColumn(
                name: "TrueskillSigma",
                table: "Players",
                newName: "Trueskill");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Trueskill",
                table: "TrueskillHistories",
                newName: "TrueskillSigma");

            migrationBuilder.RenameColumn(
                name: "Trueskill",
                table: "Players",
                newName: "TrueskillSigma");

            migrationBuilder.AddColumn<double>(
                name: "TrueskillMu",
                table: "TrueskillHistories",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "TrueskillMu",
                table: "Players",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
