using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MaintenanceApp.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUniqueConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_phraseModel_requestMaintenanceModel_RequestMaintenanceID",
                table: "phraseModel");

            migrationBuilder.DropIndex(
                name: "IX_phraseModel_RequestMaintenanceID",
                table: "phraseModel");

            migrationBuilder.AddColumn<int>(
                name: "RequestMaintenanceModelId",
                table: "phraseModel",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_phraseModel_RequestMaintenanceID",
                table: "phraseModel",
                column: "RequestMaintenanceID");

            migrationBuilder.CreateIndex(
                name: "IX_phraseModel_RequestMaintenanceModelId",
                table: "phraseModel",
                column: "RequestMaintenanceModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_phraseModel_requestMaintenanceModel_RequestMaintenanceID",
                table: "phraseModel",
                column: "RequestMaintenanceID",
                principalTable: "requestMaintenanceModel",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_phraseModel_requestMaintenanceModel_RequestMaintenanceModelId",
                table: "phraseModel",
                column: "RequestMaintenanceModelId",
                principalTable: "requestMaintenanceModel",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_phraseModel_requestMaintenanceModel_RequestMaintenanceID",
                table: "phraseModel");

            migrationBuilder.DropForeignKey(
                name: "FK_phraseModel_requestMaintenanceModel_RequestMaintenanceModelId",
                table: "phraseModel");

            migrationBuilder.DropIndex(
                name: "IX_phraseModel_RequestMaintenanceID",
                table: "phraseModel");

            migrationBuilder.DropIndex(
                name: "IX_phraseModel_RequestMaintenanceModelId",
                table: "phraseModel");

            migrationBuilder.DropColumn(
                name: "RequestMaintenanceModelId",
                table: "phraseModel");

            migrationBuilder.CreateIndex(
                name: "IX_phraseModel_RequestMaintenanceID",
                table: "phraseModel",
                column: "RequestMaintenanceID",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_phraseModel_requestMaintenanceModel_RequestMaintenanceID",
                table: "phraseModel",
                column: "RequestMaintenanceID",
                principalTable: "requestMaintenanceModel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
