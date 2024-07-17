using MigrationPlugin.Controllers;
using PluginBase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MigrationPlugin.Batches
{
    internal class ApsUploadBatch : IBatchRegistry
    {
        public string Name => "APS Upload";

        public HfBatchOperation BatchExecute => throw new NotImplementedException();

        public HfBatchOperation BuildStack => throw new NotImplementedException();

        public HfBatchOperation GetTemplate => throw new NotImplementedException();

        public HfBatchLoad LoadBatch => throw new NotImplementedException();

        public BatchDetails BatchDetails => new BatchDetails
        {
            Title = "Upload",
            Key="upload_v1",
            Description = "Basic Upload of Files to the Autodesk Cloud",
            BatchLoadUrl = BatchUploadController.PATH_LOAD,
            BatchTemplateUrl = BatchUploadController.PATH_TEMPLATE
        };

        public HfBatchOperation BatchEnqueue => throw new NotImplementedException();
    }
}
