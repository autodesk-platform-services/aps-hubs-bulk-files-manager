using ApsSettings.Data;
using ApsSettings.Data.Models;
using Bulk_Uploader_Electron.Helpers;
using Bulk_Uploader_Electron.Jobs;
using Bulk_Uploader_Electron.Models;
using Bulk_Uploader_Electron.Utilities;
using Ganss.Excel;
using Hangfire;
using System.Diagnostics;
using System.Reflection;
using Project = Bulk_Uploader_Electron.Models.Project;

namespace Bulk_Uploader_Electron.Flows
{
    public class UtilityFlows
    {
        public static async Task SaveAsExcel(dynamic content, string outputPath, string sheetName)
        {
            if (!string.IsNullOrWhiteSpace(outputPath))
            {
                var outdir = Path.GetDirectoryName(outputPath);
                if (!string.IsNullOrWhiteSpace(outdir))
                {
                    using (var stream = File.Create(outputPath))
                    {
                        var mapper = new ExcelMapper();
                        await mapper.SaveAsync(stream, content, sheetName);
                    }
                }
            }
        }

        public static async Task<List<Account>> GetHubs(string? filePath)
        {
            var results = new List<Account>();
            try
            {
                var errors = new List<ErrorMessage>();

                try
                {
                    var token = await JointTokenManager.GetToken();
                    results = await APSHelpers.GetHubs(token);
                }
                catch (Exception ex)
                {
                    errors.Add(new ErrorMessage("Error exporting accounts", ex.Message, ""));
                }

                if (!string.IsNullOrWhiteSpace(filePath))
                {
                    var outDir = Path.GetDirectoryName(filePath);

                    if (!string.IsNullOrWhiteSpace(outDir))
                    {
                        if (!Directory.Exists(outDir))
                        {
                            Directory.CreateDirectory(outDir);
                        }

                        using (var stream = File.Create(filePath))
                        {
                            new ExcelMapper().Save(stream, results, "Hubs");
                        }
                    }
                }

                return results;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{MethodBase.GetCurrentMethod()?.Name ?? "Unknown"}: {ex.Message}");
                throw;
            }
        }

        public static async Task<List<Project>> GetProjects(string hubId, string? filePath)
        {
            var accountProjects = new List<Project>();
            try
            {
                var errors = new List<ErrorMessage>();
                try
                {
                    var token = await JointTokenManager.GetToken();
                    accountProjects = await APSHelpers.GetHubProjects(hubId, token, errors: errors);
                }
                catch (Exception ex)
                {
                    errors.Add(new ErrorMessage("Error exporting projects", ex.Message, ""));
                }

                if (!string.IsNullOrWhiteSpace(filePath))
                {
                    var outDir = Path.GetDirectoryName(filePath);
                    if (!string.IsNullOrWhiteSpace(outDir))
                    {
                        using var stream = File.Create(filePath);
                        new ExcelMapper().Save(stream, accountProjects, "Projects");
                    }
                }
                return accountProjects;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{MethodBase.GetCurrentMethod()?.Name ?? "Unknown"}: {ex.Message}");
                throw;
            }
        }

        public static async Task<(List<SimpleFolder>, List<SimpleFile>)> GetProjectData(string projectId, string hubId)
        {
            var token = await JointTokenManager.GetToken();
            var project = await APSHelpers.GetHubProject(token, hubId, projectId);
            var projectName = project?.Data?.Attributes?.Name ?? "";
            token = await JointTokenManager.GetToken();
            var topFolders = await APSHelpers.GetTopFolders(token, hubId, projectId);
            var projectFileFolder = topFolders.Select(x => x.Name.ToUpper()).Contains("PROJECT FILES")
                ? topFolders.Where(x => x.Name.ToUpper() == "PROJECT FILES").FirstOrDefault()
                : topFolders.FirstOrDefault();

            projectFileFolder.IsRoot = true;
            projectFileFolder.ParentPath = projectName;
            projectFileFolder.Path = Path.Combine(projectFileFolder.ParentPath, projectFileFolder.Name);

            var (allFolders, allFiles) = await RecurseProject(projectFileFolder, projectFileFolder.Name, projectId);

            allFolders.Add(projectFileFolder);

            return (allFolders, allFiles);
        }

