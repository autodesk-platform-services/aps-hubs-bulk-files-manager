using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApsSettings.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Batches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    Errors = table.Column<string>(type: "TEXT", nullable: true),
                    Data = table.Column<string>(type: "TEXT", nullable: true),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Queued = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Started = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Completed = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Batches", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BulkUploadPresets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    ExcludedFolderNames = table.Column<string>(type: "TEXT", nullable: false),
                    ExcludedFileTypes = table.Column<string>(type: "TEXT", nullable: false),
                    ModifyPathScript = table.Column<string>(type: "TEXT", nullable: false),
                    UseModifyPathScript = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BulkUploadPresets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BulkUploads",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    LocalPath = table.Column<string>(type: "TEXT", nullable: false),
                    ProjectId = table.Column<string>(type: "TEXT", nullable: false),
                    FolderId = table.Column<string>(type: "TEXT", nullable: false),
                    ExcludedFolderNames = table.Column<string>(type: "TEXT", nullable: false),
                    ExcludedFileTypes = table.Column<string>(type: "TEXT", nullable: false),
                    ModifyPathScript = table.Column<string>(type: "TEXT", nullable: false),
                    UseModifyPathScript = table.Column<bool>(type: "INTEGER", nullable: false),
                    Logs = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    StartTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndTime = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BulkUploads", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    SettingId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Key = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.SettingId);
                });

            migrationBuilder.CreateTable(
                name: "BatchTasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    StartedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastUpdateOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CompletedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Properties = table.Column<string>(type: "TEXT", nullable: false),
                    Errors = table.Column<string>(type: "TEXT", nullable: true),
                    JobOwnerId = table.Column<int>(type: "INTEGER", nullable: true),
                    TotalSteps = table.Column<int>(type: "INTEGER", nullable: false),
                    StepsCompleted = table.Column<int>(type: "INTEGER", nullable: false),
                    ActiveStep = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BatchTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BatchTasks_Batches_JobOwnerId",
                        column: x => x.JobOwnerId,
                        principalTable: "Batches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BulkUploadAutodeskMirrorFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BulkUploadId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    FolderUrn = table.Column<string>(type: "TEXT", nullable: false),
                    ItemId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BulkUploadAutodeskMirrorFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BulkUploadAutodeskMirrorFiles_BulkUploads_BulkUploadId",
                        column: x => x.BulkUploadId,
                        principalTable: "BulkUploads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BulkUploadAutodeskMirrors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BulkUploadId = table.Column<int>(type: "INTEGER", nullable: false),
                    FolderName = table.Column<string>(type: "TEXT", nullable: false),
                    RelativeFolderPath = table.Column<string>(type: "TEXT", nullable: false),
                    FolderUrn = table.Column<string>(type: "TEXT", nullable: false),
                    FolderUrl = table.Column<string>(type: "TEXT", nullable: false),
                    ContentsRetrieved = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BulkUploadAutodeskMirrors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BulkUploadAutodeskMirrors_BulkUploads_BulkUploadId",
                        column: x => x.BulkUploadId,
                        principalTable: "BulkUploads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BulkUploadFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BulkUploadId = table.Column<int>(type: "INTEGER", nullable: false),
                    SourceFileName = table.Column<string>(type: "TEXT", nullable: false),
                    SourceAbsolutePath = table.Column<string>(type: "TEXT", nullable: false),
                    SourceRelativePath = table.Column<string>(type: "TEXT", nullable: false),
                    TargetFileName = table.Column<string>(type: "TEXT", nullable: false),
                    TargetRelativePath = table.Column<string>(type: "TEXT", nullable: false),
                    FolderUrn = table.Column<string>(type: "TEXT", nullable: false),
                    FolderUrl = table.Column<string>(type: "TEXT", nullable: false),
                    ItemId = table.Column<string>(type: "TEXT", nullable: false),
                    ObjectId = table.Column<string>(type: "TEXT", nullable: false),
                    VersionId = table.Column<string>(type: "TEXT", nullable: false),
                    WebUrl = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    Logs = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastModified = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BulkUploadFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BulkUploadFiles_BulkUploads_BulkUploadId",
                        column: x => x.BulkUploadId,
                        principalTable: "BulkUploads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StepHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StepNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    StepResult = table.Column<string>(type: "TEXT", nullable: true),
                    Error = table.Column<string>(type: "TEXT", nullable: true),
                    JobTaskAggregatorId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StepHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StepHistory_BatchTasks_JobTaskAggregatorId",
                        column: x => x.JobTaskAggregatorId,
                        principalTable: "BatchTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaskStep",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AggregatorId = table.Column<int>(type: "INTEGER", nullable: false),
                    StepType = table.Column<string>(type: "TEXT", nullable: false),
                    StepOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Data = table.Column<string>(type: "TEXT", nullable: true),
                    StartedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CompletedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ErrorMessage = table.Column<string>(type: "TEXT", nullable: true),
                    ExecutionTime = table.Column<TimeSpan>(type: "TEXT", nullable: true),
                    ExecutionRate = table.Column<double>(type: "REAL", nullable: true),
                    Retries = table.Column<int>(type: "INTEGER", nullable: false),
                    RetryCount = table.Column<int>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskStep", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskStep_BatchTasks_AggregatorId",
                        column: x => x.AggregatorId,
                        principalTable: "BatchTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BatchTasks_JobOwnerId",
                table: "BatchTasks",
                column: "JobOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_BulkUploadAutodeskMirrorFiles_BulkUploadId",
                table: "BulkUploadAutodeskMirrorFiles",
                column: "BulkUploadId");

            migrationBuilder.CreateIndex(
                name: "IX_BulkUploadAutodeskMirrors_BulkUploadId",
                table: "BulkUploadAutodeskMirrors",
                column: "BulkUploadId");

            migrationBuilder.CreateIndex(
                name: "IX_BulkUploadFiles_BulkUploadId",
                table: "BulkUploadFiles",
                column: "BulkUploadId");

            migrationBuilder.CreateIndex(
                name: "IX_StepHistory_JobTaskAggregatorId",
                table: "StepHistory",
                column: "JobTaskAggregatorId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskStep_AggregatorId",
                table: "TaskStep",
                column: "AggregatorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BulkUploadAutodeskMirrorFiles");

            migrationBuilder.DropTable(
                name: "BulkUploadAutodeskMirrors");

            migrationBuilder.DropTable(
                name: "BulkUploadFiles");

            migrationBuilder.DropTable(
                name: "BulkUploadPresets");

            migrationBuilder.DropTable(
                name: "Settings");

            migrationBuilder.DropTable(
                name: "StepHistory");

            migrationBuilder.DropTable(
                name: "TaskStep");

            migrationBuilder.DropTable(
                name: "BulkUploads");

            migrationBuilder.DropTable(
                name: "BatchTasks");

            migrationBuilder.DropTable(
                name: "Batches");
        }
    }
}
