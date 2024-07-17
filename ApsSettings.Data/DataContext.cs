using ApsSettings.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Newtonsoft.Json;
using PluginBase.Models;
using static PluginBase.Models.JobTaskAggregator;

namespace ApsSettings.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Setting> Settings { get; set; }
        public DbSet<BulkUpload> BulkUploads { get; set; }
        public DbSet<BulkUploadFile> BulkUploadFiles { get; set; }
        public DbSet<BulkDownload> BulkDownloads { get; set; }
        public DbSet<BulkDownloadFile> BulkDownloadFiles { get; set; }
        public DbSet<BatchJob> Batches { get; set; }
        public DbSet<JobTaskAggregator> BatchTasks { get; set; }
        public DbSet<BulkUploadPreset> BulkUploadPresets { get; set; }
        public DbSet<BulkUploadAutodeskMirror> BulkUploadAutodeskMirrors { get; set; }
        public DbSet<BulkUploadAutodeskMirrorFile> BulkUploadAutodeskMirrorFiles { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BatchJob>()
                .HasMany(p => p.Jobs).WithOne(p => p.JobOwner).HasForeignKey(p => p.JobOwnerId).OnDelete(DeleteBehavior.Cascade); ;

            modelBuilder.Entity<JobTaskAggregator>().OwnsMany(
            p => p.Steps, a =>
            {
                a.WithOwner().HasForeignKey(p => p.AggregatorId);
                a.Property<int>(p => p.Id);
                a.HasKey(p => p.Id);
            });

            modelBuilder.Entity<JobTaskAggregator>()
           .Property(e => e.Properties)
           .IsRequired()
           .HasConversion(
               v => JsonConvert.SerializeObject(v),
                v => v == null
                    ? new Dictionary<string, string>() // fallback
                    : JsonConvert.DeserializeObject<Dictionary<string, string>>(v)
           );

            modelBuilder.Entity<JobTaskAggregator>().OwnsMany(
            p => p.History, a =>
            {
                a.WithOwner();
                a.Property<int>(p => p.Id);
                a.HasKey(p => p.Id);
            });
        }
    }

    public class DataContextFactory : IDesignTimeDbContextFactory<DataContext>
    {
        public DataContext CreateDbContext(string[] args = null)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DataContext>();
            optionsBuilder.UseSqlite("Data Source=Bulk-Uploader.db");

            return new DataContext(optionsBuilder.Options);
        }
    }
}