        public static async Task GetProjectData(string projectId, string hubId, int jobId, string downloadPath)
        {
            DataContext tempContext = new DataContextFactory().CreateDbContext();

            var job = await tempContext.BulkDownloads.FindAsync(jobId);

            var token = await JointTokenManager.GetToken();
            var project = await APSHelpers.GetHubProject(token, hubId, projectId);
            var projectName = project?.Data?.Attributes?.Name ?? "";
            token = await JointTokenManager.GetToken();
            var topFolders = await APSHelpers.GetTopFolders(token, hubId, projectId);
            var projectFileFolder = topFolders.Select(x => x.Name.ToUpper()).Contains("PROJECT FILES")
                ? topFolders.Where(x => x.Name.ToUpper() == "PROJECT FILES" || x.Name.ToUpper().StartsWith("GROUP-")).FirstOrDefault()
                : topFolders.FirstOrDefault();

            projectFileFolder.IsRoot = true;
            projectFileFolder.ParentPath = projectName;
            projectFileFolder.Path = Path.Combine(projectFileFolder.ParentPath, projectFileFolder.Name);

            job.ApsFolderId = projectFileFolder.FolderId;
            await tempContext.SaveChangesAsync();
            await tempContext.DisposeAsync();

            await RecurseProject(projectFileFolder, projectFileFolder.Name, projectId, jobId, downloadPath);
        }

        public static async Task<(List<SimpleFolder>, List<SimpleFile>)> RecurseProject(SimpleFolder rootFolder, string rootFolderPath, string projectId)
        {
            var folders = new List<SimpleFolder>();
            var files = new List<SimpleFile>();

            var token = await JointTokenManager.GetToken();
            var (rootFolders, rootFiles) = await APSHelpers.GetFolderContents(token, projectId, rootFolder.FolderId);

            foreach (var file in rootFiles)
            {
                file.ParentPath = rootFolder.Path;
                file.Path = Path.Combine(rootFolder.Path, file.Name);
                files.Add(file);
            }

            foreach (var folder in rootFolders)
            {
                folder.ParentPath = rootFolder.Path;
                folder.Path = Path.Combine(rootFolder.Path, folder.Name);
                folders.Add(folder);
                var (_folders, _files) = await RecurseProject(folder, folder.Path, projectId);
                folders.AddRange(_folders);
                files.AddRange(_files);
            }


            return (folders, files);
        }

        /// <summary>
        /// This version of the method requires an instantiation of the UtilityFlows class since the method
        /// is not static. The method differs from the static method in that it dynamically adds files to the database
        /// as they're found from APS. This allows the user to see the progress of the download in real time (if polling for it).
        /// </summary>
        /// <param name="rootFolder"></param>
        /// <param name="rootFolderPath"></param>
        /// <param name="projectId"></param>
        /// <param name="jobId"></param>
        /// <param name="downloadPath"></param>
        /// <returns></returns>
        public static async Task RecurseProject(SimpleFolder rootFolder, string rootFolderPath, string projectId, int jobId, string downloadPath)
        {
            DataContext tempContext = new DataContextFactory().CreateDbContext();

            var job = await tempContext.BulkDownloads.FindAsync(jobId);

            if (job == null)
                return;

            var token = await JointTokenManager.GetToken();
            var (rootFolders, rootFiles) = await APSHelpers.GetFolderContents(token, projectId, rootFolder.FolderId);

            var downloadFiles = new List<BulkDownloadFile>();

            foreach (var file in rootFiles)
            {
                file.ParentPath = rootFolder.Path;
                file.Path = Path.Combine(rootFolder.Path, file.Name);
                var downloadFile = new BulkDownloadFile()
                {
                    BulkDownloadId = job.Id,
                    DestinationFilePath = Path.Combine(downloadPath, file.Path),
                    FileName = file.Name,
                    ItemId = file.ItemId,
                    SourceFilePath = file.ParentPath,
                    FileSize = file.Size,
                    ObjectId = file.ObjectId,
                    Status = DownloadFileStatus.Pending
                };
                downloadFiles.Add(downloadFile);
            }

            job.Files.AddRange(downloadFiles);
            await tempContext.SaveChangesAsync();

            foreach (var bulkDownloadFile in job.Files)
            {
                BackgroundJob.Enqueue<DownloaderHangfireJobs>(y => y.ProcessFileDownload(bulkDownloadFile.Id));
            }

            await tempContext.DisposeAsync();

            foreach (var folder in rootFolders)
            {
                folder.ParentPath = rootFolder.Path;
                folder.Path = Path.Combine(rootFolder.Path, folder.Name);
                await RecurseProject(folder, folder.Path, projectId, jobId, downloadPath);
            }
        }



