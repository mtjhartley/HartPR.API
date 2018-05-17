using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace HartPR.Migrations
{
    public partial class splittrueskillmusigma : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TournamentDate",
                table: "TrueskillHistories");

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

        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "TournamentDate",
                table: "TrueskillHistories",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
        }
    }
}
