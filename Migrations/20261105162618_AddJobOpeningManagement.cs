using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CampusPlacementManagementPlatform.Migrations
{
    /// <inheritdoc />
    public partial class AddJobOpeningManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    CompanyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CompanyEmail = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Website = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Industry = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.CompanyId);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    StudentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CollegeId = table.Column<int>(type: "int", nullable: false),
                    PlacementOfficerId = table.Column<int>(type: "int", nullable: false),
                    RegisterNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Department = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Degree = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    GraduationYear = table.Column<int>(type: "int", nullable: false),
                    CGPA = table.Column<decimal>(type: "decimal(4,2)", precision: 4, scale: 2, nullable: false),
                    ActiveBacklogs = table.Column<int>(type: "int", nullable: false),
                    ResumeUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.StudentId);
                    table.ForeignKey(
                        name: "FK_Students_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Students_Colleges_CollegeId",
                        column: x => x.CollegeId,
                        principalTable: "Colleges",
                        principalColumn: "CollegeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Students_PlacementOfficers_PlacementOfficerId",
                        column: x => x.PlacementOfficerId,
                        principalTable: "PlacementOfficers",
                        principalColumn: "PlacementOfficerId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CompanyHRs",
                columns: table => new
                {
                    CompanyHRId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ApprovedBy = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyHRs", x => x.CompanyHRId);
                    table.ForeignKey(
                        name: "FK_CompanyHRs_AspNetUsers_ApprovedBy",
                        column: x => x.ApprovedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompanyHRs_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompanyHRs_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "JobOpenings",
                columns: table => new
                {
                    JobOpeningId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    JobDescription = table.Column<string>(type: "nvarchar(3000)", maxLength: 3000, nullable: false),
                    Location = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    EmploymentType = table.Column<int>(type: "int", nullable: false),
                    SalaryMin = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: true),
                    SalaryMax = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: true),
                    MinimumCGPA = table.Column<decimal>(type: "decimal(4,2)", precision: 4, scale: 2, nullable: false),
                    MaximumBacklogs = table.Column<int>(type: "int", nullable: false),
                    EligibleGraduationYear = table.Column<int>(type: "int", nullable: false),
                    EligibleDepartments = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ApplicationDeadline = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CreatedByCompanyHRId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PublishedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ClosedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobOpenings", x => x.JobOpeningId);
                    table.ForeignKey(
                        name: "FK_JobOpenings_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobOpenings_CompanyHRs_CreatedByCompanyHRId",
                        column: x => x.CreatedByCompanyHRId,
                        principalTable: "CompanyHRs",
                        principalColumn: "CompanyHRId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "JobTargetColleges",
                columns: table => new
                {
                    JobTargetCollegeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobOpeningId = table.Column<int>(type: "int", nullable: false),
                    CollegeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobTargetColleges", x => x.JobTargetCollegeId);
                    table.ForeignKey(
                        name: "FK_JobTargetColleges_Colleges_CollegeId",
                        column: x => x.CollegeId,
                        principalTable: "Colleges",
                        principalColumn: "CollegeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobTargetColleges_JobOpenings_JobOpeningId",
                        column: x => x.JobOpeningId,
                        principalTable: "JobOpenings",
                        principalColumn: "JobOpeningId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompanyHRs_ApprovedBy",
                table: "CompanyHRs",
                column: "ApprovedBy");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyHRs_CompanyId",
                table: "CompanyHRs",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyHRs_UserId",
                table: "CompanyHRs",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobOpenings_CompanyId",
                table: "JobOpenings",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_JobOpenings_CreatedByCompanyHRId",
                table: "JobOpenings",
                column: "CreatedByCompanyHRId");

            migrationBuilder.CreateIndex(
                name: "IX_JobTargetColleges_CollegeId",
                table: "JobTargetColleges",
                column: "CollegeId");

            migrationBuilder.CreateIndex(
                name: "IX_JobTargetColleges_JobOpeningId_CollegeId",
                table: "JobTargetColleges",
                columns: new[] { "JobOpeningId", "CollegeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Students_CollegeId_RegisterNumber",
                table: "Students",
                columns: new[] { "CollegeId", "RegisterNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Students_PlacementOfficerId",
                table: "Students",
                column: "PlacementOfficerId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_UserId",
                table: "Students",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobTargetColleges");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "JobOpenings");

            migrationBuilder.DropTable(
                name: "CompanyHRs");

            migrationBuilder.DropTable(
                name: "Companies");
        }
    }
}
