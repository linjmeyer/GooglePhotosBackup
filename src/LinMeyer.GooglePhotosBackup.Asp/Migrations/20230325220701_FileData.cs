using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinMeyer.GooglePhotosBackup.Asp.Migrations
{
    /// <inheritdoc />
    public partial class FileData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LocalPath",
                table: "FileStatuses",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RemoteId",
                table: "FileStatuses",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "RemoteKind",
                table: "FileStatuses",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LocalPath",
                table: "FileStatuses");

            migrationBuilder.DropColumn(
                name: "RemoteId",
                table: "FileStatuses");

            migrationBuilder.DropColumn(
                name: "RemoteKind",
                table: "FileStatuses");
        }
    }
}
