using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanArchitectureTemplate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBookingLecturerApprovalFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Bookings",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AddColumn<DateTime>(
                name: "LecturerApprovedAt",
                table: "Bookings",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LecturerApprovedBy",
                table: "Bookings",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LecturerEmail",
                table: "Bookings",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LecturerRejectReason",
                table: "Bookings",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_LecturerApprovedBy",
                table: "Bookings",
                column: "LecturerApprovedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_LecturerEmail",
                table: "Bookings",
                column: "LecturerEmail");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Users_LecturerApprovedBy",
                table: "Bookings",
                column: "LecturerApprovedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Users_LecturerApprovedBy",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_LecturerApprovedBy",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_LecturerEmail",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "LecturerApprovedAt",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "LecturerApprovedBy",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "LecturerEmail",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "LecturerRejectReason",
                table: "Bookings");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Bookings",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30);
        }
    }
}
