using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ServiceRequestSystem.Migrations
{
    /// <inheritdoc />
    public partial class SeedMoreClientsAndEngineers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "Email", "FullName", "PasswordHash", "Phone", "RoleId" },
                values: new object[,]
                {
                    { 6, "client2@example.com", "Сидоренко Марина Олегівна", "12345", "+380506667788", 1 },
                    { 7, "client3@example.com", "Гончаренко Максим Андрійович", "12345", "+380507778899", 1 },
                    { 8, "engineer2@example.com", "Шевченко Роман Володимирович", "12345", "+380508889900", 3 },
                    { 9, "engineer3@example.com", "Бондар Назар Сергійович", "12345", "+380509990011", 3 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 9);
        }
    }
}
