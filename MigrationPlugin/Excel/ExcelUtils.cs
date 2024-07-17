using Bogus;
using Ganss.Excel;
using MigrationPlugin.Models;

namespace MigrationPlugin.Excel
{
    public class ExcelUtils
    {
        public const string UploadFile = "Batch.Upload";

        public const string UploadWorksheet = "BatchUploads";

        public const string DownloadFile = "Batch.Download";

        public const string DownloadWorksheet = "BatchDownloads";

        public static BatchDownload MockBatchDownload()
        {
            var mock = new Faker<BatchDownload>()
                .StrictMode(true)
                .RuleFor(o => o.SourceProject, f => f.Internet.Url())
                .RuleFor(o => o.SourceFolderId, f => f.Internet.Url())
                .RuleFor(o => o.DownloadFolder, f => f.System.DirectoryPath())
                .RuleFor(o => o.IgnoreExtensions, f => f.System.FileExt())
                .RuleFor(o => o.IgnoreFolders, f => f.Lorem.Word())
                .RuleFor(o => o.SourceHub, f => f.Internet.Url())
                .RuleFor(o => o.UseProjectForRoot, f => true)
                .RuleFor(o => o.Name, f => f.Name.LastName());
            return mock;
        }

        public static BatchUpload MockBatchUpload()
        {
            var mock = new Faker<BatchUpload>()
                .StrictMode(true)
                .RuleFor(o => o.TargetFolderUrl, f => f.Internet.Url())
                .RuleFor(o => o.RootFolderPath, f => f.System.DirectoryPath())
                .RuleFor(o => o.IgnoreExtensions, f => f.System.FileExt())
                .RuleFor(o => o.IgnoreFolders, f => f.Lorem.Word());
            return mock;
        }

        public static byte[] MockBatchDownloadXlsx()
        {
            try
            {
                var data = new List<BatchDownload>()
                {
                    MockBatchDownload(), MockBatchDownload()
                };

                using (var stream = new MemoryStream())
                {
                    new ExcelMapper().Save(stream, data, "BatchDownload");
                    return stream.ToArray();
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning("Template download", ex);
                throw;
            }
        }

        public static byte[] MockBatchUploadXlsx(string path)
        {
            try
            {
                var dir = Path.GetDirectoryName(path);
                var fileName = Path.GetFileName(path);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir!);
                }

                var data = new List<BatchUpload>()
                {
                    MockBatchUpload(), MockBatchUpload()
                };

                using (var stream = new MemoryStream())
                {
                    new ExcelMapper().Save(stream, data, "BatchUpload");
                    return stream.ToArray();
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning("Template download", ex);
                throw;
            }
        }

        //public static List<BatchUpload> LoadBatchUploadXlsx(Stream stream)
        //{
        //    try
        //    {
        //        ExcelPackage ep = new ExcelPackage(stream);

        //        ExcelWorksheet? ws = null;

        //        for (int i = 0; i < ep.Workbook.Worksheets.Count; i++)
        //        {
        //            if (ep.Workbook.Worksheets[i].Name == UploadWorksheet)
        //            {
        //                ws = ep.Workbook.Worksheets[i];
        //                break;
        //            }
        //        }

        //        if (ws == null)
        //        {
        //            throw new Exception($"Worksheet with name {UploadWorksheet} was not found");
        //        }

        //        var list = ep.GetWorksheet(UploadWorksheet)
        //            .ToList<BatchUpload>(configuration => configuration.SkipCastingErrors());
        //        return list;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}

        //public static List<BatchDownload> LoadBatchDownloadXlsx(Stream stream)
        //{
        //    try
        //    {
        //        ExcelPackage ep = new ExcelPackage(stream);

        //        ExcelWorksheet? ws = null;

        //        for (int i = 0; i < ep.Workbook.Worksheets.Count; i++)
        //        {
        //            if (ep.Workbook.Worksheets[i].Name == DownloadWorksheet)
        //            {
        //                ws = ep.Workbook.Worksheets[i];
        //                break;
        //            }
        //        }

        //        if (ws == null)
        //        {
        //            throw new Exception($"Worksheet with name {DownloadWorksheet} was not found");
        //        }

        //        var list = ep.GetWorksheet(DownloadWorksheet)
        //            .ToList<BatchDownload>(configuration => configuration.SkipCastingErrors());
        //        return list;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}

        //public static void MockBatchDownladXlsx(string path)
        //{
        //    try
        //    {
        //        var dir = Path.GetDirectoryName(path);
        //        if (!Directory.Exists(dir))
        //        {
        //            Directory.CreateDirectory(dir!);
        //        }
        //        var data = new List<BatchDownload>()
        //        {
        //            MockBatchDownload(), MockBatchDownload()
        //        };

        //        //var excelPackage = CreateWorksheet(data, DownloadWorksheet)
        //        //    .ToExcelPackage();

        //        //AddStyleToTable(excelPackage, 0, WsColor.BlueWhite);

        //        using (var stream = File.Create(path))
        //        {
        //        //    excelPackage.SaveAs(stream);
        //            stream.Close();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}
    }
}