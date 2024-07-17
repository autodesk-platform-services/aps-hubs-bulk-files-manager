using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApsSettings.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveFieldFromDownloadJobTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "BulkDownloads");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "BulkDownloads",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
