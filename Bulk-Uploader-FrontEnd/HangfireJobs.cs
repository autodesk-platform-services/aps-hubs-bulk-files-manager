using ApsSettings.Data;
using ApsSettings.Data.Models;
using Bulk_Uploader_Electron.Managers;
using Bulk_Uploader_Electron.Utilities;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using RestSharp;
using Serilog;
using System.Net;
using Ac.Net.Authentication;
using Bulk_Uploader_Electron.Helpers;
using Flurl.Http;

namespace mass_upload_via_s3_csharp;

public class FileDTU
{
    public bool Equals(FileDTU file)
    {
        if (file == null)
            return false;

        if (this.Name == file.Name && this.ParentFolderUrn == file.ParentFolderUrn &&
            this.ParentFolderUrn == file.ParentFolderUrn && this.ItemId == file.ItemId)
            return true;
        else
            return false;
    }

    public override bool Equals(Object obj)
    {
        if (obj == null)
            return false;

        FileDTU personObj = obj as FileDTU;
        if (personObj == null)
            return false;
        else
            return Equals(personObj);
    }


    public string ProjectId { get; set; }
    public string ParentFolderUrn { get; set; }
    public string LocalParentPath { get; set; }
    public string Name { get; set; }
    public string ObjectId { get; set; }
    public string ItemId { get; set; }
    public string UploadKey { get; set; }
}

public class HangfireJobs
{
    public readonly DataContext _context;

    public HangfireJobs(DataContext dataContext)
    {
        _context = dataContext;
    }

    private static DateTime lastActivityTime = DateTime.Now;
    private static int fileNo = 1;
    private static List<FileDTU> filesProcessed = new List<FileDTU>();

    [Queue("build-autodesk-mirror")]
    public async Task<List<BulkUploadAutodeskMirror>> RetrieveNewMirrorFolders(int bulkUploadId, int mirrorFolderId)
    {
        var bulkUpload = await _context.BulkUploads.FirstOrDefaultAsync(x => x.Id == bulkUploadId);
        var bulkUploadMirror =
            await _context.BulkUploadAutodeskMirrors.FirstOrDefaultAsync(x => x.Id == mirrorFolderId);

        var token = await JointTokenManager.GetToken();
        // var token = await ThreeLeggedMananger.Instance.GetToken();
        var (folders, files) =
            await APSHelpers.GetFolderContents(token, bulkUpload.ProjectId, bulkUploadMirror.FolderUrn);

        var returnFolders = new List<BulkUploadAutodeskMirror>();
        if (files.Count > 0)
        {
            var newBulkUploadMirrorFiles = files
                .Select(file => new BulkUploadAutodeskMirrorFile()
                {
                    FolderUrn = bulkUploadMirror.FolderUrn,
                    Name = file.Name.ToLower(),
                    ItemId = file.ItemId,
                    BulkUploadId = bulkUpload.Id
                })
                .ToList();

            await _context.BulkUploadAutodeskMirrorFiles.AddRangeAsync(newBulkUploadMirrorFiles);
        }

        if (folders.Count > 0)
        {
            var existingFolders = await _context.BulkUploadAutodeskMirrors
                .Where(x => x.BulkUploadId == bulkUploadId)
                .Select(x => x.RelativeFolderPath)
                .ToListAsync();
            
            //Create Mirror records
            returnFolders = folders
                .Select(folder =>
                {
                    return new BulkUploadAutodeskMirror()
                    {
                        BulkUploadId = bulkUpload.Id,
                        FolderName = folder.Name,
                        FolderUrn = folder.FolderId,
                        FolderUrl = folder.Url,
                        RelativeFolderPath = Path.Combine(bulkUploadMirror.RelativeFolderPath, folder.Name),
                        ContentsRetrieved = false
                    };
                })
                .Where(x=>existingFolders.Contains(x.RelativeFolderPath) == false)
                .ToList();

            await _context.BulkUploadAutodeskMirrors.AddRangeAsync(returnFolders);
        }

        bulkUploadMirror.ContentsRetrieved = true;
        _context.BulkUploadAutodeskMirrors.Update(bulkUploadMirror);
        await _context.SaveChangesAsync();

        return returnFolders;
    }

