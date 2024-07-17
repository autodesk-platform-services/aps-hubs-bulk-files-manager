using Ac.Net.Authentication;
using Ac.Net.Authentication.Models;
using ApsSettings.Data;
using ApsSettings.Data.Models;

using Bulk_Uploader_Electron.Models;
using Bulk_Uploader_Electron.Utilities;

using System.Collections.Concurrent;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using Serilog;
using System.Reflection;
using Bulk_Uploader_Electron.Helpers;

namespace Bulk_Uploader_Electron.Flows
{
    public class BatchFlows
    {
        //        public static async Task<(List<SimpleFolder>, List<SimpleFile>)> QueueDownload(
        //             DataContext context,
        //             ITokenManager tokenManger,
        //             string hubId,
        //             string projectId,
        //             string outputDirectory,
        //             string? filePath,
        //             BlockingCollection<ErrorMessage>? errors = null)
        //        {
        //            List<SimpleFolder> folderResults = new List<SimpleFolder>();
        //            List<SimpleFile> fileResults = new List<SimpleFile>();

        //            try
        //            {
        //                (folderResults, fileResults) = await UtilityFlows.GetProjectContent(
        //                              tokenManger: tokenManger,
        //                              hubId: hubId,
        //                              projectId: projectId,
        //                              foldersOnly: false,
        //                              includeProjectName: true,
        //                              errors: errors);

        //                var folderList = new Dictionary<string, SimpleFolder>();

        //                foreach (var simpleFolder in folderResults)
        //                {
        //                    if (string.IsNullOrEmpty(simpleFolder.Path) || simpleFolder.Path == "Unknown")
        //                    {
        //                        continue;
        //                    }
        //                    var pathsegments = simpleFolder.Path.Split(Path.DirectorySeparatorChar);
        //                    var current = simpleFolder.Path.ToUpper().Trim();
        //                    if (!folderList.ContainsKey(current))
        //                    {
        //                        folderList[current] = simpleFolder;
        //                    }
        //                }

        //                var root = new DirectoryInfo(outputDirectory);
        //                if (!root.Exists)
        //                {
        //                    root.Create();
        //                }



        //                foreach (var item in fileResults)
        //                {
        //                    item.Status = "Started";
        //                //    await AccToDisk(tokenManger, outputDirectory, errors, item);
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                if (errors != null)
        //                    errors.Add(new ErrorMessage("Error exporting projects", ex.Message, ""));
        //            }

        //            if (!string.IsNullOrWhiteSpace(filePath))
        //            {
        //                var outDir = Path.GetDirectoryName(filePath);

        //                if (!string.IsNullOrWhiteSpace(outDir))
        //                {
        //                    WorksheetWrapper<SimpleFile>? wsw = null;
        //                    WorksheetWrapper<ErrorMessage>? emw = null;
        //                    ExcelPackage? excelPackage = null;

        //                    if (!Directory.Exists(outDir))
        //                    {
        //                        Directory.CreateDirectory(outDir);
        //                    }

        //                    wsw = ExcelUtils.CreateWorksheet(fileResults, "Downloads");

        //                    if (wsw != null)
        //                    {
        //                        if (errors.Count > 0) wsw.NextWorksheet(errors, "Errors");
        //                        excelPackage = wsw.ToExcelPackage();
        //                    }
        //                    else
        //                    {
        //                        emw = ExcelUtils.CreateWorksheet(errors, "Errors");
        //                        excelPackage = wsw.ToExcelPackage();
        //                    }

        //                    ExcelUtils.AddStyleToTable(excelPackage, 0, ExcelUtils.WsColor.BlueWhite);

        //                    using (var stream = File.Create(filePath))
        //                    {
        //                        excelPackage.SaveAs(stream);
        //                        stream.Close();
        //                    }
        //                }
        //            }

        //            return (folderResults, fileResults);

        //        }

        //            public static async Task<(List<SimpleFolder>, List<SimpleFile>)> DownloadProjecct(
        //             ITokenManager tokenManger,
        //             string hubId,
        //             string projectId,
        //             string outputDirectory,
        //             string? filePath,
        //             BlockingCollection<ErrorMessage>? errors = null)
        //        {
        //            List<SimpleFolder> folderResults = new List<SimpleFolder>();
        //            List<SimpleFile> fileResults = new List<SimpleFile>();

        //            try
        //            {
        //                (folderResults, fileResults) = await UtilityFlows.GetProjectContent(
        //                              tokenManger: tokenManger,
        //                              hubId: hubId,
        //                              projectId: projectId,
        //                              foldersOnly: false,
        //                              includeProjectName: true,
        //                              errors: errors);

