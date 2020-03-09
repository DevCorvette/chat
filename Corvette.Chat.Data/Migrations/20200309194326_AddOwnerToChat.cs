using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Corvette.Chat.Data.Migrations
{
    public partial class AddOwnerToChat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OwnerId",
                table: "Chats",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Chats_OwnerId",
                table: "Chats",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_Users_OwnerId",
                table: "Chats",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chats_Users_OwnerId",
                table: "Chats");

            migrationBuilder.DropIndex(
                name: "IX_Chats_OwnerId",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Chats");
        }
    }
}
