using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Renamer.Data.Migrations
{
    public partial class CreateEpisodeDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Series",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SeriesId = table.Column<int>(nullable: false),
                    SeriesName = table.Column<string>(nullable: true),
                    SeriesNamePreferred = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Series", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserFavorites",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SeriesId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFavorites", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Episodes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TVDBEpisodeId = table.Column<int>(nullable: false),
                    Season = table.Column<int>(nullable: false),
                    AiredSeasonId = table.Column<int>(nullable: false),
                    EpisodeName = table.Column<string>(nullable: true),
                    FirstAired = table.Column<DateTime>(nullable: false),
                    SeriesId = table.Column<int>(nullable: false),
                    TVSeriesId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Episodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Episodes_Series_TVSeriesId",
                        column: x => x.TVSeriesId,
                        principalTable: "Series",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Episodes_TVSeriesId",
                table: "Episodes",
                column: "TVSeriesId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Episodes");

            migrationBuilder.DropTable(
                name: "UserFavorites");

            migrationBuilder.DropTable(
                name: "Series");
        }
    }
}
