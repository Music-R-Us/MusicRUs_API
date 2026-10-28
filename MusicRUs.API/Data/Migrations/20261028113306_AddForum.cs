using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MusicRUs.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddForum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "forum_posts",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    content = table.Column<string>(type: "text", nullable: false),
                    track_id = table.Column<int>(type: "integer", nullable: true),
                    album_id = table.Column<int>(type: "integer", nullable: true),
                    artist_id = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_forum_posts", x => x.id);
                    table.CheckConstraint("ck_forum_posts_not_empty", "char_length(btrim(title)) > 0 AND char_length(btrim(content)) > 0");
                    table.CheckConstraint("ck_forum_posts_one_music_link", "num_nonnulls(track_id, album_id, artist_id) <= 1");
                    table.ForeignKey(
                        name: "fk_forum_posts_albums_album_id",
                        column: x => x.album_id,
                        principalTable: "albums",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_forum_posts_artists_artist_id",
                        column: x => x.artist_id,
                        principalTable: "artists",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_forum_posts_tracks_track_id",
                        column: x => x.track_id,
                        principalTable: "tracks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_forum_posts_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "comments",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    post_id = table.Column<int>(type: "integer", nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    content = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_comments", x => x.id);
                    table.CheckConstraint("ck_comments_not_empty", "char_length(btrim(content)) > 0");
                    table.ForeignKey(
                        name: "fk_comments_forum_posts_post_id",
                        column: x => x.post_id,
                        principalTable: "forum_posts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_comments_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "post_genres",
                columns: table => new
                {
                    post_id = table.Column<int>(type: "integer", nullable: false),
                    genre_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_post_genres", x => new { x.post_id, x.genre_id });
                    table.ForeignKey(
                        name: "fk_post_genres_forum_posts_post_id",
                        column: x => x.post_id,
                        principalTable: "forum_posts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_post_genres_genres_genre_id",
                        column: x => x.genre_id,
                        principalTable: "genres",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_comments_post_id_created_at",
                table: "comments",
                columns: new[] { "post_id", "created_at" });

            migrationBuilder.CreateIndex(
                name: "ix_comments_user_id",
                table: "comments",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_forum_posts_album_id",
                table: "forum_posts",
                column: "album_id");

            migrationBuilder.CreateIndex(
                name: "ix_forum_posts_artist_id",
                table: "forum_posts",
                column: "artist_id");

            migrationBuilder.CreateIndex(
                name: "ix_forum_posts_created_at",
                table: "forum_posts",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ix_forum_posts_track_id",
                table: "forum_posts",
                column: "track_id");

            migrationBuilder.CreateIndex(
                name: "ix_forum_posts_user_id",
                table: "forum_posts",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_post_genres_genre_id",
                table: "post_genres",
                column: "genre_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "comments");

            migrationBuilder.DropTable(
                name: "post_genres");

            migrationBuilder.DropTable(
                name: "forum_posts");
        }
    }
}
