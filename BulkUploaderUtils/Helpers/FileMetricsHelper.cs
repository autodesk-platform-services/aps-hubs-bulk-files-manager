using Serilog;


namespace BulkUploaderUtils.Helpers
{
    public class FileCollectionMetrics
    {
        public string Path = "";
        public float TotalSizeMb = 0;
        public int TotalFileCount = 0;
    }
    public class FileMetricsHelper
    {
        /// <summary>
        /// Given a path, this will iterate all files recursively for a total file size (MB) and file count
        /// </summary>
        /// <param name="rootFolderPath"></param>
        /// <returns></returns>
        public FileCollectionMetrics GetFileSizeAndCount(string rootFolderPath)
        {

            var metrics = new FileCollectionMetrics();

            try
            {
                metrics.Path = rootFolderPath;

                // Get full exclusion list
                string[] customerExclusion = AppSettings.Instance.CustomerExcludedFileTypes.Split(",");
                string[] folderExclusion = AppSettings.Instance.CustomerExcludedFolderNames.Split(",");
                var accExclusion = AppSettings.Instance.IllegalFileTypes;  //Known ACC 
                accExclusion = accExclusion.Union(customerExclusion).ToArray();

                accExclusion = accExclusion.Select(s => s.ToUpperInvariant()).ToArray();

                var localFiles = Directory
                    .GetFiles(rootFolderPath, "*", SearchOption.AllDirectories)
                    .ToList();

                long filesInBytes = 0;
                int fileCount = 0;

                foreach (string filename in localFiles)
                {
                    foreach (var fold in folderExclusion)
                    {
                        var folders = filename.Split(Path.DirectorySeparatorChar);
                        if (!folders.Any(x => x.ToUpper() == fold.ToUpper()))
                        {
                            string extension = Path.GetExtension(filename)
                            .ToUpper()
                            .Replace(".", "");

                            if (!accExclusion.Contains(extension))
                            {
                                fileCount++;
                                filesInBytes += new FileInfo(filename).Length;
                            }
                        }
                    }
                }

                //Convert bytes to Mega
                var filesInMb = (filesInBytes / 1024f) / 1024f;
                metrics.TotalSizeMb = filesInMb;
                metrics.TotalFileCount = fileCount;
            }
            catch (Exception ex) 
            {
                Log.Error("Problem calculating file sizes and count for local files");
            }

            return metrics;
        }

    }
}
