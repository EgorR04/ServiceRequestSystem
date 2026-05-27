using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiceRequestSystem.Migrations
{
    /// <inheritdoc />
    public partial class HashUserPasswords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEICIAjP9LYBhjQ1X7/U8am9gxBZJwwpMmBYkTgGx/by5uf2rWo2DzWDBiIPFLfjijA==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEICIAjP9LYBhjQ1X7/U8am9gxBZJwwpMmBYkTgGx/by5uf2rWo2DzWDBiIPFLfjijA==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEICIAjP9LYBhjQ1X7/U8am9gxBZJwwpMmBYkTgGx/by5uf2rWo2DzWDBiIPFLfjijA==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 4,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEICIAjP9LYBhjQ1X7/U8am9gxBZJwwpMmBYkTgGx/by5uf2rWo2DzWDBiIPFLfjijA==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 5,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEICIAjP9LYBhjQ1X7/U8am9gxBZJwwpMmBYkTgGx/by5uf2rWo2DzWDBiIPFLfjijA==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 6,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEICIAjP9LYBhjQ1X7/U8am9gxBZJwwpMmBYkTgGx/by5uf2rWo2DzWDBiIPFLfjijA==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 7,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEICIAjP9LYBhjQ1X7/U8am9gxBZJwwpMmBYkTgGx/by5uf2rWo2DzWDBiIPFLfjijA==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 8,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEICIAjP9LYBhjQ1X7/U8am9gxBZJwwpMmBYkTgGx/by5uf2rWo2DzWDBiIPFLfjijA==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 9,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEICIAjP9LYBhjQ1X7/U8am9gxBZJwwpMmBYkTgGx/by5uf2rWo2DzWDBiIPFLfjijA==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "PasswordHash",
                value: "12345");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                column: "PasswordHash",
                value: "12345");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                column: "PasswordHash",
                value: "12345");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 4,
                column: "PasswordHash",
                value: "12345");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 5,
                column: "PasswordHash",
                value: "12345");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 6,
                column: "PasswordHash",
                value: "12345");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 7,
                column: "PasswordHash",
                value: "12345");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 8,
                column: "PasswordHash",
                value: "12345");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 9,
                column: "PasswordHash",
                value: "12345");
        }
    }
}
