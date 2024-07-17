using ApsSettings.Data;
using Microsoft.Extensions.DependencyInjection;
using MigrationPlugin.Controllers;
using MigrationPlugin.Plugin;
using PluginBase.Models;

namespace MigrationPlugin.Batches
{
    internal class ApsDownloadBatch : IBatchRegistry
    {
        public string Name => "APS Download";

        public HfBatchOperation BatchExecute => throw new NotImplementedException();

        public HfBatchOperation BuildStack => throw new NotImplementedException();

        public HfBatchOperation GetTemplate => throw new NotImplementedException();

        public HfBatchLoad LoadBatch => throw new NotImplementedException();

        public BatchDetails BatchDetails => new BatchDetails
        {
            Title = "Download",
            Key = "download_v1",
            Description = "Basic Download of Files from the Autodesk Cloud",
            BatchLoadUrl = BatchDownloadController.PATH_LOAD,
            BatchTemplateUrl = BatchDownloadController.PATH_TEMPLATE
        };

        public HfBatchOperation BatchEnqueue => EnqueueStart;

        private BatchProcessResults EnqueueStart(BatchJob batchJob)
        {
            try
            {
                var context = MigrationPluginObj.Instance.ServiceProvider.GetService<DataContext>()!;
            }
            catch (Exception ex)
            {
                throw;
            }

            return null;
        }
    }
}