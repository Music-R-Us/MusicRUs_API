using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicRUs.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixProfileForeignKeyNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_favorite_albums_forum_profile_profile_id",
                table: "favorite_albums");

            migrationBuilder.DropForeignKey(
                name: "fk_favorite_artists_forum_profile_profile_id",
                table: "favorite_artists");

            migrationBuilder.AddForeignKey(
                name: "fk_favorite_albums_forum_profiles_profile_id",
                table: "favorite_albums",
                column: "profile_id",
                principalTable: "forum_profiles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_favorite_artists_forum_profiles_profile_id",
                table: "favorite_artists",
                column: "profile_id",
                principalTable: "forum_profiles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_favorite_albums_forum_profiles_profile_id",
                table: "favorite_albums");

            migrationBuilder.DropForeignKey(
                name: "fk_favorite_artists_forum_profiles_profile_id",
                table: "favorite_artists");

            migrationBuilder.AddForeignKey(
                name: "fk_favorite_albums_forum_profile_profile_id",
                table: "favorite_albums",
                column: "profile_id",
                principalTable: "forum_profiles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_favorite_artists_forum_profile_profile_id",
                table: "favorite_artists",
                column: "profile_id",
                principalTable: "forum_profiles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
