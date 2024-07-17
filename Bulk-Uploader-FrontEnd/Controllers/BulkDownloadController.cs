using Ac.Net.Authentication;
using Ac.Net.Authentication.Models;
using ApsSettings.Data;
using ApsSettings.Data.Models;
using Bulk_Uploader_Electron.Flows;
using Bulk_Uploader_Electron.Jobs;
using Ganss.Excel;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bulk_Uploader_Electron.Controllers;

public class BulkDownloadController : ControllerBase
{
    private readonly DataContext _dataContext;

    public BulkDownloadController(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    [HttpGet]
    [Route("api/bulkDownloads")]
    public async Task<ActionResult> Get()
    {
        var bulkDownloads = await _dataContext.BulkDownloads.Include(x => x.Files).ToListAsync();
        bulkDownloads.ForEach(bulkDownload =>
        {
            bulkDownload.PendingDownloadCount = bulkDownload.Files
                .Where(x => x.Status == DownloadFileStatus.Pending)
                .LongCount();
            bulkDownload.InProgressDownloadCount = bulkDownload.Files
                .Where(x => x.Status == DownloadFileStatus.InProgress)
                .LongCount();
            bulkDownload.SuccessDownloadCount = bulkDownload.Files
                .Where(x => x.Status == DownloadFileStatus.Success)
                .LongCount();
            bulkDownload.FailedDownloadCount = bulkDownload.Files
                .Where(x => x.Status == DownloadFileStatus.Failed)
                .LongCount();
            bulkDownload.Files = new List<BulkDownloadFile>();

            // Status based on file statuses ** Not Mapped **
            bulkDownload.Status = GetBulkDownloadStatus(bulkDownload);
        });
        return Ok(bulkDownloads);
    }

    [HttpGet]
    [Route("api/downloads/{id}")]
    public async Task<ActionResult> Get(int id)
    {
        var bulkDownload = await _dataContext.BulkDownloads
            .Include(x=> x.Files)
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync();
        if (bulkDownload == null) return NotFound();
        
        
        var fileStatuses = await _dataContext.BulkDownloadFiles
            .Where(x => x.BulkDownloadId == bulkDownload.Id)
            .Select(x=>x.Status)
            .ToListAsync();
        
        bulkDownload.SuccessDownloadCount = fileStatuses
            .Where(x => x == DownloadFileStatus.Success)
            .LongCount();
        bulkDownload.InProgressDownloadCount = fileStatuses
            .Where(x => x == DownloadFileStatus.InProgress)
            .LongCount();
        bulkDownload.PendingDownloadCount = fileStatuses
            .Where(x => x == DownloadFileStatus.Pending)
            .LongCount();
        bulkDownload.FailedDownloadCount = fileStatuses
            .Where(x => x == DownloadFileStatus.Failed)
            .LongCount();
        
        // Status based on file statuses ** Not Mapped **
        bulkDownload.Status = GetBulkDownloadStatus(bulkDownload);
        
        return Ok(bulkDownload);
    }
    
    [HttpGet]
    [Route("api/downloads/{id}/status/{status}")]
    public async Task<ActionResult> Get(int id, DownloadFileStatus status, [FromQuery] int count = 1000, [FromQuery] int skip = 0)
    {
        var bulkDownload = await _dataContext.BulkDownloads
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync();

        if (bulkDownload == null) return NotFound();

        var files = await _dataContext.BulkDownloadFiles
            .Where(x => x.BulkDownloadId == bulkDownload.Id)
            .Where(x => x.Status == status)
            .Skip(skip)
            .Take(count)
            .ToListAsync();
        
        return Ok(files);
    }

    [HttpPost]
    [Route("/api/downloads/bulk")]
    public async Task<ActionResult> BulkDownload([FromBody] List<AccountsProjectsMapping> data,
        [FromQuery] string downloadFolder)
    {
        try
        {
            foreach (var entry in data)
            {
                var job = new BulkDownload()
                {
                    Name = $"{entry.AccountName}/{entry.ProjectName} Download",
                    CloudPath = $"{entry.AccountId}/{entry.ProjectName}",
                    LocalPath = downloadFolder,
                    HubId = entry.AccountId,
                    ProjectId = entry.ProjectId,
                    ApsFolderId = ""
                };
                job.AddLogs("Bulk download job created");
                _dataContext.BulkDownloads.Add(job);
                await _dataContext.SaveChangesAsync();

                BackgroundJob.Enqueue<DownloaderHangfireJobs>(x => 
                    x.StartBulkDownload(job.Id));
            }

            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest("Unable to determine file");
        }
    }

    [HttpPost]
    [Route("/api/bulkDownloads/{bulkDownloadId}/bulkDownloadFiles")]
    public async Task<ActionResult> BulkDownload(string bulkDownloadId, [FromBody] List<BulkDownloadFile> data)
    {
        try
        {
            foreach (var entry in data)
            {
                entry.Status = DownloadFileStatus.Pending;
                BackgroundJob.Enqueue<DownloaderHangfireJobs>(x => 
                    x.ProcessFileDownload(entry.Id));
            }
            
            _dataContext.BulkDownloadFiles.UpdateRange(data);
            await _dataContext.SaveChangesAsync();

            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest("Unable to determine file");
        }
    }

    [HttpPost]
    [Route("api/download")]
    public async Task<ActionResult> CreatDownloadJobs([FromBody] List<BulkDownload> bulkDownloads)
    {
        foreach (var job in bulkDownloads)
        {
            job.AddLogs("Bulk download job created");
            _dataContext.BulkDownloads.Add(job);
            await _dataContext.SaveChangesAsync();

            BackgroundJob.Enqueue<DownloaderHangfireJobs>(x =>
                x.StartBulkDownload(job.Id));
        }

        return Ok();
    }

    [HttpDelete]
    [Route("api/download")]
    public async Task<ActionResult> Delete(int id)
    {
        var downloadJob = await _dataContext.BulkDownloads.FindAsync(id);
        if (downloadJob == null) return NotFound();

        _dataContext.Remove(downloadJob);
        await _dataContext.SaveChangesAsync();
        return Ok();
    }

    //todo: edit this to use the new BulkDownloadFile.InProgress new flag
    public BulkDownload.BulkDownloadStatus GetBulkDownloadStatus(BulkDownload bulkDownload)
    { 
        // Status based on file statuses ** Not Mapped **
        // Pending is somewhat an assumption. If there are no files to download it'll always show pending status
        if ((bulkDownload.PendingDownloadCount != 0 && bulkDownload.SuccessDownloadCount == 0 &&
          bulkDownload.FailedDownloadCount == 0) || (bulkDownload.PendingDownloadCount == 0 && 
          bulkDownload.SuccessDownloadCount == 0 && bulkDownload.FailedDownloadCount == 0))
           return ApsSettings.Data.Models.BulkDownload.BulkDownloadStatus.Pending;
        if (bulkDownload.PendingDownloadCount != 0 &&
                 (bulkDownload.SuccessDownloadCount > 0 || bulkDownload.FailedDownloadCount > 0))
           return ApsSettings.Data.Models.BulkDownload.BulkDownloadStatus.InProgress;
        if (bulkDownload.PendingDownloadCount == 0)
            return ApsSettings.Data.Models.BulkDownload.BulkDownloadStatus.Complete;

        return ApsSettings.Data.Models.BulkDownload.BulkDownloadStatus.Unknown;
    }
    
}