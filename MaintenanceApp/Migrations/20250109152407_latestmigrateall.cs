using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MaintenanceApp.Migrations
{
    /// <inheritdoc />
    public partial class latestmigrateall : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_phraseModel_RequestMaintenanceID",
                table: "phraseModel");

            migrationBuilder.CreateIndex(
                name: "IX_phraseModel_RequestMaintenanceID",
                table: "phraseModel",
                column: "RequestMaintenanceID",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_phraseModel_RequestMaintenanceID",
                table: "phraseModel");

            migrationBuilder.CreateIndex(
                name: "IX_phraseModel_RequestMaintenanceID",
                table: "phraseModel",
                column: "RequestMaintenanceID");
        }
    }
}
