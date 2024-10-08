﻿// <auto-generated />
using System;
using ApsSettings.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ApsSettings.Data.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.22");

            modelBuilder.Entity("ApsSettings.Data.Models.BulkDownload", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ApsFolderId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("CloudPath")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("HubId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("LocalPath")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Logs")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ProjectId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("BulkDownloads");
                });

            modelBuilder.Entity("ApsSettings.Data.Models.BulkDownloadFile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("BulkDownloadId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("TEXT");

                    b.Property<string>("DestinationFilePath")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<long>("FileSize")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ItemId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("LastModified")
                        .HasColumnType("TEXT");

                    b.Property<string>("Logs")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ObjectId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("SourceFilePath")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("BulkDownloadId");

                    b.ToTable("BulkDownloadFiles");
                });

            modelBuilder.Entity("ApsSettings.Data.Models.BulkUpload", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("EndTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("ExcludedFileTypes")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ExcludedFolderNames")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("FolderId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("LocalPath")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Logs")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ModifyPathScript")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ProjectId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("TEXT");

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("UseModifyPathScript")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("BulkUploads");
                });

            modelBuilder.Entity("ApsSettings.Data.Models.BulkUploadAutodeskMirror", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("BulkUploadId")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("ContentsRetrieved")
                        .HasColumnType("INTEGER");

                    b.Property<string>("FolderName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("FolderUrl")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("FolderUrn")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("RelativeFolderPath")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("BulkUploadId");

                    b.ToTable("BulkUploadAutodeskMirrors");
                });

            modelBuilder.Entity("ApsSettings.Data.Models.BulkUploadAutodeskMirrorFile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("BulkUploadId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("FolderUrn")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ItemId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("BulkUploadId");

                    b.ToTable("BulkUploadAutodeskMirrorFiles");
                });

            modelBuilder.Entity("ApsSettings.Data.Models.BulkUploadFile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("BulkUploadId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("TEXT");

                    b.Property<string>("FolderUrl")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("FolderUrn")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ItemId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("LastModified")
                        .HasColumnType("TEXT");

                    b.Property<string>("Logs")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("BucketKey")
                       .IsRequired()
                       .HasColumnType("TEXT");

                    b.Property<string>("ObjectId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("SourceAbsolutePath")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("SourceFileName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("SourceRelativePath")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER");

                    b.Property<string>("TargetFileName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("TargetRelativePath")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("VersionId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("WebUrl")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("BulkUploadId");

                    b.ToTable("BulkUploadFiles");
                });

            modelBuilder.Entity("ApsSettings.Data.Models.BulkUploadPreset", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ExcludedFileTypes")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ExcludedFolderNames")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ModifyPathScript")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("UseModifyPathScript")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("BulkUploadPresets");
                });

            modelBuilder.Entity("ApsSettings.Data.Models.Setting", b =>
                {
                    b.Property<int>("SettingId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("SettingId");

                    b.ToTable("Settings");
                });

            modelBuilder.Entity("PluginBase.Models.BatchJob", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("Completed")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("Created")
                        .HasColumnType("TEXT");

                    b.Property<string>("Data")
                        .HasColumnType("TEXT");

                    b.Property<string>("Errors")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("Queued")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("Started")
                        .HasColumnType("TEXT");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Batches");
                });

            modelBuilder.Entity("PluginBase.Models.JobTaskAggregator", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ActiveStep")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("CompletedOn")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Errors")
                        .HasColumnType("TEXT");

                    b.Property<int?>("JobOwnerId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("LastUpdateOn")
                        .HasColumnType("TEXT");

                    b.Property<string>("Properties")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("StartedOn")
                        .HasColumnType("TEXT");

                    b.Property<int>("StepsCompleted")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TotalSteps")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("JobOwnerId");

                    b.ToTable("BatchTasks");
                });

            modelBuilder.Entity("ApsSettings.Data.Models.BulkDownloadFile", b =>
                {
                    b.HasOne("ApsSettings.Data.Models.BulkDownload", null)
                        .WithMany("Files")
                        .HasForeignKey("BulkDownloadId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ApsSettings.Data.Models.BulkUploadAutodeskMirror", b =>
                {
                    b.HasOne("ApsSettings.Data.Models.BulkUpload", "BulkUpload")
                        .WithMany("AutodeskMirrors")
                        .HasForeignKey("BulkUploadId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("BulkUpload");
                });

            modelBuilder.Entity("ApsSettings.Data.Models.BulkUploadAutodeskMirrorFile", b =>
                {
                    b.HasOne("ApsSettings.Data.Models.BulkUpload", "BulkUpload")
                        .WithMany()
                        .HasForeignKey("BulkUploadId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("BulkUpload");
                });

            modelBuilder.Entity("ApsSettings.Data.Models.BulkUploadFile", b =>
                {
                    b.HasOne("ApsSettings.Data.Models.BulkUpload", null)
                        .WithMany("Files")
                        .HasForeignKey("BulkUploadId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("PluginBase.Models.JobTaskAggregator", b =>
                {
                    b.HasOne("PluginBase.Models.BatchJob", "JobOwner")
                        .WithMany("Jobs")
                        .HasForeignKey("JobOwnerId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.OwnsMany("PluginBase.Models.TaskStep", "Steps", b1 =>
                        {
                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("INTEGER");

                            b1.Property<int>("AggregatorId")
                                .HasColumnType("INTEGER");

                            b1.Property<DateTime?>("CompletedOn")
                                .HasColumnType("TEXT");

                            b1.Property<string>("Data")
                                .HasColumnType("TEXT");

                            b1.Property<string>("Description")
                                .IsRequired()
                                .HasColumnType("TEXT");

                            b1.Property<string>("ErrorMessage")
                                .HasColumnType("TEXT");

                            b1.Property<double?>("ExecutionRate")
                                .HasColumnType("REAL");

                            b1.Property<TimeSpan?>("ExecutionTime")
                                .HasColumnType("TEXT");

                            b1.Property<string>("Notes")
                                .HasColumnType("TEXT");

                            b1.Property<int>("Retries")
                                .HasColumnType("INTEGER");

                            b1.Property<int>("RetryCount")
                                .HasColumnType("INTEGER");

                            b1.Property<DateTime?>("StartedOn")
                                .HasColumnType("TEXT");

                            b1.Property<int>("StepOrder")
                                .HasColumnType("INTEGER");

                            b1.Property<string>("StepType")
                                .IsRequired()
                                .HasColumnType("TEXT");

                            b1.HasKey("Id");

                            b1.HasIndex("AggregatorId");

                            b1.ToTable("TaskStep");

                            b1.WithOwner()
                                .HasForeignKey("AggregatorId");
                        });

                    b.OwnsMany("PluginBase.Models.StepHistory", "History", b1 =>
                        {
                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("INTEGER");

                            b1.Property<string>("Error")
                                .HasColumnType("TEXT");

                            b1.Property<int>("JobTaskAggregatorId")
                                .HasColumnType("INTEGER");

                            b1.Property<int>("StepNumber")
                                .HasColumnType("INTEGER");

                            b1.Property<string>("StepResult")
                                .HasColumnType("TEXT");

                            b1.HasKey("Id");

                            b1.HasIndex("JobTaskAggregatorId");

                            b1.ToTable("StepHistory");

                            b1.WithOwner()
                                .HasForeignKey("JobTaskAggregatorId");
                        });

                    b.Navigation("History");

                    b.Navigation("JobOwner");

                    b.Navigation("Steps");
                });

            modelBuilder.Entity("ApsSettings.Data.Models.BulkDownload", b =>
                {
                    b.Navigation("Files");
                });

            modelBuilder.Entity("ApsSettings.Data.Models.BulkUpload", b =>
                {
                    b.Navigation("AutodeskMirrors");

                    b.Navigation("Files");
                });

            modelBuilder.Entity("PluginBase.Models.BatchJob", b =>
                {
                    b.Navigation("Jobs");
                });
#pragma warning restore 612, 618
        }
    }
}
