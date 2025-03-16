using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MaintenanceApp.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUnwantedColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_phraseModel_requestMaintenanceModel_RequestMaintenanceModelId",
                table: "phraseModel");

            migrationBuilder.DropIndex(
                name: "IX_phraseModel_RequestMaintenanceModelId",
                table: "phraseModel");

            migrationBuilder.DropColumn(
                name: "RequestID",
                table: "requestMaintenanceModel");

            migrationBuilder.DropColumn(
                name: "RequestMaintenanceModelId",
                table: "phraseModel");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RequestID",
                table: "requestMaintenanceModel",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RequestMaintenanceModelId",
                table: "phraseModel",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_phraseModel_RequestMaintenanceModelId",
                table: "phraseModel",
                column: "RequestMaintenanceModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_phraseModel_requestMaintenanceModel_RequestMaintenanceModelId",
                table: "phraseModel",
                column: "RequestMaintenanceModelId",
                principalTable: "requestMaintenanceModel",
                principalColumn: "Id");
        }
    }
}
