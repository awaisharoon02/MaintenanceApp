using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MaintenanceApp.Migrations
{
    /// <inheritdoc />
    public partial class Maintenance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "requestMaintenanceModel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestID = table.Column<int>(type: "int", nullable: false),
                    RequestDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Department = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequestedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReceivedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReceivedTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    Machine = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Proposal = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DescriptionAction = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MachineRunning = table.Column<TimeOnly>(type: "time", nullable: false),
                    RequestOpen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ActionTakenBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ActionTakenDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ActionApprovedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ActionApprovedDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ActionAcknowledgedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ActionAcknowledgedDate = table.Column<DateOnly>(type: "date", nullable: false),
                    MachineDowntime = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_requestMaintenanceModel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "phraseModel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Phrase = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsChecked = table.Column<bool>(type: "bit", nullable: false),
                    RequestMaintenanceID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_phraseModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_phraseModel_requestMaintenanceModel_RequestMaintenanceID",
                        column: x => x.RequestMaintenanceID,
                        principalTable: "requestMaintenanceModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_phraseModel_RequestMaintenanceID",
                table: "phraseModel",
                column: "RequestMaintenanceID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "phraseModel");

            migrationBuilder.DropTable(
                name: "requestMaintenanceModel");
        }
    }
}
