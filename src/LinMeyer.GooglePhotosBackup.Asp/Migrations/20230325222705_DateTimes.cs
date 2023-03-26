using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinMeyer.GooglePhotosBackup.Asp.Migrations
{
    /// <inheritdoc />
    public partial class DateTimes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Checked",
                table: "FileStatuses",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "FileStatuses",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "Updated",
                table: "FileStatuses",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Checked",
                table: "FileStatuses");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "FileStatuses");

            migrationBuilder.DropColumn(
                name: "Updated",
                table: "FileStatuses");
        }
    }
}