        //                var folderList = new Dictionary<string, SimpleFolder>();

        //                foreach (var simpleFolder in folderResults)
        //                {
        //                    if (string.IsNullOrEmpty(simpleFolder.Path) || simpleFolder.Path == "Unknown")
        //                    {
        //                        continue;
        //                    }
        //                    var pathsegments = simpleFolder.Path.Split(Path.DirectorySeparatorChar);
        //                    var current = simpleFolder.Path.ToUpper().Trim();
        //                    if (!folderList.ContainsKey(current))
        //                    {
        //                        folderList[current] = simpleFolder;
        //                    }
        //                }

        //                var root = new DirectoryInfo(outputDirectory);
        //                if (!root.Exists)
        //                {
        //                    root.Create();
        //                }

        //                foreach (var key in folderList.Keys.ToList())
        //                {
        //                    var fi = new DirectoryInfo(Path.Combine(outputDirectory, key));
        //                    if (!fi.Exists)
        //                    {
        //                        fi.Create();
        //                    }
        //                }

        //                foreach (var item in fileResults)
        //                {
        //                    item.Status = "Started";
        //                    // await AccToDisk(tokenManger, outputDirectory, errors, item);
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                if (errors != null)
        //                    errors.Add(new ErrorMessage("Error exporting projects", ex.Message, ""));
        //            }

        //            if (!string.IsNullOrWhiteSpace(filePath))
        //            {
        //                var outDir = Path.GetDirectoryName(filePath);

        //                if (!string.IsNullOrWhiteSpace(outDir))
        //                {
        //                    WorksheetWrapper<SimpleFile>? wsw = null;
        //                    WorksheetWrapper<ErrorMessage>? emw = null;
        //                    ExcelPackage? excelPackage = null;

        //                    if (!Directory.Exists(outDir))
        //                    {
        //                        Directory.CreateDirectory(outDir);
        //                    }

        //                    wsw = ExcelUtils.CreateWorksheet(fileResults, "Downloads");

        //                    if (wsw != null)
        //                    {
        //                        if (errors.Count > 0) wsw.NextWorksheet(errors, "Errors");
        //                        excelPackage = wsw.ToExcelPackage();
        //                    }
        //                    else
        //                    {
        //                        emw = ExcelUtils.CreateWorksheet(errors, "Errors");
        //                        excelPackage = wsw.ToExcelPackage();
        //                    }

        //                    ExcelUtils.AddStyleToTable(excelPackage, 0, ExcelUtils.WsColor.BlueWhite);

        //                    using (var stream = File.Create(filePath))
        //                    {
        //                        excelPackage.SaveAs(stream);
        //                        stream.Close();
        //                    }
        //                }
        //            }

        //            return (folderResults, fileResults);

        //            //var ef = ExcelUtils.CreateWorksheet(folderResults, "Folders");
        //            //var eff = foldersOnly ? null : ef.AddWorksheet(fileResults, "Files");
        //            //if (errors != null)
        //            //{
        //            //    var err = eff == null ? ef.AddWorksheet(errors, "Errors") : eff.NextWorksheet(errors, "Errors");
        //            //    excelPackage = err.ToExcelPackage();
        //            //    ExcelUtils.AddStyleToTable(excelPackage!, 0, ExcelUtils.WsColor.BlueWhite);

        //            //}
        //            //var cnt = 0;
        //            //if (eff != null)
        //            //{
        //            //    ExcelUtils.AddStyleToTable(excelPackage!, 1, ExcelUtils.WsColor.GreenWhite);
        //            //    cnt = 1;
        //            //}
        //            //if (errors != null)
        //            //{
        //            //    ExcelUtils.AddStyleToTable(excelPackage!, cnt + 1, ExcelUtils.WsColor.GrayWhite);
        //            //}

        //            //using (var stream = File.Create(path))
        //            //{
        //            //    excelPackage!.SaveAs(stream);
        //            //    stream.Close();
        //            //}
        //        }

        public class AccToDiskData
        {

            public string OutputDirectory { get; set; }
            public string StorageObjectId { get; set; }
            public int StepId { get; set; }
            public long Size { get; set; }
            public string RelativePath { get; set; }
            public string ErrorMessage { get; set; }

            public AccToDiskData(int stepId, string outputDirectory, string storageObjectId, string relativePath, long size)
            {

                OutputDirectory = outputDirectory;
                StorageObjectId = storageObjectId;
                RelativePath = relativePath;
                Size = size;
                StepId = stepId;
            }
        }

