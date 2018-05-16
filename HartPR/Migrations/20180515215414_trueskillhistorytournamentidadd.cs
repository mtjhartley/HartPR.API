using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace HartPR.Migrations
{
    public partial class trueskillhistorytournamentidadd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TournamentName",
                table: "TrueskillHistories");

            migrationBuilder.AddColumn<Guid>(
                name: "TournamentId",
                table: "TrueskillHistories",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TournamentId",
                table: "TrueskillHistories");

            migrationBuilder.AddColumn<string>(
                name: "TournamentName",
                table: "TrueskillHistories",
                nullable: true);
        }
    }
}
