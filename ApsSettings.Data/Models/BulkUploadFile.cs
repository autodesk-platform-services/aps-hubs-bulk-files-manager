using Microsoft.ClearScript.V8;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ApsSettings.Data.Models
{
    public class BulkUploadFile
    {
        public int Id { get; set; }
        public int BulkUploadId { get; set; }
        public string SourceFileName { get; set; }
        public string SourceAbsolutePath { get; set; }
        public string SourceRelativePath { get; set; }
        public string TargetFileName { get; set; } = "";
        public string TargetRelativePath { get; set; } = "";

        public string FolderUrn { get; set; } = "";
        public string FolderUrl { get; set; } = "";
        public string ItemId { get; set; } = "";
        public string ObjectId { get; set; } = "";
        public string VersionId { get; set; } = "";
        public string WebUrl { get; set; } = "";

        [JsonConverter(typeof(StringEnumConverter))]
        public JobFileStatus Status { get; set; } = JobFileStatus.Pending;

        public string Logs { get; set; } = "";
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public DateTime LastModified { get; set; } = DateTime.Now;
        
        public void AddLogs(string log)
        {
            Logs += $"[{DateTime.Now.ToString()}] {log}\r\n";
        }

        public static BulkUploadFile CreateFile(BulkUpload bulkUpload, string sourceFolderPath, string sourceFilePath)
        {
            var bulkUploadFile = new BulkUploadFile()
            {
                BulkUploadId = bulkUpload.Id,
                SourceFileName = Path.GetFileName(sourceFilePath),
                SourceAbsolutePath = sourceFilePath,
                SourceRelativePath = sourceFolderPath,
                TargetRelativePath = sourceFolderPath,
                TargetFileName = Path.GetFileName(sourceFilePath),
                Status = bulkUpload.Status == BulkUploadStatus.Preview ? JobFileStatus.Proposed : JobFileStatus.Pending
            };

            var fileType = Path.GetExtension(bulkUploadFile.SourceFileName).ToLower().Replace(".", "");
            if (bulkUpload.ExcludedFileTypesList.Contains(fileType))
            {
                bulkUploadFile.Status = JobFileStatus.DoNotUpload;
                bulkUploadFile.AddLogs($"Do Not Upload: Filetype {fileType} is excluded");
            }

            var folderNames = Path.GetDirectoryName(sourceFilePath)
                                  ?.Split("\\")
                                  .Select(x => x.ToLower()).ToList()
                              ?? new List<string>();
            var intersectingFolderNames = bulkUpload.ExcludedFolderNamesList.Intersect(folderNames).ToList();
            if (intersectingFolderNames.Count() > 0)
            {
                bulkUploadFile.Status = JobFileStatus.DoNotUpload;
                bulkUploadFile.AddLogs(
                    $"Do Not Upload: Folders Named {string.Join(", ", intersectingFolderNames)} are excluded");
            }
            if(bulkUploadFile.Status != JobFileStatus.DoNotUpload)
            {
                if (!string.IsNullOrWhiteSpace(bulkUpload.ModifyPathScript) && bulkUpload.UseModifyPathScript)
                {
                    using (var engine = new V8ScriptEngine())
                    {
                        var nonLoopingMirrors = bulkUpload.AutodeskMirrors.Select(mirror => new BulkUploadAutodeskMirror()
                        {
                            FolderName = mirror.FolderName,
                            RelativeFolderPath = mirror.RelativeFolderPath,
                            FolderUrl = mirror.FolderUrl,
                            FolderUrn = mirror.FolderUrn
                        });
                        try
                        {
                            engine.Execute($"rootFolderPath = \"{bulkUpload.LocalPath.Replace("\\", "\\\\")}\"");
                            engine.Execute(
                                $"absoluteFilePath = \"{bulkUploadFile.SourceAbsolutePath.Replace("\\", "\\\\")}\"");
                            engine.Execute(
                                $"relativeFilePath = \"{bulkUploadFile.SourceRelativePath.Replace("\\", "\\\\")}\"");
                            engine.Execute($"fileName = \"{bulkUploadFile.SourceFileName}\"");
                            engine.Execute($"cloudFolders = {JsonConvert.SerializeObject(nonLoopingMirrors)}");

                            var scriptToExecute =
                                $"function main(){{ {bulkUpload.ModifyPathScript} }}; outputValue = main();";

                            engine.Execute(scriptToExecute);

                            var outputValue = engine.Script.outputValue;

                            if (outputValue == null)
                                throw new Exception("Unable to determine ModifyScript output. Ensure return object.");

                            if (outputValue.targetRelativePath.GetType() == typeof(string) && !string.IsNullOrEmpty(outputValue.targetRelativePath))
                                bulkUploadFile.TargetRelativePath = outputValue.targetRelativePath.StartsWith("\\") ? outputValue.targetRelativePath.Replace("\\\\", @"\\") : outputValue.targetRelativePath;

                            if (outputValue.targetFileName.GetType() == typeof(string) && !string.IsNullOrEmpty(outputValue.targetFileName))
                                bulkUploadFile.TargetFileName = outputValue.targetFileName;

                            if (outputValue.shouldUpload == false)
                            {
                                bulkUploadFile.Status = JobFileStatus.DoNotUpload;
                                bulkUploadFile.AddLogs($"Do Not Upload: File was rejected by script filter");
                            }
                        }
                        catch (Exception exception)
                        {
                            bulkUploadFile.Status = JobFileStatus.Failed;
                            bulkUploadFile.AddLogs(exception.Message);
                        }
                    }
                }
            }
            
            //Find existing Folder URN / Folder URL
            var mirrorFolder =
                bulkUpload.AutodeskMirrors.FirstOrDefault(x => x.RelativeFolderPath.ToLower() ==
                                                               (bulkUploadFile.TargetRelativePath?.ToLower() ?? ""));

            if (mirrorFolder != null)
            {
                bulkUploadFile.FolderUrn = mirrorFolder.FolderUrn;
                bulkUploadFile.FolderUrl = mirrorFolder.FolderUrl;
            }
            
            return bulkUploadFile;
        }
    }

    public enum JobFileStatus
    {
        Proposed,
        Pending,
        Success,
        Failed,
        DoNotUpload
    }
}
