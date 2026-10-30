using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicRUs.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddProfileColors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "accent_color",
                table: "forum_profiles",
                type: "character varying(7)",
                maxLength: 7,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "primary_color",
                table: "forum_profiles",
                type: "character varying(7)",
                maxLength: 7,
                nullable: true);

            migrationBuilder.AddCheckConstraint(
                name: "ck_forum_profiles_accent_color",
                table: "forum_profiles",
                sql: "accent_color ~ '^#[0-9A-Fa-f]{6}$'");

            migrationBuilder.AddCheckConstraint(
                name: "ck_forum_profiles_primary_color",
                table: "forum_profiles",
                sql: "primary_color ~ '^#[0-9A-Fa-f]{6}$'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "ck_forum_profiles_accent_color",
                table: "forum_profiles");

            migrationBuilder.DropCheckConstraint(
                name: "ck_forum_profiles_primary_color",
                table: "forum_profiles");

            migrationBuilder.DropColumn(
                name: "accent_color",
                table: "forum_profiles");

            migrationBuilder.DropColumn(
                name: "primary_color",
                table: "forum_profiles");
        }
    }
}
