using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MusicRUs.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSocial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "friendships",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    requester_id = table.Column<int>(type: "integer", nullable: false),
                    addressee_id = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_friendships", x => x.id);
                    table.CheckConstraint("ck_friendships_not_self", "requester_id <> addressee_id");
                    table.ForeignKey(
                        name: "fk_friendships_users_addressee_id",
                        column: x => x.addressee_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_friendships_users_requester_id",
                        column: x => x.requester_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "messages",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    sender_id = table.Column<int>(type: "integer", nullable: false),
                    recipient_id = table.Column<int>(type: "integer", nullable: false),
                    content = table.Column<string>(type: "text", nullable: false),
                    read_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_messages", x => x.id);
                    table.CheckConstraint("ck_messages_not_empty", "char_length(btrim(content)) > 0");
                    table.CheckConstraint("ck_messages_not_self", "sender_id <> recipient_id");
                    table.ForeignKey(
                        name: "fk_messages_users_recipient_id",
                        column: x => x.recipient_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_messages_users_sender_id",
                        column: x => x.sender_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_blocks",
                columns: table => new
                {
                    blocker_id = table.Column<int>(type: "integer", nullable: false),
                    blocked_id = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_blocks", x => new { x.blocker_id, x.blocked_id });
                    table.CheckConstraint("ck_user_blocks_not_self", "blocker_id <> blocked_id");
                    table.ForeignKey(
                        name: "fk_user_blocks_users_blocked_id",
                        column: x => x.blocked_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_blocks_users_blocker_id",
                        column: x => x.blocker_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_friendships_addressee_id",
                table: "friendships",
                column: "addressee_id");

            migrationBuilder.CreateIndex(
                name: "ix_friendships_requester_id",
                table: "friendships",
                column: "requester_id");

            migrationBuilder.CreateIndex(
                name: "ix_messages_recipient_id_created_at",
                table: "messages",
                columns: new[] { "recipient_id", "created_at" });

            migrationBuilder.CreateIndex(
                name: "ix_messages_sender_id_recipient_id_created_at",
                table: "messages",
                columns: new[] { "sender_id", "recipient_id", "created_at" });

            migrationBuilder.CreateIndex(
                name: "ix_user_blocks_blocked_id",
                table: "user_blocks",
                column: "blocked_id");

            migrationBuilder.Sql(
            "CREATE UNIQUE INDEX ux_friendships_pair ON friendships " +
            "(LEAST(requester_id, addressee_id), GREATEST(requester_id, addressee_id));");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.Sql("DROP INDEX IF EXISTS ux_friendships_pair;");
            
            migrationBuilder.DropTable(
                name: "friendships");

            migrationBuilder.DropTable(
                name: "messages");

            migrationBuilder.DropTable(
                name: "user_blocks");
        }
    }
}
