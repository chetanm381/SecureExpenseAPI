using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecureExpenseAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddExpenseEntityUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Expenses",
                newName: "CreatedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Expenses",
                newName: "Date");
        }
    }
}
