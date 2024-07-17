using System.IO;

namespace BulkUploaderUtils.Helpers
{
    public class FolderHelpers
    {
        public static bool PathHasFolder(string pathToFileName, string folderToCheck)
        {
            bool bHasFolder = false;

            try
            {
                var folders = pathToFileName.Split(Path.DirectorySeparatorChar);
                bHasFolder = folders.Any(x => x.ToUpper() == folderToCheck.ToUpper());
            }
            catch (Exception ex)
            {             
            }

            return bHasFolder;
        }
    }
}
