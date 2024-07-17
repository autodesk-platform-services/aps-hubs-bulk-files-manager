using ApsSettings.Data;
using ApsSettings.Data.Models;
using Bulk_Uploader_Electron.ClientApp.src.Utilities;
using Bulk_Uploader_Electron.Utils;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Bcpg.OpenPgp;
using PluginBase.Models;
using Serilog;

namespace Bulk_Uploader_Electron.Controllers
{
    public class BatchProcessingController : ControllerBase
    {
        public readonly DataContext DataContext;

        public BatchProcessingController(DataContext dataContext)
        {
            DataContext = dataContext;
        }

        [HttpGet]
        [Route("api/batch")]
        public async Task<ActionResult<List<BatchDetails>>> GetBatchTypes()
        {
            var items = BatchManager.Instance.Batches.Select(p => p.BatchDetails); ;
            return Ok(items);
        }

        [HttpGet]
        [Route("api/batch/active")]
        public async Task<ActionResult<List<BatchJob>>> GetActiveBatchJobs()
        {
            try
            {
                var items =
                DataContext.Batches.Where(p => p.Completed == null).ToList();
                return Ok(items);
            }
            catch (Exception ex)
            {
                Log.Error("GetActiveBatchJobs", ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("api/batch/process")]
        public async Task<ActionResult<BatchProcessResults>> ProcessBatch([FromBody] BatchProcessPayload payload)
        {
            try
            {
                var batchRegistry = BatchManager.Instance.GetBatch(payload.BatchId);
                if (batchRegistry == null) throw new NullReferenceException(nameof(batchRegistry) + " Not found in batch registry");
                var batch = this.DataContext.Batches.Find(payload.BatchId);
                if (batch == null) throw new NullReferenceException(nameof(Batch) + " Not found in database");
                //if (payload.Action == "template")
                //{
                //    return Ok(batchRegistry.GetTemplate(batch));
                //}
                //else if (payload.Action == "template")
                //{
                //    return Ok(batchRegistry.GetTemplate(batch));
                //}
                //else if (payload.Action == "start")
                //{
                //    return Ok(batchRegistry.GetTemplate(batch));
                //}
                //else
                //{
                //    throw new InvalidOperationException(payload.Action + " is not a valid action");
                //}
                return Ok();
            }
            catch (Exception ex)
            {
                var errorResult = new BatchProcessResults
                {
                    BatchId = payload.BatchId,
                    DbId = payload.DbId,
                    Status = "error"
                };
                errorResult.Errors.Add(ex.Message);
                return BadRequest(errorResult);
            }
        }

        //[HttpPost]
        //[Route("api/batch/key/{batchKey}/operation/{{batchOperation}")]
        //public async Task<ActionResult<BatchProcessResults>> TestProcess([FromBody] BatchProcessPayload payload, [FromQuery] PgpSecretKeyRing batchKey, [FromQuery] string batchOperation)
        //{
        //    try
        //    {
        //        var batchRegistry = BatchManager.Instance.GetBatch(payload.BatchId);
        //        if (batchRegistry == null) throw new NullReferenceException(nameof(batchRegistry) + " Not found in batch registry");
        //        var batch = this.DataContext.Batches.Find(payload.BatchId);
        //        if (batch == null) throw new NullReferenceException(nameof(Batch) + " Not found in database");
        //        //if (payload.Action == "template")
        //        //{
        //        //    return Ok(batchRegistry.GetTemplate(batch));
        //        //}
        //        //else if (payload.Action == "template")
        //        //{
        //        //    return Ok(batchRegistry.GetTemplate(batch));
        //        //}
        //        //else if (payload.Action == "start")
        //        //{
        //        //    return Ok(batchRegistry.GetTemplate(batch));
        //        //}
        //        //else
        //        //{
        //        //    throw new InvalidOperationException(payload.Action + " is not a valid action");
        //        //}
        //        return Ok();
        //    }
        //    catch (Exception ex)
        //    {
        //        var errorResult = new BatchProcessResults
        //        {
        //            BatchId = payload.BatchId,
        //            DbId = payload.DbId,
        //            Status = "error"
        //        };
        //        errorResult.Errors.Add(ex.Message);
        //        return BadRequest(errorResult);
        //    }
        //}

        [HttpPost]
        [Route("api/batch/load/{batchKey}")]
        public async Task<ActionResult<BatchProcessResults>> TestLoad([FromRoute] string batchKey, [FromForm] IFormFile FileData)
        {
            try
            {
                var batch = BatchManager.Instance.GetBatch(batchKey);
                if (batch == null) throw new NullReferenceException($"Batch {batchKey ?? "Unknown"} cannot be found");
                batch.LoadBatch(FileData);

                //if (payload.Action == "template")
                //{
                //    return Ok(batchRegistry.GetTemplate(batch));
                //}
                //else if (payload.Action == "template")
                //{
                //    return Ok(batchRegistry.GetTemplate(batch));
                //}
                //else if (payload.Action == "start")
                //{
                //    return Ok(batchRegistry.GetTemplate(batch));
                //}
                //else
                //{
                //    throw new InvalidOperationException(payload.Action + " is not a valid action");
                //}
                return Ok();
            }
            catch (Exception ex)
            {
                var errorResult = new BatchProcessResults
                {
                    BatchId = "3",//"payload.BatchId,
                    DbId = 5,//"payload.BatchId,
                    Status = "error"
                };
                errorResult.Errors.Add(ex.Message);
                return BadRequest(errorResult);
            }
        }

        //[HttpGet]
        //[Route("api/batch/enqueue/{batchJobId}")]
        //public async Task<ActionResult<List<BatchJob>>> GetActiveBatchJobs(int batchJobId)
        //{
        //    try
        //    {
        //       // BatchManager.Instance.
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error("GetActiveBatchJobs", ex);
        //        return BadRequest(ex.Message);
        //    }

        //}
    }
}