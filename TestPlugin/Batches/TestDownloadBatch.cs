using ApsSettings.Data;
using Ganss.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using PluginBase.Models;
using TestPlugin.Controllers;
using TestPlugin.Models;
using TestPlugin.Plugin;

namespace MigrationPlugin.Batches
{
    internal class TestDownloadBatch : IBatchRegistry
    {
        public string Name => "Test Download";

        public HfBatchOperation BatchExecute => throw new NotImplementedException();

        public HfBatchOperation BuildStack => throw new NotImplementedException();

        public HfBatchLoad LoadBatch { get => ProcessBatch; }

        public HfBatchOperation BatchEnqueue => EnqueueStart;

        public BatchDetails BatchDetails => new BatchDetails
        {
            Title = "Test 1",
            Key = "test_1_v1",
            Description = "Test of a plugin",
            BatchLoadUrl = TestBatchController.PATH_LOAD,
            BatchTemplateUrl = TestBatchController.PATH_TEMPLATE
        };

        protected void ProcessBatch(IFormFile fileData)
        {
            string name = fileData.FileName;
            string extension = Path.GetExtension(fileData.FileName);

            using (var memoryStream = new MemoryStream())
            {
                fileData.CopyTo(memoryStream);
                var items = new ExcelMapper(memoryStream).Fetch<TestDownload>();
                var context = TestPluginObj.Instance.ServiceProvider.GetService<DataContext>()!;

                foreach (var batchData in items)
                {
                    var jb = new BatchJob
                    {
                        Name = batchData.Name,
                        Data = JsonConvert.SerializeObject(batchData),
                        Type = this.BatchDetails.Key
                    };

                    context.Batches.Add(jb);
                }

                context.SaveChanges();
            }
        }

        private BatchProcessResults EnqueueStart(BatchJob batchJob)
        {
            try
            {
                var context = TestPluginObj.Instance.ServiceProvider.GetService<DataContext>()!;
            }
            catch (Exception ex)
            {
                throw;
            }

            return null;
        }
    }
}