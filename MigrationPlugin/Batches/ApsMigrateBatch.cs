using MigrationPlugin.Controllers;
using PluginBase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MigrationPlugin.Batches
{
    public class ApsMigrateBatch : IBatchRegistry
    {
        public string Name => "APS Simple Migrate";


        public HfBatchOperation BatchExecute => throw new NotImplementedException();

        public HfBatchOperation BuildStack => throw new NotImplementedException();

        public HfBatchOperation GetTemplate => throw new NotImplementedException();

        public HfBatchLoad LoadBatch => throw new NotImplementedException();

        public BatchDetails BatchDetails => new BatchDetails
        {
            Title = "Migration",
            Key = "migrate_v1",
            Description = "Basic Migration of Files to the Autodesk Cloud",
            BatchLoadUrl = BatchBasicMigrationController.PATH_LOAD,
            BatchTemplateUrl = BatchBasicMigrationController.PATH_TEMPLATE
        };

        public HfBatchOperation BatchEnqueue => throw new NotImplementedException();
    }
}