    [Queue("build-autodesk-mirror")]
    public async Task BuildAutodeskMirror(int bulkUploadId, int mirrorId)
    {
        //Get items from database
        var bulkUploadMirror =
            await _context.BulkUploadAutodeskMirrors.FirstOrDefaultAsync(x => x.Id == mirrorId);
        if (bulkUploadMirror == null) return;
        var bulkUpload = await _context.BulkUploads.FirstOrDefaultAsync(x => x.Id == bulkUploadMirror.BulkUploadId);
        if (bulkUpload == null) return;

        var newBulkUploadMirrors = await RetrieveNewMirrorFolders(bulkUploadId, mirrorId);

        foreach (var mirror in newBulkUploadMirrors)
        {
            BackgroundJob.Enqueue<HangfireJobs>(x => x.BuildAutodeskMirror(bulkUploadId, mirror.Id));
        }

        //Check if there is are any folders that have not been processed
        var remainingJobsCount = await _context.BulkUploadAutodeskMirrors
            .Where(x => x.BulkUploadId == bulkUploadId)
            .CountAsync(x => x.ContentsRetrieved == false);

        //If all folders have been processed, kick off the folder parsing
        if (remainingJobsCount == 0)
        {
            bulkUpload.AddLogs("Finished creating Autodesk Cloud Folder Mirror");
            _context.BulkUploads.Update(bulkUpload);
            await _context.SaveChangesAsync();

            BackgroundJob.Enqueue<HangfireJobs>(x => x.ProcessLocalFolder(
                bulkUpload.Id,
                bulkUpload.LocalPath
            ));
        }
    }

    [Queue("process-local-folder")]
    public async Task ProcessLocalFolder(int bulkUploadId, string folderPath)
    {
        //Retrieve mirrors and bulkUpload from database, to assist in creating files
        var bulkUpload = await _context.BulkUploads
            .Include(x => x.AutodeskMirrors)
            .FirstOrDefaultAsync(x => x.Id == bulkUploadId);
        if (bulkUpload == null) return;

        //Retrieve all local folder contents
        var allFolders = Directory.GetDirectories(folderPath).ToList();
        var allFiles = Directory.GetFiles(folderPath).ToList();


        //For each folder, run a new 'Process LocalFolder' job
        foreach (var folder in allFolders)
        {
            BackgroundJob.Enqueue<HangfireJobs>(x => x.ProcessLocalFolder(bulkUploadId, folder));
        }

        //For each file, run a new 'Process LocalFile' job
        var bulkUploadFiles = allFiles.Select(filePath => BulkUploadFile.CreateFile(
            bulkUpload,
            Path.GetDirectoryName(filePath.Replace(bulkUpload.LocalPath, "")),
            filePath
        )).ToList();
        await _context.BulkUploadFiles.AddRangeAsync(bulkUploadFiles);
        await _context.SaveChangesAsync();


        foreach (var file in bulkUploadFiles)
        {
            //if (file.Status != JobFileStatus.DoNotUpload && file.Status != JobFileStatus.Failed &&
            //    file.Status != JobFileStatus.Proposed)
            if (file.Status != JobFileStatus.DoNotUpload && file.Status != JobFileStatus.Failed)
            {
                if (string.IsNullOrWhiteSpace(file.FolderUrn))
                {
                    BackgroundJob.Enqueue<HangfireJobs>(x => x.CreateMissingFolder(file.BulkUploadId, file.Id));
                }
                else
                {
                    BackgroundJob.Enqueue<HangfireJobs>(x => x.ProcessFile(file.BulkUploadId, file.Id));
                }
            }
        }
    }

    private List<string> GetAllDirectories(string relativePath)
    {
        var splitPaths = relativePath.Split("\\").ToList();
        var directories = new List<string>();
        var currentFolder = "";

        foreach (string path in splitPaths)
        {
            currentFolder = Path.Combine(currentFolder, path);
            if (string.IsNullOrWhiteSpace(currentFolder)) currentFolder = "\\";
            directories.Add(currentFolder);
        }

        return directories;
    }

