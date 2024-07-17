using ApsSettings.Data.Models;
using Bogus;
using EPPlus.Core.Extensions;
using EPPlus.Core.Extensions.Attributes;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using OfficeOpenXml.Table;
using System.IO;

namespace BulkUploaderUtils.Excel
{
    public static class ExcelUtils
    {
        public const string UploadFile = "Batch.Upload";

        public const string UploadWorksheet = "BatchUploads";

        public const string DownloadFile = "Batch.Download";

        public const string DownloadWorksheet = "BatchDownloads";

        public enum WsColor
        {
            BlueWhite = TableStyles.Medium2,
            OrangeWhite = TableStyles.Medium3,
            GrayWhite = TableStyles.Medium4,
            YellowWhite = TableStyles.Medium5,
            LBlueWhite = TableStyles.Medium6,
            GreenWhite = TableStyles.Medium7,
            BlueBlue = TableStyles.Medium9,
            OrangeOrange = TableStyles.Medium10,
            GrayGray = TableStyles.Medium11,
            YellowYellow = TableStyles.Medium12,
            LBlueLBlue = TableStyles.Medium13,
            GreenGreen = TableStyles.Medium14
        }

        public static void AddStyleToTable(ExcelPackage package, int worksheetNumber, WsColor color)
        {
            try
            {
                package.Workbook.Worksheets[worksheetNumber].Tables[0].TableStyle = (TableStyles)color;
            }
            catch { }
        }

        public static WorksheetWrapper<T1> AddWorksheet<T1, T2>(this WorksheetWrapper<T2> wrapper, IEnumerable<T1> data, string worksheetName)
        {
            return wrapper.NextWorksheet(data, worksheetName)
                .WithConfiguration(configuration => configuration.WithColumnConfiguration(x => x.AutoFit()));
        }

        public static WorksheetWrapper<T1> CreateWorksheet<T1>(IEnumerable<T1> data, string worksheetName)
        {
            return data
                .ToWorksheet(worksheetName)                
                .WithConfiguration(configuration => configuration
                .WithColumnConfiguration(x => x.AutoFit())); ;
        }
        public static BatchDownload MockBatchDownload()
        {
            var mock = new Faker<BatchDownload>()
                .StrictMode(true)
                .RuleFor(o => o.SourceProject, f => f.Internet.Url())
                .RuleFor(o => o.SourceFolderId, f => f.Internet.Url())
                .RuleFor(o => o.DownloadFolder, f => f.System.DirectoryPath())
                .RuleFor(o => o.IgnoreExtensions, f => f.System.FileExt())
                .RuleFor(o => o.IgnoreFolders, f => f.Lorem.Word());
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

        public static string GetPath()
        {
            var folder = AppSettings.Instance.OutputFolder;
            if (string.IsNullOrEmpty(folder))
            {
                folder = Path.GetTempPath();
            }
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            return folder;
        }


        public static void MockBatchUploadXlsx(string path)
        {
            try
            {
                var dir = Path.GetDirectoryName(path);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir!);
                }

                var data = new List<BatchUpload>()
                {
                    MockBatchUpload(), MockBatchUpload()
                };

                var excelPackage = CreateWorksheet(data, UploadWorksheet)
                    .ToExcelPackage();

                AddStyleToTable(excelPackage, 0, WsColor.BlueWhite);

                using (var stream = File.Create(path))
                {
                    excelPackage.SaveAs(stream);
                    stream.Close();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static List<BatchUpload> LoadBatchUploadXlsx(Stream  stream)
        {
            try
            {
                ExcelPackage ep = new ExcelPackage(stream);

                ExcelWorksheet? ws = null;

                for (int i = 0; i < ep.Workbook.Worksheets.Count; i++)
                {
                    if (ep.Workbook.Worksheets[i].Name == UploadWorksheet)
                    {
                        ws = ep.Workbook.Worksheets[i];
                        break;
                    }
                }

                if (ws == null)
                {
                    throw new Exception($"Worksheet with name {UploadWorksheet} was not found");
                }

                var list = ep.GetWorksheet(UploadWorksheet)
                    .ToList<BatchUpload>(configuration => configuration.SkipCastingErrors());
                return list;
            }
            catch(Exception ex) 
            {
                throw;
            }
        }

        public static List<BatchDownload> LoadBatchDownloadXlsx(Stream stream)
        {
            try
            {
                ExcelPackage ep = new ExcelPackage(stream);

                ExcelWorksheet? ws = null;

                for (int i = 0; i < ep.Workbook.Worksheets.Count; i++)
                {
                    if (ep.Workbook.Worksheets[i].Name == DownloadWorksheet)
                    {
                        ws = ep.Workbook.Worksheets[i];
                        break;
                    }
                }

                if (ws == null)
                {
                    throw new Exception($"Worksheet with name {DownloadWorksheet} was not found");
                }

                var list = ep.GetWorksheet(DownloadWorksheet)
                    .ToList<BatchDownload>(configuration => configuration.SkipCastingErrors());
                return list;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static void MockBatchDownladXlsx(string path)
        {
            try
            {
                var dir = Path.GetDirectoryName(path);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir!);
                }
                var data = new List<BatchDownload>()
                {
                    MockBatchDownload(), MockBatchDownload()
                };

                var excelPackage = CreateWorksheet(data, DownloadWorksheet)
                    .ToExcelPackage();

                AddStyleToTable(excelPackage, 0, WsColor.BlueWhite);

                using (var stream = File.Create(path))
                {
                    excelPackage.SaveAs(stream);
                    stream.Close();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}