        #region OldCode
        // public static async Task<(List<SimpleFolder>, List<SimpleFile>)> GetFolderContent(
        //     ITokenManager tokenManager,
        //     string projectId,
        //     string folderId,
        //     bool foldersOnly,
        //     bool recurse,
        //     BlockingCollection<ErrorMessage>? errors = null)
        // {
        //     try
        //     {
        //         var (parentFolders, parentFiles) = await APSHelpers.GetFolderContents(
        //                     token: await tokenManager.GetToken(),
        //                     projectId: projectId,
        //                     folderId: folderId,
        //                     folderOnly: foldersOnly,
        //                     userId: ""
        //                    );
        //
        //         if (recurse)
        //         {
        //             foreach (var parentFolder in parentFolders)
        //             {
        //                 var (childFolders, childFiles) = await GetFolderContent(
        //                     tokenManager: tokenManager,
        //                     projectId: projectId,
        //                     folderId: parentFolder.FolderId,
        //                     foldersOnly: foldersOnly,
        //                     recurse: recurse,
        //                     errors: errors);
        //
        //                 foreach (var folder in childFolders)
        //                 {
        //                     folder.ParentPath = parentFolder.Path;
        //                     folder.Path = Path.Combine(folder.ParentPath ?? "", folder.Name);
        //                     parentFolders.Add(folder);
        //                 }
        //
        //                 if (!foldersOnly)
        //                 {
        //                     foreach (var file in childFiles)
        //                     {
        //                         file.ParentPath = parentFolder.Path;
        //                         file.Path = Path.Combine(file?.ParentPath ?? "Unknown", file?.Name ?? "Unknown");
        //                         parentFiles.Add(file!);
        //                     }
        //                 }
        //             }
        //         }
        //
        //         return (parentFolders, parentFiles);
        //     }
        //     catch (Exception ex)
        //     {
        //         if (errors != null)
        //             errors.Add(new ErrorMessage($"Get Folder: {folderId}", ex.Message, ex.StackTrace ?? "No Stack"));
        //     }
        //
        //     return (new List<SimpleFolder>(), new List<SimpleFile>());
        // }
        //
        // public class CreateDownloadJobArgs
        // {
        //     public readonly DataContext context;
        //     public readonly ITokenManager tokenManger;
        //
        //     public CreateDownloadJobArgs(DataContext context, ITokenManager tokenManger, string hubId, string projectId, bool includeProjectName, string? filePath)
        //     {
        //         this.tokenManger = tokenManger;
        //         this.context = context;
        //         this.hubId = hubId;
        //         this.projectId = projectId;
        //         this.includeProjectName = includeProjectName;
        //         this.filePath = filePath;
        //     }
        //
        //     public readonly string hubId;
        //     public readonly string projectId;
        //     public readonly bool includeProjectName;
        //     public readonly string? filePath;
        // }
        //
        // //public static async Task<(List<SimpleFolder>, List<SimpleFile>)> CreateProjectDownloadEntites(
        // //    CreateDownloadJobArgs args,
        // //    BlockingCollection<ErrorMessage>? errors = null)
        // //{
        // //    List<SimpleFolder> folderResults = new List<SimpleFolder>();
        // //    List<SimpleFile> fileResults = new List<SimpleFile>();
        //
        // //    try
        // //    {
        // //        (folderResults, fileResults) =
        // //                  await GetProjectContent(
        // //                      tokenManger: args.tokenManger,
        // //                      hubId: args.hubId,
        // //                      projectId: args.projectId,
        // //                      foldersOnly: false,
        // //                      includeProjectName: args.includeProjectName,
        // //                      errors: errors);
        //
        // //        if (!string.IsNullOrWhiteSpace(args.filePath))
        // //        {
        // //            var outDir = Path.GetDirectoryName(args.filePath);
        //
        // //            if (!string.IsNullOrWhiteSpace(outDir))
        // //            {
        // //                ExcelPackage? excelPackage = null;
        // //                var ef = ExcelUtils.CreateWorksheet(folderResults, "Folders");
        // //                var eff = ef.AddWorksheet(fileResults, "Files");
        // //                if (errors != null)
        // //                {
        // //                    var err = eff == null ? ef.AddWorksheet(errors, "Errors") : eff.NextWorksheet(errors, "Errors");
        // //                    excelPackage = err.ToExcelPackage();
        // //                    ExcelUtils.AddStyleToTable(excelPackage!, 0, ExcelUtils.WsColor.BlueWhite);
        // //                }
        // //                var cnt = 0;
        // //                if (eff != null)
        // //                {
        // //                    ExcelUtils.AddStyleToTable(excelPackage!, 1, ExcelUtils.WsColor.GreenWhite);
        // //                    cnt = 1;
        // //                }
        // //                if (errors != null)
        // //                {
        // //                    ExcelUtils.AddStyleToTable(excelPackage!, cnt + 1, ExcelUtils.WsColor.GrayWhite);
        // //                }
        //
        // //                //using (var stream = File.Create(filePath))
        // //                //{
        // //                //    excelPackage!.SaveAs(stream);
        // //                //    stream.Close();
        // //                //}
        // //            }
        // //        }
        //
        // //        //if (fileResults.Count > 0)
        // //        //{
        // //        //    var batchJob = new BatchJob() { Name = $"Download.{projectId}.{(DateTime.Now.ToString("yy.MM.dd.HH.mm"))}" };
        // //        //    context.Batches.Add(batchJob);
        //
        // //        //    foreach (var fr in fileResults)
        // //        //    {
        // //        //        var job = new JobTaskAggregator()
        // //        //        {
        // //        //            JobOwner = batchJob,
        // //        //            Description
        // //        //        }
        // //        //    }
        //
        // //        //}
        //
        // //        return (folderResults, fileResults);
        // //    }
        // //    catch (Exception ex)
        // //    {
        // //        Debug.WriteLine($"{MethodBase.GetCurrentMethod()?.Name ?? "Unknown"}: {ex.Message}");
        // //        throw;
        // //    }
        // //}
        //
        // public static async Task<(List<SimpleFolder>, List<SimpleFile>)> GetProjectContentWithOutput(
        //     ITokenManager tokenManger,
        //     string hubId,
        //     string projectId,
        //     bool foldersOnly,
        //     bool includeProjectName,
        //     string? outPath,
        //     BlockingCollection<ErrorMessage>? errors = null)
        // {
        //     List<SimpleFolder> folderResults = new List<SimpleFolder>();
        //     List<SimpleFile> fileResults = new List<SimpleFile>();
        //
        //     try
        //     {
        //         (folderResults, fileResults) =
        //                   await GetProjectContent(
        //                       tokenManger: tokenManger,
        //                       hubId: hubId,
        //                       projectId: projectId,
        //                       foldersOnly: foldersOnly,
        //                       includeProjectName: includeProjectName,
        //                       errors: errors);
        //
        //         if (!string.IsNullOrWhiteSpace(outPath))
        //         {
        //             var outDir = Path.GetDirectoryName(outPath);
        //         
        //             if (!string.IsNullOrWhiteSpace(outDir))
        //             {
        //                  
        //                 using (var stream = File.Create(outPath))
        //                 {
        //                     try
        //                     {
        //                         var mapper = new ExcelMapper();
        //                         if(foldersOnly)
        //                             await mapper.SaveAsync(stream, folderResults, "Folders");
        //                         else
        //                             await mapper.SaveAsync(stream, fileResults, "Content");
        //                     }
        //                     catch (Exception ex)
        //                     {
        //         
        //                         Log.Error("Error saving excel file", ex);
        //                     }
        //                     
        //                 }
        //             }
        //         }
        //         return (folderResults, fileResults);
        //     }
        //     catch (Exception ex)
        //     {
        //         Debug.WriteLine($"{MethodBase.GetCurrentMethod()?.Name ?? "Unknown"}: {ex.Message}");
        //         throw;
        //     }
        // }
        //
        // public static async Task<(List<SimpleFolder>, List<SimpleFile>)> GetProjectContent(
        //     ITokenManager tokenManger,
        //     string hubId,
        //     string projectId,
        //     bool foldersOnly,
        //     bool includeProjectName,
        //     BlockingCollection<ErrorMessage>? errors)
        // {
        //     var files = new List<SimpleFile>();
        //     var folders = new List<SimpleFolder>();
        //
        //     try
        //     {
        //         string? projectName = null; ;
        //
        //         if (includeProjectName)
        //         {
        //             try
        //             {
        //                 var project = await ForgeHelpers.GetProject(await tokenManger.GetToken(), hubId, projectId);
        //                 projectName = project?.data?.attributes?.name ?? "";
        //             }
        //             catch (Exception ex)
        //             {
        //                 Log.Warning(ex.Message);
        //             }
        //         }
        //
        //         var topFolders = await ForgeHelpers.GetTopFolders(
        //             accountId: hubId,
        //             projectId: projectId,
        //             token: await tokenManger.GetToken());
        //
        //         foreach (var topFolder in topFolders)
        //         {
        //             var name = topFolder.Name;
        //             if (includeProjectName)
        //             {
        //                 if (topFolders.Count < 2)
        //                 {
        //                     name = !string.IsNullOrEmpty(projectName) ? projectName : name;
        //                 }
        //                 else
        //                 {
        //                     name = !string.IsNullOrEmpty(projectName) ? name + "." + projectName : name;
        //                 }
        //             }
        //             topFolder.ParentPath = "";
        //             topFolder.Path = name;
        //             topFolder.Name = name;
        //             folders.Add(topFolder);
        //             var (childFolders, childFiles) = await GetFolderContent(
        //                 tokenManager: tokenManger,
        //                 projectId: projectId,
        //                 folderId: topFolder.FolderId,
        //                 foldersOnly: foldersOnly,
        //                 recurse: true,
        //                 errors: errors
        //                 );
        //             foreach (var folder in childFolders)
        //             {
        //                 folder.ParentPath = topFolder.Path;
        //                 folder.Path = Path.Combine(topFolder.Path, folder.Name);
        //                 folders.Add(folder);
        //             }
        //
        //             if (!foldersOnly)
        //             {
        //                 foreach (var file in childFiles)
        //                 {
        //                     file.ParentPath = topFolder.Path;
        //                     file.Path = Path.Combine(topFolder.Path, file.Name);
        //                     files.Add(file);
        //                 }
        //             }
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         if (errors != null)
        //             errors.Add(new ErrorMessage("Error exporting projects", ex.Message, ""));
        //     }
        //     return (folders, files);
        // }

