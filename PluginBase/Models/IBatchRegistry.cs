using Microsoft.AspNetCore.Http;

namespace PluginBase.Models
{
    public delegate BatchProcessResults HfBatchOperation(BatchJob batchJob);
    public delegate void HfBatchLoad(IFormFile fileData);


    public interface IBatchRegistry
    {
        HfBatchOperation BatchEnqueue { get; }
        HfBatchOperation BatchExecute { get; }
        HfBatchLoad LoadBatch { get; }
        BatchDetails BatchDetails { get; }
    }
}