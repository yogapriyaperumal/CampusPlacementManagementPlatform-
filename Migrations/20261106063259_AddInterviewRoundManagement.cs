using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CampusPlacementManagementPlatform.Migrations
{
    /// <inheritdoc />
    public partial class AddInterviewRoundManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InterviewRounds",
                columns: table => new
                {
                    InterviewRoundId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RecruitmentDriveId = table.Column<int>(type: "int", nullable: false),
                    RoundName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    RoundNumber = table.Column<int>(type: "int", nullable: false),
                    RoundType = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DurationMinutes = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewRounds", x => x.InterviewRoundId);
                    table.ForeignKey(
                        name: "FK_InterviewRounds_RecruitmentDrives_RecruitmentDriveId",
                        column: x => x.RecruitmentDriveId,
                        principalTable: "RecruitmentDrives",
                        principalColumn: "RecruitmentDriveId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InterviewRounds_RecruitmentDriveId_RoundNumber",
                table: "InterviewRounds",
                columns: new[] { "RecruitmentDriveId", "RoundNumber" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InterviewRounds");
        }
    }
}