        //public static async Task GetFolderData(ITokenManager tokenManager, string hubId, string projectId, bool recurse, string userId)
        //{
        //    try
        //    {
        //        var topFolders = await ForgeHelpers.GetTopFolders(hubId, projectId, await tokenManager.GetToken());
        //        foreach (var item in topFolders)
        //        {
        //            if (recurse)
        //            {
        //                await GetChildFolderData(
        //                    tokenManager: tokenManager,
        //                    projectId: projectId,
        //                    folderId: item.FolderId,
        //                    recurse: recurse,
        //                    userId: userId);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}

        //public static async Task GetChildFolderData(ITokenManager tokenManager, string projectId, string folderId, bool recurse, string? userId)
        //{
        //    try
        //    {
        //        var childFolders = await ForgeHelpers.GetSubFolders(
        //            token: await tokenManager.GetToken(),
        //            projectId: projectId,
        //            folderId: folderId,
        //            userId: userId);
        //        foreach (var item in childFolders)
        //        {
        //            if (recurse)
        //            {
        //                await GetChildFolderData(
        //                    tokenManager: tokenManager,
        //                    projectId: projectId,
        //                    folderId: item.FolderId,
        //                    recurse: recurse,
        //                    userId: userId);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}

        //public static async Task GetContents(string hubId, string projectId)
        //{
        //    try
        //    {
        //        var outDir = ExcelUtils.GetPath();
        //        var fileName = $"Project.Folders.{DateTime.Now.ToString("yy.MM.dd.mm.ss")}.xlsx";
        //        var path = Path.Combine(outDir!, fileName);
        //        var errors = new List<ErrorMessage>();
        //        WorksheetWrapper<Project>? wsw = null;
        //        WorksheetWrapper<ErrorMessage>? emw = null;
        //        ExcelPackage? excelPackage = null;