        private static async Task AccToDisk(string data)
        {
            try
            {
                //  DataContext context = new DataContext();
                AccToDiskData args = JsonSerializer.Deserialize<AccToDiskData>(data);
                var parts = args.StorageObjectId.Split(":");
                var p = parts[parts.Length - 1];
                parts = p.Split("/");
                if (parts.Length != 2)
                {
                    var ex = new Exception("Item Id Not the Expected Format");
                    throw ex;
                }
                var timeout = args.Size > 1000000000 ? 22 : args.Size > 500000000 ? 11 : args.Size > 100000000 ? 6 : 2;

                var token = await JointTokenManager.GetToken();
                var signedUrl = await APSHelpers.GetDownloadUrl(token, parts[0], parts[1], timeout);
                var path = Path.Combine(args.OutputDirectory, args.RelativePath!);

                token = await JointTokenManager.GetToken();
                var stream = await APSHelpers.GetDownloadStream(token, args.StorageObjectId);

                CopyFileSection(stream, path, 0, stream.Length);

                //byte[]? buffer;
                //using (var memoryStream = new MemoryStream())
                //{
                //    stream.CopyTo(memoryStream);
                //    buffer = memoryStream.ToArray();
                //}
                ////stream.Write(buffer, 0, (int)stream.Length);

                //using (FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
                //{
                //    fileStream.Write(buffer, 0, buffer.Length);
                //}




            }
            catch (Exception ex)
            {

                Log.Warning(MethodBase.GetCurrentMethod().Name, ex);
                // A
            }
        }

        public static void CopyFileSection(Stream inStream, string outFile, long startPosition, long size)
        {
            // Open the files as streams

            using (var outStream = File.OpenWrite(outFile))
            {
                // seek to the start position
                inStream.Seek(startPosition, SeekOrigin.Begin);

                // Create a variable to track how much more to copy
                // and a buffer to temporarily store a section of the file
                long remaining = size;
                byte[] buffer = new byte[81920];

                do
                {
                    // Read the smaller of 81920 or remaining and break out of the loop if we've already reached the end of the file
                    int bytesRead = inStream.Read(buffer, 0, (int)Math.Min(buffer.Length, remaining));
                    if (bytesRead == 0) { break; }

                    // Write the buffered bytes to the output file
                    outStream.Write(buffer, 0, bytesRead);
                    remaining -= bytesRead;
                }
                while (remaining > 0);
            }
        }
        //        public static async Task ProcessDownloadBatch(ITokenManager tokenManager, DataContext context, List<BatchDownload> items, BlockingCollection<ErrorMessage>? errors = null)
        //        {
        //            var row = 0;

        //            foreach (var item in items)
        //            {
        //                List<SimpleFolder> folders = new List<SimpleFolder>();
        //                List<SimpleFile> files = new List<SimpleFile>();
        //                var errorText = new StringBuilder();

        //                try
        //                {
        //                    row++;
        //                    // verify inputs
        //                    if (string.IsNullOrEmpty(item.SourceHub))
        //                    {
        //                        errorText.AppendLine($"Row {row}: Hub ID must be provided");
        //                    }
        //                    if (string.IsNullOrEmpty(item.SourceProject))
        //                    {
        //                        errorText.AppendLine($"Row {row}: HProject ID must be provided");
        //                    }
        //                    if (string.IsNullOrEmpty(item.DownloadFolder))
        //                    {
        //                        errorText.AppendLine($"Row {row}: HDownload target folder must be provided");
        //                    }

        //                    if (errorText.Length > 0)
        //                    {
        //                        errors?.Add(new ErrorMessage("Error processing batch", errorText.ToString(), "No Stack"));
        //                        continue;
        //                    }

        //                    if (string.IsNullOrWhiteSpace(item.SourceFolderId))
        //                    {
        //                        (folders, files) = await UtilityFlows.GetProjectContent(
        //                            tokenManager,
        //                            item.SourceHub,
        //                            item.SourceProject,
        //                            false,
        //                            true,
        //                            new System.Collections.Concurrent.BlockingCollection<ErrorMessage>());
        //                    }
        //                    else
        //                    {
        //                        (folders, files) = await UtilityFlows.GetFolderContent(
        //                            tokenManager,
        //                            item.SourceHub,
        //                            item.SourceProject,
        //                            false,
        //                            true,
        //                            new System.Collections.Concurrent.BlockingCollection<ErrorMessage>());
        //                    }

        //                    // filter by folder
        //                    FilterFolderContents(item.IgnoreExtensions ?? "", item.IgnoreFolders ?? "", ref folders, ref files);

