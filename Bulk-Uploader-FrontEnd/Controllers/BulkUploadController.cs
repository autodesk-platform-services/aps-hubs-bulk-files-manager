using ApsSettings.Data;
using ApsSettings.Data.Models;
using Hangfire;
using mass_upload_via_s3_csharp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bulk_Uploader_Electron.Controllers;

public class BulkUploadController : ControllerBase
{
    public readonly DataContext DataContext;

    public BulkUploadController(DataContext dataContext)
    {
        DataContext = dataContext;
    }

    [HttpGet]
    [Route("api/bulkUploads")]
    public async Task<ActionResult> Get()
    {
        var bulkUploads = await DataContext.BulkUploads
            .OrderByDescending(x => x.StartTime)
            .ToListAsync();

        foreach(var bulkUpload in bulkUploads)
        {
            var fileStatuses = await DataContext.BulkUploadFiles
                .Where(x => x.BulkUploadId == bulkUpload.Id)
                .Select(x=>x.Status)
                .ToListAsync();
            
            bulkUpload.ProposedFileCount = fileStatuses
                .Where(x => x == JobFileStatus.Proposed)
                .LongCount();
            bulkUpload.DoNotUploadFileCount = fileStatuses
                .Where(x => x == JobFileStatus.DoNotUpload)
                .LongCount();
            bulkUpload.SuccessFileCount = fileStatuses
                .Where(x => x == JobFileStatus.Success)
                .LongCount();
            bulkUpload.PendingFileCount = fileStatuses
                .Where(x => x == JobFileStatus.Pending)
                .LongCount();
            bulkUpload.FailedFileCount = fileStatuses
                .Where(x => x == JobFileStatus.Failed)
                .LongCount();
            bulkUpload.Files = new List<BulkUploadFile>();
        };

        return Ok(bulkUploads);
    }

    [HttpGet]
    [Route("api/bulkUploads/{id}")]
    public async Task<ActionResult> Get(int id)
    {
        var bulkUpload = await DataContext.BulkUploads
            .Where(x => x.Id == id)
            .Include(x => x.AutodeskMirrors)
            .FirstOrDefaultAsync();

        if (bulkUpload == null) return NotFound();

        
        var fileStatuses = await DataContext.BulkUploadFiles
            .Where(x => x.BulkUploadId == bulkUpload.Id)
            .Select(x=>x.Status)
            .ToListAsync();
            
        bulkUpload.ProposedFileCount = fileStatuses
            .Where(x => x == JobFileStatus.Proposed)
            .LongCount();
        bulkUpload.DoNotUploadFileCount = fileStatuses
            .Where(x => x == JobFileStatus.DoNotUpload)
            .LongCount();
        bulkUpload.SuccessFileCount = fileStatuses
            .Where(x => x == JobFileStatus.Success)
            .LongCount();
        bulkUpload.PendingFileCount = fileStatuses
            .Where(x => x == JobFileStatus.Pending)
            .LongCount();
        bulkUpload.FailedFileCount = fileStatuses
            .Where(x => x == JobFileStatus.Failed)
            .LongCount();

        return Ok(bulkUpload);
    }
    [HttpGet]
    [Route("api/bulkUploads/{id}/status/{status}")]
    public async Task<ActionResult> Get(int id, JobFileStatus status, [FromQuery] int count = 1000, [FromQuery] int skip = 0)
    {
        var bulkUpload = await DataContext.BulkUploads
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync();

        if (bulkUpload == null) return NotFound();

        var files = await DataContext.BulkUploadFiles
            .Where(x => x.BulkUploadId == bulkUpload.Id)
            .Where(x => x.Status == status)
            .Skip(skip)
            .Take(count)
            .ToListAsync();
        
        return Ok(files);
    }

    [HttpPatch]
    [Route("api/bulkUploads")]
    public async Task<ActionResult> Patch([FromBody] List<BulkUpload> bulkUploads)
    {
        DataContext.BulkUploads.UpdateRange(bulkUploads);
        await DataContext.SaveChangesAsync();
        return Ok();
    }

    [HttpPost]
    [Route("api/bulkUploads")]
    public async Task<ActionResult> Post([FromBody] List<BulkUpload> bulkUploads)
    {
        foreach (var bulkUpload in bulkUploads)
        {
            bulkUpload.AddLogs("Created Bulk Upload");

            DataContext.BulkUploads.Add(bulkUpload);
            await DataContext.SaveChangesAsync();

            var bulkUploadId = bulkUpload.Id;
            var localPath = Path.GetDirectoryName(bulkUpload.LocalPath);

            var autodeskMirror = new BulkUploadAutodeskMirror()
            {
                BulkUploadId = bulkUploadId,
                RelativeFolderPath = "\\",
                FolderName = "",
                FolderUrl = "",
                FolderUrn = bulkUpload.FolderId
            };

            DataContext.BulkUploadAutodeskMirrors.Add(autodeskMirror);
            await DataContext.SaveChangesAsync();

            BackgroundJob.Enqueue<HangfireJobs>(x => x.BuildAutodeskMirror(bulkUploadId, autodeskMirror.Id));
            BackgroundJob.Enqueue<HangfireJobs>(x => x.KillOnceFinished(bulkUploadId));
        }

        return Created("api/bulkUploads", bulkUploads);
    }

    [HttpPost]
    [Route("api/bulkUploads/{bulkUploadId}/bulkUploadFiles")]
    public async Task<ActionResult> Post(int bulkUploadId, [FromBody] List<BulkUploadFile> bulkUploadFiles)
    {
        foreach (var file in bulkUploadFiles)
        {
            BackgroundJob.Enqueue<HangfireJobs>(x => x.CreateMissingFolder(file.BulkUploadId, file.Id));
        }

        return Ok();
    }

    [HttpDelete]
    [Route("api/bulkUploads/{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var bulkUpload = await DataContext.BulkUploads.FirstOrDefaultAsync(x => x.Id == id);

        if (bulkUpload == null) return NotFound();

        DataContext.BulkUploads.Remove(bulkUpload);
        await DataContext.SaveChangesAsync();
        return Ok();
    }
}