using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MaintenanceApp.Migrations
{
    /// <inheritdoc />
    public partial class AddFormStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FormStatus",
                table: "requestMaintenanceModel",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FormStatus",
                table: "requestMaintenanceModel");
        }
    }
}