        //                    BuildJobArgs args = new BuildJobArgs { 
        //                        Context = context,
        //                        Description = $"Download.{item.SourceProject}.{DateTime.Now.ToString("yy.MM.dd.mm")}",
        //                        Files = files.ToList<ISimpleFile>(),
        //                        JobPrefix = "Dnld",
        //                        StepActions = null,                      
        //                    };

        //                    BuildFileJob(args);

        //                }
        //                catch (Exception ex)
        //                {
        //                    errors?.Add(new ErrorMessage("Error processing batch", ex));
        //                }
        //            }
        //        }
        //        public class BuildJobArgs
        //        {
        //            public DataContext Context { get; set; }
        //            public string Description { get; set; }
        //            public string JobPrefix { get; set; }
        //            public List<ISimpleFile> Files { get; set; }
        //            public List<Action<AggregatorActionArgs>>? StepActions { get; set; }
        //        }

        //        public class AggregatorActionArgs 
        //        {
        //            public BatchJob? Batch { get; set; }
        //            public DataContext? Context { get; set; }
        //            public JobTaskAggregator? Aggregator { get; set; }
        //            public ISimpleFile? File { get; set; }
        //            public int Index { get; set; }
        //        }

        //        public static void BuildFileJob(BuildJobArgs args)
        //        {
        //            if (args.Files.Count > 0)
        //            {
        //                BatchJob batch = new BatchJob() { Name = args.Description??"No Description" };
        //                batch.Jobs = new List<JobTaskAggregator>();
        //                args.Context.Batches.Add(batch);
        //                args.Context.SaveChanges();

        //                foreach (var file in args.Files)
        //                {
        //                    JobTaskAggregator job = new JobTaskAggregator();
        //                    args.Context.BatchTasks.Add(job);
        //                    job.JobOwner = batch;
        //                    batch.Jobs.Add(job);
        //                    job.Description = $"{args.JobPrefix ??"STD"}.{(file?.Path ?? file?.Name ?? "Unknown")}";
        //                    job.LastUpdateOn = DateTime.Now;
        //                    if (args.StepActions != null)
        //                    {
        //                        foreach (var action in args.StepActions)
        //                        {
        //                            var index = 1; 
        //                            try
        //                            {
        //                                AggregatorActionArgs actionArgs = new AggregatorActionArgs { 
        //                                    Batch = batch,
        //                                    File = file,
        //                                    Context = args.Context,
        //                                    Aggregator = job,
        //                                    Index = index
        //                                };
        //                                index++;
        //                                action(actionArgs);
        //                            }
        //                            catch (Exception ex)
        //                            {


        //                            }
        //                        }
        //                    }
        //                    args.Context.SaveChanges();
        //                }


        //            }
        //        }

        //        public const string ACCDOWNLOADSTEP = "ACCDOWNLOADSTEP";

        //        private static void AddDownloadFromApsStep(AggregatorActionArgs args)
        //        {
        //            // TaskStep step = new TaskStep()
        //            // {
        //            //     StepType = ACCDOWNLOADSTEP,
        //            //     Description = "Download from APS",
        //            //     Data = "" 
        //            // }
        //            // args.Aggregator.Steps
        //        }

        //        private static void FilterFolderContents(string ignoreExtensions, string ignoreFolders, ref List<SimpleFolder> folders, ref List<SimpleFile> files)
        //        {
        //            if (!string.IsNullOrEmpty(ignoreFolders) && folders.Count > 0)
        //            {
        //                var skippedFolders = new List<string>();
        //                var skipped = ignoreFolders.ToUpper().Split(",").Select(p => p.Trim());
        //                folders = folders.Where(p =>
        //                {
        //                    var skippedFolder = skipped.Contains(p.Name ?? "".ToUpper()) ? p : null;
        //                    if (skippedFolder != null)
        //                    {
        //                        skippedFolders.Add(skippedFolder.Path ?? "NotFound");
        //                        return false;
        //                    }
        //                    else return true;
        //                }
        //                ).ToList();

        //                if (skippedFolders.Count > 0)
        //                {
        //                    files = files.Where(p => !skippedFolders.Contains(p.ParentPath ?? "")).ToList();
        //                }

        //            }

        //            // filter by
        //            if (!string.IsNullOrEmpty(ignoreExtensions) && files.Count > 0)
        //            {
        //                var exts = ignoreExtensions.ToUpper().Split(",").Select(p => p.Trim());
        //                files = files.Where(p => !exts.Contains(p.FileType ?? "".ToUpper())).ToList();
        //            }
        //        }
    }
}