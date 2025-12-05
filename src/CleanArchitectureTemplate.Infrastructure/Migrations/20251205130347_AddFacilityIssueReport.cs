using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanArchitectureTemplate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFacilityIssueReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FacilityIssueReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ReportCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    BookingId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReportedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    IssueTitle = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    IssueDescription = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Severity = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Category = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ImageUrls = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    NewFacilityId = table.Column<Guid>(type: "uuid", nullable: true),
                    HandledBy = table.Column<Guid>(type: "uuid", nullable: true),
                    AdminResponse = table.Column<string>(type: "text", nullable: true),
                    HandledAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ResolvedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacilityIssueReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FacilityIssueReports_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FacilityIssueReports_Facilities_NewFacilityId",
                        column: x => x.NewFacilityId,
                        principalTable: "Facilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FacilityIssueReports_Users_HandledBy",
                        column: x => x.HandledBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FacilityIssueReports_Users_ReportedBy",
                        column: x => x.ReportedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FacilityIssueReports_BookingId",
                table: "FacilityIssueReports",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_FacilityIssueReports_HandledBy",
                table: "FacilityIssueReports",
                column: "HandledBy");

            migrationBuilder.CreateIndex(
                name: "IX_FacilityIssueReports_IsDeleted",
                table: "FacilityIssueReports",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_FacilityIssueReports_NewFacilityId",
                table: "FacilityIssueReports",
                column: "NewFacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_FacilityIssueReports_ReportCode",
                table: "FacilityIssueReports",
                column: "ReportCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FacilityIssueReports_ReportedBy",
                table: "FacilityIssueReports",
                column: "ReportedBy");

            migrationBuilder.CreateIndex(
                name: "IX_FacilityIssueReports_Status",
                table: "FacilityIssueReports",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FacilityIssueReports");
        }
    }
}
