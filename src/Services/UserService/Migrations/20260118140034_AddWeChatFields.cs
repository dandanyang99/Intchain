using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Intchain.UserService.Migrations
{
    /// <inheritdoc />
    public partial class AddWeChatFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "openid_bound_at",
                table: "users",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "session_key",
                table: "users",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "unionid",
                table: "users",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "openid_bound_at",
                table: "users");

            migrationBuilder.DropColumn(
                name: "session_key",
                table: "users");

            migrationBuilder.DropColumn(
                name: "unionid",
                table: "users");
        }
    }
}