    [Queue("create-cloud-folders")]
    public async Task CreateMissingFolder(int bulkUploadId, int bulkUploadFileId)
    {
        //Retrieve the bulk upload and file
        var bulkUpload = await _context.BulkUploads.FirstOrDefaultAsync(x => x.Id == bulkUploadId);
        var bulkUploadFile =
            await _context.BulkUploadFiles.FirstOrDefaultAsync(x => x.Id == bulkUploadFileId);

        if (bulkUpload == null || bulkUploadFile == null || bulkUpload.Id != bulkUploadFile.BulkUploadId)
        {
            return;
        }


        //Find the closest parent that exists in the mirror
        var paths = GetAllDirectories(bulkUploadFile.TargetRelativePath);
        BulkUploadAutodeskMirror lastMirror = null;
        foreach (var path in paths)
        {
            var matchingMirror = await _context.BulkUploadAutodeskMirrors
                .FirstOrDefaultAsync(x => x.BulkUploadId == bulkUploadId && x.RelativeFolderPath == path);

            if (matchingMirror == null)
            {
                if (lastMirror == null) return;
                var folderName = path.Split("\\").LastOrDefault() ?? "";

                try
                {
                    //Create Folder
                    var folder = await APSHelpers.CreateFolder(bulkUpload.ProjectId, lastMirror.FolderUrn, folderName);
                    bulkUploadFile.AddLogs($"Created folder: {path}");
                    var newMirrorFolder = new BulkUploadAutodeskMirror()
                    {
                        BulkUploadId = bulkUploadId,
                        ContentsRetrieved = true,
                        FolderName = folder.Data.Attributes.Name,
                        FolderUrn = folder.Data.Id,
                        FolderUrl = folder.Links.Self.Href, // Issue with the new SDK: missing -> folder.Links?.WebView?.Href,
                        RelativeFolderPath = path
                    };

                    _context.BulkUploadAutodeskMirrors.Add(newMirrorFolder);
                    await _context.SaveChangesAsync();
                    lastMirror = newMirrorFolder;
                }
                catch (FlurlHttpException e)
                {
                    if (e.StatusCode == 409)
                    {
                        //TODO: Check APS to see if the folder has been created. If so, add it to the mirror and restart the job
                        await RetrieveNewMirrorFolders(bulkUploadId, lastMirror.Id);
                    }

                    throw;
                }
            }
            else
            {
                lastMirror = matchingMirror;
            }
        }

        if (lastMirror == null)
        {
            bulkUploadFile.AddLogs("Could not find a matching folder in the folder paths");
            bulkUploadFile.Status = JobFileStatus.Failed;
        }
        else
        {
            bulkUploadFile.AddLogs("Added folder URN");
            bulkUploadFile.FolderUrn = lastMirror.FolderUrn;
            bulkUploadFile.FolderUrl = lastMirror.FolderUrl;
        }

        _context.BulkUploadFiles.Update(bulkUploadFile);
        await _context.SaveChangesAsync();

        //Run processFile if a folder URN has been created
        if (!string.IsNullOrWhiteSpace(bulkUploadFile.FolderUrn))
        {
            BackgroundJob.Enqueue<HangfireJobs>(x => x.ProcessFile(bulkUploadFile.BulkUploadId, bulkUploadFile.Id));
        }
    }

    [Queue("process-file")]
    public async Task CheckForUpdatedFiles(int bulkUploadId, int bulkUploadFileId)
    {
        //Retrieve the bulk upload and file
        var bulkUpload = await _context.BulkUploads.FirstOrDefaultAsync(x => x.Id == bulkUploadId);
        var bulkUploadFile =
            await _context.BulkUploadFiles.FirstOrDefaultAsync(x => x.Id == bulkUploadFileId);

        if (bulkUpload == null || bulkUploadFile == null || bulkUpload.Id != bulkUploadFile.BulkUploadId) return;

        //Get folder contents, and update file mirrors if necessary
        var token = await JointTokenManager.GetToken();
        // var token = await ThreeLeggedMananger.Instance.GetToken();
        var (folders, files) =
            await APSHelpers.GetFolderContents(token, bulkUpload.ProjectId, bulkUploadFile.FolderUrn);


        var existingFiles = await _context.BulkUploadAutodeskMirrorFiles
            .Where(x => x.BulkUploadId == bulkUploadId)
            .Where(x => x.FolderUrn == bulkUploadFile.FolderUrn)
            .Select(x => x.Name)
            .ToListAsync();

        var newFiles = files
            .Where(x => !existingFiles.Contains(x.Name))
            .Select(x => new BulkUploadAutodeskMirrorFile()
            {
                BulkUploadId = bulkUploadId,
                FolderUrn = bulkUploadFile.FolderUrn,
                ItemId = x.ItemId,
                Name = x.Name
            });

        await _context.BulkUploadAutodeskMirrorFiles.AddRangeAsync(newFiles);
        await _context.SaveChangesAsync();

        //Process file
        BackgroundJob.Enqueue<HangfireJobs>(x => x.ProcessFile(bulkUploadId, bulkUploadFileId));
    }

