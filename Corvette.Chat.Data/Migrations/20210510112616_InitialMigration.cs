using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Corvette.Chat.Data.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    created = table.Column<DateTime>(nullable: false, defaultValueSql: "timezone('UTC'::text, now())"),
                    name = table.Column<string>(maxLength: 200, nullable: false),
                    login = table.Column<string>(maxLength: 200, nullable: false),
                    secret_key = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "chats",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    created = table.Column<DateTime>(nullable: false, defaultValueSql: "timezone('UTC'::text, now())"),
                    is_private = table.Column<bool>(nullable: false),
                    name = table.Column<string>(maxLength: 200, nullable: true),
                    owner_id = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chats", x => x.id);
                    table.ForeignKey(
                        name: "FK_chats_users_owner_id",
                        column: x => x.owner_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "members",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    created = table.Column<DateTime>(nullable: false, defaultValueSql: "timezone('UTC'::text, now())"),
                    user_id = table.Column<Guid>(nullable: false),
                    chat_id = table.Column<Guid>(nullable: false),
                    last_read_date = table.Column<DateTime>(nullable: false, defaultValueSql: "timezone('UTC'::text, now())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_members", x => x.id);
                    table.ForeignKey(
                        name: "FK_members_chats_chat_id",
                        column: x => x.chat_id,
                        principalTable: "chats",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_members_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "messages",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    created = table.Column<DateTime>(nullable: false, defaultValueSql: "timezone('UTC'::text, now())"),
                    text = table.Column<string>(maxLength: 3000, nullable: false),
                    author_id = table.Column<Guid>(nullable: false),
                    chat_id = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_messages", x => x.id);
                    table.ForeignKey(
                        name: "FK_messages_users_author_id",
                        column: x => x.author_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_messages_chats_chat_id",
                        column: x => x.chat_id,
                        principalTable: "chats",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_chats_owner_id",
                table: "chats",
                column: "owner_id");

            migrationBuilder.CreateIndex(
                name: "IX_members_user_id",
                table: "members",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_members_chat_id_user_id",
                table: "members",
                columns: new[] { "chat_id", "user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_messages_author_id",
                table: "messages",
                column: "author_id");

            migrationBuilder.CreateIndex(
                name: "IX_messages_chat_id",
                table: "messages",
                column: "chat_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_login",
                table: "users",
                column: "login",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_name",
                table: "users",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_login_secret_key",
                table: "users",
                columns: new[] { "login", "secret_key" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "members");

            migrationBuilder.DropTable(
                name: "messages");

            migrationBuilder.DropTable(
                name: "chats");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
