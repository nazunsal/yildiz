using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace yildiz.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class RemoveOldAdminUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "Email", "IsAdmin", "Name", "Password", "Username" },
                values: new object[] { 1, "admin@yildiz.com", true, "Yıldız Admin", "1234", "admin" });
        }
    }
}
