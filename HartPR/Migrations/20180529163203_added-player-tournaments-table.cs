using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace HartPR.Migrations
{
    public partial class addedplayertournamentstable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlayerTournaments",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Placing = table.Column<int>(nullable: false),
                    PlayerId = table.Column<Guid>(nullable: false),
                    Seed = table.Column<int>(nullable: false),
                    TournamentId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerTournaments", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerTournaments");
        }
    }
}
