using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.DataManagement.Users.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "VARCHAR(255)", nullable: false),
                    max_players_in_list = table.Column<int>(type: "integer", nullable: false, defaultValue: 100),
                    max_lists_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 30)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                    table.CheckConstraint("ch_max_lists_val_range", "max_lists_count >= 5 AND max_lists_count <= 100");
                    table.CheckConstraint("ch_max_players_val_range", "max_players_in_list >= 5 AND max_players_in_list <= 500");
                });

            migrationBuilder.CreateTable(
                name: "organises",
                columns: table => new
                {
                    club_id = table.Column<string>(type: "text", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_organises", x => x.club_id);
                    table.ForeignKey(
                        name: "fk_organises_by_user",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "players_lists",
                columns: table => new
                {
                    list_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "VARCHAR(255)", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_players_lists", x => x.list_id);
                    table.CheckConstraint("ch_name_len_range", "LENGTH(name) >= 1");
                    table.ForeignKey(
                        name: "fk_player_lists_by_user",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "contains",
                columns: table => new
                {
                    player_id = table.Column<string>(type: "text", nullable: false),
                    list_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_contains", x => new { x.list_id, x.player_id });
                    table.ForeignKey(
                        name: "fk_contains_by_lists",
                        column: x => x.list_id,
                        principalTable: "players_lists",
                        principalColumn: "list_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_organises_user_id",
                table: "organises",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_players_lists_user_id",
                table: "players_lists",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "uq_names_per_user",
                table: "players_lists",
                columns: new[] { "name", "user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uq_users_names",
                table: "users",
                column: "name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "contains");

            migrationBuilder.DropTable(
                name: "organises");

            migrationBuilder.DropTable(
                name: "players_lists");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
