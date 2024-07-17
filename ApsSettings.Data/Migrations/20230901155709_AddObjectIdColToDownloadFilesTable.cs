using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApsSettings.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddObjectIdColToDownloadFilesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ObjectId",
                table: "BulkDownloadFiles",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ObjectId",
                table: "BulkDownloadFiles");
        }
    }
}
