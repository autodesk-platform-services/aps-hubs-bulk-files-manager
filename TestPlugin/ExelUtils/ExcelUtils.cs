using Bogus;
using Ganss.Excel;
using TestPlugin.Models;

namespace TestPlugin.Excel
{
    public class ExcelUtils
    {
        public const string UploadFile = "Test.Upload";

        public const string UploadWorksheet = "TestUploads";

        public const string DownloadFile = "Test.Download";

        public const string DownloadWorksheet = "TestDownloads";

        public static TestDownload MockTestDownload(int index)
        {
            var mock = new Faker<TestDownload>()
                .StrictMode(true)
                .RuleFor(o => o.SourceProject, f => f.Internet.Url())
                .RuleFor(o => o.SourceFolderId, f => f.Internet.Url())
                .RuleFor(o => o.DownloadFolder, f => $"c:\\temp\\{index.ToString("000")}")
                .RuleFor(o => o.IgnoreExtensions, f => f.System.FileExt())
                .RuleFor(o => o.IgnoreFolders, f => f.Lorem.Word())
                .RuleFor(o => o.SourceHub, f => f.Internet.Url())
                .RuleFor(o => o.UseProjectForRoot, f => true)
                .RuleFor(o => o.Name, "TestBatch_" + index.ToString());
            return mock;
        }



        public static byte[] MockTestDownloadXlsx()
        {
            try
            {
                var list = new List<TestDownload>();
                for (int i = 1; i < 10; i++)
                {
                    list.Add(MockTestDownload(i));
                }
                

                using (var stream = new MemoryStream())
                {
                    new ExcelMapper().Save(stream, list, "TestDownload");
                    return stream.ToArray();
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning("Template download", ex);
                throw;
            }
        }

    }
}