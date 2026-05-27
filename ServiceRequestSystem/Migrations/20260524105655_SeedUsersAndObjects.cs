using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ServiceRequestSystem.Migrations
{
    /// <inheritdoc />
    public partial class SeedUsersAndObjects : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ServiceObjects",
                columns: new[] { "ServiceObjectId", "Address", "ContactPerson", "ContactPhone", "Description", "ObjectName" },
                values: new object[,]
                {
                    { 1, "м. Київ, вул. Центральна, 10", "Іваненко Олександр", "+380501112233", "Офісне приміщення з IP-камерами відеоспостереження", "Офіс №1" },
                    { 2, "м. Київ, вул. Робоча, 25", "Сидоренко Марина", "+380506667788", "Офісне приміщення з відеореєстратором та мережевими камерами", "Офіс №2" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "Email", "FullName", "PasswordHash", "Phone", "RoleId" },
                values: new object[,]
                {
                    { 1, "client@example.com", "Іваненко Олександр Петрович", "12345", "+380501112233", 1 },
                    { 2, "operator@example.com", "Петренко Андрій Сергійович", "12345", "+380502223344", 2 },
                    { 3, "engineer@example.com", "Коваленко Дмитро Іванович", "12345", "+380503334455", 3 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ServiceObjects",
                keyColumn: "ServiceObjectId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ServiceObjects",
                keyColumn: "ServiceObjectId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3);
        }
    }
}
