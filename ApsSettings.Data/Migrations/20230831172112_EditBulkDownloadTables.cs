using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApsSettings.Data.Migrations
{
    /// <inheritdoc />
    public partial class EditBulkDownloadTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "BulkDownloads",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_BulkDownloadFiles_BulkDownloadId",
                table: "BulkDownloadFiles",
                column: "BulkDownloadId");

            migrationBuilder.AddForeignKey(
                name: "FK_BulkDownloadFiles_BulkDownloads_BulkDownloadId",
                table: "BulkDownloadFiles",
                column: "BulkDownloadId",
                principalTable: "BulkDownloads",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BulkDownloadFiles_BulkDownloads_BulkDownloadId",
                table: "BulkDownloadFiles");

            migrationBuilder.DropIndex(
                name: "IX_BulkDownloadFiles_BulkDownloadId",
                table: "BulkDownloadFiles");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "BulkDownloads");
        }
    }
}
