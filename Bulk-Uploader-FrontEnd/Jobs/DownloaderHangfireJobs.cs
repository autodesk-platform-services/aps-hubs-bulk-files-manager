using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using Ac.Net.Authentication;
using ApsSettings.Data;
using ApsSettings.Data.Models;
using Bulk_Uploader_Electron.Controllers;
using Bulk_Uploader_Electron.Flows;
using Bulk_Uploader_Electron.Helpers;
using Bulk_Uploader_Electron.Models;
using Bulk_Uploader_Electron.Utilities;
using Flurl.Http;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using NPOI.HSSF.Record;
using Serilog;

namespace Bulk_Uploader_Electron.Jobs;

public class DownloaderHangfireJobs
{
    private readonly DataContext _dataContext;

    public DownloaderHangfireJobs(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    [Queue("default")]
    public async Task StartBulkDownload(int bulkJobId)
    {
        var job = await _dataContext.BulkDownloads.FindAsync(bulkJobId);

        if (job == null) 
            return;
        
        job.AddLogs("Gathering requirements for download.");
        await _dataContext.SaveChangesAsync();

        if (string.IsNullOrWhiteSpace(job.ApsFolderId))
        {
            job.AddLogs("APS Folder Id Was Not Provided.");
            await _dataContext.SaveChangesAsync();
            
            // goes out and gets the project files folder OR GROUP- folder if A360, then calls the recurse function like below.
            await UtilityFlows.GetProjectData(job.ProjectId, job.HubId, job.Id, job.LocalPath);
        }
        else
        {
            job.AddLogs("APS Folder Id Was Provided.");
            await _dataContext.SaveChangesAsync();
            var token = await JointTokenManager.GetToken();
            var folder = await APSHelpers.GetRootFolder(token, job.HubId, job.ProjectId, job.ApsFolderId);
            await UtilityFlows.RecurseProject(folder, folder.Name, job.ProjectId, job.Id, job.LocalPath);
        }
    }
    
    //TODO: Work out a better solution to passing data, rather than passing the whole list of folders and files
    [Queue("create-local-mirror")]
    public async Task CreateLocalMirror(int BulkDownloadJobId, List<SimpleFolder> folders, List<SimpleFile> files, string downloadPath)
    {
        try
        {
            // mirror the folder structure from ACC, B360, etc.
            var uniqueFolders = folders.Select(x => x.Path).Distinct().ToList();
            foreach (var path in uniqueFolders)
            {
                var folderPath = Path.Combine(downloadPath, path);
                Directory.CreateDirectory(folderPath);
            }

            var downloadJob = await _dataContext.BulkDownloads.FindAsync(BulkDownloadJobId);
            downloadJob?.AddLogs("Creating local mirror.");
            downloadJob?.AddLogs($"{files.Count} files to download.");
            foreach (var file in files)
            {
                var downloadFile = new BulkDownloadFile()
                {
                    BulkDownloadId = BulkDownloadJobId,
                    DestinationFilePath = Path.Combine(downloadPath, file.Path),
                    FileName = file.Name,
                    ItemId = file.ItemId,
                    SourceFilePath = file.ParentPath,
                    FileSize = file.Size,
                    ObjectId = file.ObjectId,
                };
                downloadJob?.Files.Add(downloadFile);
            }
            await _dataContext.SaveChangesAsync();
            
            downloadJob?.Files.ForEach(file =>
            {
                BackgroundJob.Enqueue<DownloaderHangfireJobs>(y => y.ProcessFileDownload(file.Id));
            });
        }
        catch (Exception ex)
        {
            Log.Error("A problem occured while creating a local mirror.");
            Log.Error(ex.Message);
        }
    }

    [Queue("process-file-download")]
    public async Task ProcessFileDownload(int fileId)
    {
        try
        {
            var file = await _dataContext.BulkDownloadFiles.FindAsync(fileId);

            if (file == null)
                return;

            file.Status = DownloadFileStatus.InProgress;
            await _dataContext.SaveChangesAsync();

            var token = await JointTokenManager.GetToken();
            Regex rg = new Regex("^urn:adsk\\.objects:os\\.object:([-_.a-z0-9]{3,128})\\/(.+)$");
            var matches = rg.Matches(file.ObjectId);
            string bucketKey = matches[0].Groups[1].Value;
            string objectName = matches[0].Groups[2].Value;
            var downloadUrl = await APSHelpers.GetDownloadUrl(token, bucketKey, objectName);

            await downloadUrl.DownloadFileAsync(Path.GetDirectoryName(file.DestinationFilePath), file.FileName);
            // using (WebClient webClient = new WebClient())
            // {
            //     // Download the Web resource and save it into the destination
            //     webClient.DownloadFile(downloadUrl, file.DestinationFilePath);
            // }
            file.Status = DownloadFileStatus.Success;
            file.AddLogs($"File downloaded successfully.");
            await _dataContext.SaveChangesAsync();

        }
        catch (Exception ex)
        {
            var bulkDownloadFile = await _dataContext.BulkDownloadFiles.FindAsync(fileId);
            bulkDownloadFile.Status = DownloadFileStatus.Failed;
            bulkDownloadFile.AddLogs($"File downloaded failed.");
            bulkDownloadFile.AddLogs($"{ex.Message}");
            await _dataContext.SaveChangesAsync();
            Log.Error($"A problem occured while downloading {bulkDownloadFile.FileName}");
            Log.Error(ex.Message);
        }
    }
}