        //        try
        //        {
        //            var token = await ThreeLeggedMananger.Instance.GetToken();
        //            //  var token2 = await TwoLeggedManager.Instance.GetToken();
        //            var projects = await ForgeHelpers.GetProjects(
        //                accountId: hubId,
        //                token: token,
        //                errors: errors);

        //            wsw = ExcelUtils.CreateWorksheet(projects, "Projects");
        //        }
        //        catch (Exception ex)
        //        {
        //            errors.Add(new ErrorMessage("Error exporting projects", ex.Message, ""));
        //        }

        //        if (wsw != null)
        //        {
        //            if (errors.Count > 0) wsw.NextWorksheet(errors, "Errors");
        //            excelPackage = wsw.ToExcelPackage();
        //        }
        //        else
        //        {
        //            emw = ExcelUtils.CreateWorksheet(errors, "Errors");
        //            excelPackage = wsw.ToExcelPackage();
        //        }

        //        ExcelUtils.AddStyleToTable(excelPackage, 0, ExcelUtils.WsColor.BlueWhite);

        //        using (var stream = File.Create(path))
        //        {
        //            excelPackage.SaveAs(stream);
        //            stream.Close();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine($"{MethodBase.GetCurrentMethod()?.Name ?? "Unknown"}: {ex.Message}");
        //        throw;
        //    }
        //}
        #endregion
    }
}