    [Queue("process-file")]
    public async Task ProcessFile(int bulkUploadId, int bulkUploadFileId)
    {
        var bulkUpload = await _context.BulkUploads.FirstOrDefaultAsync(x => x.Id == bulkUploadId);
        var bulkUploadFile =
            await _context.BulkUploadFiles.FirstOrDefaultAsync(x => x.Id == bulkUploadFileId);

        if (bulkUpload == null || bulkUploadFile == null || bulkUpload.Id != bulkUploadFile.BulkUploadId)
        {
            return;
        }

        var existingFile = await _context.BulkUploadAutodeskMirrorFiles
            .Where(x => x.BulkUploadId == bulkUploadId)
            .Where(x => x.FolderUrn == bulkUploadFile.FolderUrn)
            .Where(x => x.Name == bulkUploadFile.TargetFileName.ToLower())
            .FirstOrDefaultAsync();

        if (existingFile != null)
        {
            bulkUploadFile.ItemId = existingFile.ItemId;
            _context.BulkUploadFiles.Update(bulkUploadFile);
            await _context.SaveChangesAsync();
        }

        if (string.IsNullOrWhiteSpace(bulkUploadFile.FolderUrn))
        {
            await UpdateFileStatus(bulkUploadFileId, JobFileStatus.Failed, "Folder not created yet");
        }

        try
        {
            //Create storage location
            var storageLocation = await APSHelpers.CreateStorageLocation(bulkUpload.ProjectId, bulkUploadFile.TargetFileName, bulkUpload.FolderId);
            bulkUploadFile.ObjectId = storageLocation.Data.Id.Replace("urn:adsk.objects:os.object:wip.dm.prod/", "");
            _context.BulkUploadFiles.Update(bulkUploadFile);
            await _context.SaveChangesAsync();

            // Upload file to storage location
            long UPLOAD_CHUNK_SIZE = 20 * 1024 * 1024;
            FileStream fileStream = new FileStream(bulkUploadFile.SourceAbsolutePath, FileMode.Open,
                FileAccess.Read);
            long fileSize = fileStream.Length;
            int numberOfChunks = (int) Math.Round((double) (fileSize / UPLOAD_CHUNK_SIZE)) + 1;
            long partsUploaded = 0;
            long start = 0;

            List<string> uploadUrls = new List<string>();
            string uploadKey = null;
            int maxBatches = 25;

            Console.WriteLine($"Number of file chunks to upload: {numberOfChunks}");

            using (BinaryReader reader = new BinaryReader(fileStream))
            {
                while (partsUploaded < numberOfChunks)
                {
                    int attempts = 0;

                    long chunkWithParts = (long) ((partsUploaded + 1) * UPLOAD_CHUNK_SIZE);
                    long end = Math.Min(chunkWithParts, fileSize);

                    int numberOfBytes = (int) (end - start);

                    byte[] fileBytes = new byte[numberOfBytes];
                    reader.BaseStream.Seek(start, SeekOrigin.Begin);
                    int count = reader.Read(fileBytes, 0, numberOfBytes);

                    while (true)
                    {
                        attempts++;
                        if (uploadUrls.Count == 0)
                        {
                            //var token = await TokenManager.GetTwoLeggedToken();
                            var part = Math.Min(numberOfChunks - partsUploaded, maxBatches);
                            int partInt = Convert.ToInt32(part);

                            int partsUploadInt = Convert.ToInt32(partsUploaded) + 1;
                            var uploadParams = await APSHelpers.GetUploadUrls("wip.dm.prod", bulkUploadFile.ObjectId, 60, partInt, partsUploadInt, uploadKey);
                            uploadKey = uploadParams.UploadKey;
                            uploadUrls = uploadParams.Urls;
                        }

                        string currentUrl = uploadUrls[0];
                        uploadUrls.RemoveAt(0);

                        try
                        {
                            var responseBuffer = await UploadBufferRestSharp(currentUrl, fileBytes);

                            int statusCode = (int) responseBuffer.StatusCode;

                            switch (statusCode)
                            {
                                case 403:
                                    //Console.WriteLine("403, refreshing urls");
                                    uploadUrls = new List<string>();
                                    break;
                                case 503:
                                    break;
                                case int n when (n >= 400 && n != 503):
                                    throw new Exception(responseBuffer.Content);
                                default:
                                    goto NextChunk;
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Problem with chunking upload");
                            Console.WriteLine(ex.Message);
                            Log.Error("Problem with chunking upload");
                            Log.Error(ex.Message);
                            if (attempts == 5)
                                throw;
                        }
                    }

                    NextChunk:
                    partsUploaded++;
                    Console.WriteLine(
                        $"{partsUploaded.ToString()} of {numberOfChunks.ToString()} parts uploaded for {bulkUploadFile.SourceAbsolutePath} ");
                    start = end;
                }
            }

            _ = await APSHelpers.CompleteUpload("wip.dm.prod", bulkUploadFile.ObjectId, uploadKey);

            //Create Version or Update Version
            if (string.IsNullOrWhiteSpace(bulkUploadFile.ItemId))
            {
                try
                {
                    var version = await APSHelpers.CreateFirstVersion(bulkUpload.ProjectId,
                        bulkUploadFile.TargetFileName, bulkUploadFile.FolderUrn, bulkUploadFile.ObjectId);
                    bulkUploadFile.VersionId = version.Data.Id;
                    bulkUploadFile.WebUrl = version.Links.Self.Href;  // Issue with the new SDK: missing -> version.Link.WebView.Href
                }
                catch (FlurlHttpException e)
                {
                    if (e.StatusCode == 409)
                    {
                        BackgroundJob.Enqueue<HangfireJobs>(x =>
                            x.CheckForUpdatedFiles(bulkUploadId, bulkUploadFileId));
                        return;
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            else
            {
                var version = await APSHelpers.CreateNextVersion(bulkUpload.ProjectId, bulkUploadFile.TargetFileName,
                    bulkUploadFile.ItemId, bulkUploadFile.ObjectId);
                bulkUploadFile.VersionId = version.Data.Id;
                bulkUploadFile.WebUrl = version.Links.Self.Href;  // Issue with the new SDK: missing -> version.Link.WebView.Href
            }

            _context.BulkUploadFiles.Update(bulkUploadFile);
            await _context.SaveChangesAsync();

            await UpdateFileStatus(bulkUploadFileId, JobFileStatus.Success);

            //TODO: Build an event queue to pass messages to the UI and refresh any currently active views
        }
        catch (Exception e)
        {
            await UpdateFileStatus(bulkUploadFileId, JobFileStatus.Failed, e.Message);
            Log.Error("Problem uploading file: " + bulkUploadFile.SourceAbsolutePath);
            Log.Error("Reason: " + e.Message);
            Log.Information("Stack: " + e.StackTrace);
            throw;
        }
    }

    [Queue("kill-finished")]
    public async Task KillOnceFinished(int dbJobId)
    {
        Task.Delay(2000);
        var api = JobStorage.Current.GetMonitoringApi(); // Determine if we are the last job standing
        while (api.ProcessingCount() > 1)
        {
            Task.Delay(500);
        }

        Log.Information("File Upload Process Completed");
        // var _context = new DataContextFactory().CreateDbContext();
        var job = await _context.BulkUploads.FindAsync(dbJobId);
        if (job == null)
            return;
        job.EndTime = DateTime.Now;
        _context.SaveChanges();
        //Process.Start(SerilogFileResults.LogFilePath);

        //Environment.Exit(0);
    }

    /// <summary>
    /// Upload the specific part through url
    /// </summary>
    /// <param name="url">URL to upload the specified part</param>
    /// <param name="buffer">Buffer array to upload</param>
    public static async Task<dynamic> UploadBufferRestSharp(string url, byte[] buffer)
    {
        RestResponse response = null;

        try
        {
            RestClient client = new RestClient();
            RestRequest request = new RestRequest(url, RestSharp.Method.Put);
            //request.AddParameter("", buffer, ParameterType.RequestBody);
            request.AddParameter("application/octet-stream", buffer, ParameterType.RequestBody);

            response = await client.ExecuteAsync(request);
        }
        catch (Exception e)
        {
        }

        // See if we are getting a "Slow Down" message aka 503 Service Unavailable

        if (response != null)
        {
            if (response.StatusCode == HttpStatusCode.ServiceUnavailable)
            {
                Task.Delay(5000);
                UploadBufferRestSharp(url, buffer);
            }
        }

        return response;
    }

    public async Task UpdateFileStatus(int dbFileId, JobFileStatus status, string notes = "")
    {
        try
        {
            var dbFile = await _context.BulkUploadFiles.FindAsync(dbFileId);
            if (dbFile == null)
                return;
            dbFile.Status = status;
            dbFile.LastModified = DateTime.Now;
            if (!string.IsNullOrWhiteSpace(notes)) dbFile.AddLogs(notes);
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Problem updating file status in db for file ID : {dbFileId}");
            Log.Error("Reason: " + e.Message);
            Log.Information("Stack: " + e.StackTrace);

            Console.WriteLine("Reason: " + e.Message);
            Console.WriteLine("stack: " + e.StackTrace);
        }
    }
}