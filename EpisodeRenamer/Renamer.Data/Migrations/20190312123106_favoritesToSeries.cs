using Microsoft.EntityFrameworkCore.Migrations;

namespace Renamer.Data.Migrations
{
    public partial class favoritesToSeries : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            // manual by PTG:
            migrationBuilder.Sql("ALTER TABLE UserFavorites RENAME TO UserFavoritesOld", true);
            migrationBuilder.CreateTable(
                name: "UserFavorites",
                columns: table => new {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SeriesId = table.Column<int>(nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_UserFavorites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Series_UserFavorites_UserFavoritesId",
                        column: x => x.SeriesId,
                        principalTable: "Series",
                        principalColumn: "SeriesId",
                        onDelete: ReferentialAction.Restrict);
                });
            migrationBuilder.Sql("DROP TABLE UserFavoritesOld", true);

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserFavorites_Series_SeriesId",
                table: "UserFavorites");
        }
    }
}
