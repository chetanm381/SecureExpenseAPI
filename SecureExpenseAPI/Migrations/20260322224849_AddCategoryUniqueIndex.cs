using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecureExpenseAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Categories_Name_UserId",
                table: "Categories",
                columns: new[] { "Name", "UserId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Categories_Name_UserId",
                table: "Categories");
        }
    }
}
