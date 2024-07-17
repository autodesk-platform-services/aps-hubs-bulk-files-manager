using Bulk_Uploader_Electron.Helpers;
using Bulk_Uploader_Electron.Managers;
using Flurl.Http;
using Serilog;
using System.Web;

namespace Bulk_Uploader_Electron.Utilities
{
    public static class APSHelpers
    {
        #region Hubs
        public static async Task<List<Models.Account>> GetHubs(string? token = null)
        {
            try
            {
                token ??= await TwoLeggedTokenManager.GetTwoLeggedToken();
                var apsHubs = await APSClientHelper.DataManagement.GetHubsAsync(accessToken: token);
                List<Models.Account> accounts = new();
                foreach (var hub in apsHubs.Data)
                {
                    accounts.Add(new Models.Account()
                    {
                        AccountId = hub.Id,
                        Enabled = false,
                        Region = hub.Attributes.Region,
                        Name = hub.Attributes.Name
                    });
                }
                return accounts;
            }
            catch (Exception exception)
            {
                Log.Error(exception.Message);
                Log.Error(exception.StackTrace ?? string.Empty);
                throw;
            }
        }
        #endregion


        #region Projects
        public static async Task<List<Models.Project>> GetHubProjects(string hubId, string token, string? userId = null, List<Models.ErrorMessage>? errors = null)
        {
            List<Models.Project> projects = new();

            var failures = 0;
            int limit = 50; // Issue with the new SDK: does not work.
            int pageNumber = 0; // Issue with the new SDK: does not work.
            bool hasNextPage = true;

            while (hasNextPage)
            {
                try
                {
                    var apsProjects = await APSClientHelper.DataManagement.GetHubProjectsAsync(hubId, xUserId: userId, accessToken: token, pageNumber: pageNumber, pageLimit: limit);
                    foreach (var project in apsProjects.Data)
                    {
                        projects.Add(new()
                        {
                            AccountId = hubId,
                            ProjectId = project.Id,
                            Name = project.Attributes.Name,
                            ProjectType = project.Attributes.Extension.Data.ProjectType == "ACC" ? Models.ProjectType.ACC : Models.ProjectType.BIM360
                        });
                    }
                    hasNextPage = apsProjects.Links.Next != null;
                    pageNumber++;
                }
                catch (Exception exception)
                {
                    errors?.Add(new("Projects", exception.Message, exception.StackTrace ?? ""));
                    Log.Error(exception.Message);
                    Log.Error(exception.StackTrace ?? string.Empty);
                    failures++;
                    if (failures > 5) throw;
                }
            }
            return projects;
        }
        public static async Task<Autodesk.DataManagement.Model.Project> GetHubProject(string token, string hubId, string projectId)
        {
            try
            {
                return await APSClientHelper.DataManagement.GetProjectAsync(hubId, projectId, accessToken: token);
            }
            catch (Exception exception)
            {
                Log.Error(exception.Message);
                Log.Error(exception.StackTrace ?? string.Empty);
                throw;
            }
        }
        public static async Task<Autodesk.DataManagement.Model.Storage> CreateStorageLocation(string projectId, string fileName, string folderUrn)
        {
            try
            {
                var token = await TwoLeggedTokenManager.GetTwoLeggedToken();
                Autodesk.DataManagement.Model.StoragePayload storagePayload = new()
                {
                    Jsonapi = new() { _Version = Autodesk.DataManagement.Model.VersionNumber._10 },
                    Data = new()
                    {
                        Type = Autodesk.DataManagement.Model.Type.Objects,
                        Attributes = new()
                        {
                            Name = fileName
                        },
                        Relationships = new()
                        {
                            Target = new()
                            {
                                Data = new()
                                {
                                    Type = Autodesk.DataManagement.Model.Type.Folders,
                                    Id = folderUrn
                                }
                            }
                        }
                    }
                };
                return await APSClientHelper.DataManagement.CreateStorageAsync(projectId, storagePayload: storagePayload, accessToken: token);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Log.Error(ex.Message);
                throw;
            }
        }
        public static async Task<Autodesk.DataManagement.Model.Folder> CreateFolder(string projectId, string parentFolderId, string folderName)
        {
            await Task.Delay(10000);  // What is the purpose of this 10sec delay?
            try
            {
                projectId = projectId.StartsWith("b.") ? projectId : $"b.{projectId}";  // Is it required?
                var token = await TwoLeggedTokenManager.GetTwoLeggedToken();

                Autodesk.DataManagement.Model.FolderPayload folderPayload = new()
                {
                    Jsonapi = new() { _Version = Autodesk.DataManagement.Model.VersionNumber._10 },
                    Data = new()
                    {
                        Type = Autodesk.DataManagement.Model.Type.Folders,
                        Attributes = new()
                        {
                            Name = folderName,
                            Extension = new()
                            {
                                Type = Autodesk.DataManagement.Model.Type.FoldersautodeskBim360Folder, // "folders:autodesk.bim360:Folder",
                                _Version = Autodesk.DataManagement.Model.VersionNumber._10
                            }
                        },
                        Relationships = new()
                        {
                            Parent = new()
                            {
                                Data = new()
                                {
                                    Type = Autodesk.DataManagement.Model.Type.Folders,
                                    Id = parentFolderId
                                }
                            }
                        }
                    }
                };
                var apsCreateFolder = await APSClientHelper.DataManagement.CreateFolderAsync(projectId, folderPayload: folderPayload, accessToken: token);
                // Issue with the new SDK:  how to handle 429 status response for retry.
                return apsCreateFolder;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Creating Folder: " + folderName);
                Log.Error("Error Creating Folder: " + folderName);
                Console.WriteLine(ex.Message);
                Log.Error(ex.Message);
                throw;
            }
        }
        #endregion


        #region Folders
        public static async Task<List<Models.SimpleFolder>> GetTopFolders(string token, string hubId, string projectId)
        {
            try
            {
                var apsProjectTopFolders = await APSClientHelper.DataManagement.GetProjectTopFoldersAsync(hubId, projectId, accessToken: token, excludeDeleted: true);
                List<Models.SimpleFolder> folders = new();
                foreach (var folder in apsProjectTopFolders.Data)
                {
                    if (IsValidTopFolder(folder.Attributes.Name))
                    {
                        folders.Add(new()
                        {
                            FolderId = folder.Id,
                            Name = folder.Attributes.Name,
                            Url = folder.Links.WebView.Href,
                            IsRoot = false
                        });
                    }
                }
                return folders;
            }
            catch (Exception exception)
            {
                Log.Error(exception.Message);
                Log.Error(exception.StackTrace ?? string.Empty);
                throw;
            }
        }
        public static async Task<Models.SimpleFolder> GetRootFolder(string token, string hubId, string projectId, string apsFolderUrn)
        {
            try
            {
                var apsProject = await APSClientHelper.DataManagement.GetProjectAsync(hubId, projectId, accessToken: token);
                var apsFolder = await APSClientHelper.DataManagement.GetFolderAsync(projectId, apsFolderUrn, accessToken: token);

                return new()
                {
                    Name = apsFolder.Data.Attributes.Name,
                    ParentPath = apsFolder.Data.Attributes.DisplayName,
                    FolderId = apsFolderUrn,
                    Path = $"{apsProject.Data.Attributes.Name}/{apsFolder.Data.Attributes.Name}"
                };
            }
            catch (Exception exception)
            {
                Log.Error(exception.Message);
                Log.Error(exception.StackTrace ?? string.Empty);
                throw;
            }
        }
        public static async Task<(List<Models.SimpleFolder>, List<Models.SimpleFile>)> GetFolderContents(string token, string projectId, string folderId)
        {
            var (folders, files) = (new List<Models.SimpleFolder>(), new List<Models.SimpleFile>());

            var failures = 0;
            var limit = 1; // Issue with the new SDK: does not work.
            int pageNumber = 0; // Issue with the new SDK: does not work.
            bool hasNextPage = true;

            while (hasNextPage)
            {
                try
                {
                    var apsFolderContent = await APSClientHelper.DataManagement.GetFolderContentsAsync(projectId, folderId, accessToken: token, pageLimit: limit, pageNumber: pageNumber);
                    foreach (var item in apsFolderContent.Data)
                    {
                        if (item.Type == "folders")
                        {
                            folders.Add(new()
                            {
                                FolderId = item.Id,
                                Name = item.Attributes.Name,
                                Url = item.Links.Self.Href,  // Issue with the new SDK: does not support webView link
                                IsRoot = false
                            });
                        }
                    }
                    if (apsFolderContent.Included != null)
                        foreach (var item in apsFolderContent.Included)
                        {
                            if (item.Type == "versions" && item.Attributes.FileType != null)
                            {
                                Models.SimpleFile newfile = new()
                                {
                                    VersionId = item.Id,
                                    Name = item.Attributes.Name,
                                    FileType = item.Attributes.FileType,
                                    ItemId = item.Relationships.Item.Data.Id,
                                    DerivativeId = item.Relationships.Derivatives.Data.Id,
                                    ObjectId = item.Relationships.Storage.Data.Id,
                                    Url = item.Links.WebView.Href,
                                    Size = Convert.ToInt64(item.Attributes.StorageSize)
                                };
                                if (DateTime.TryParse(item.Attributes.LastModifiedTime, out DateTime itemTime))
                                    newfile.LastModified = itemTime;
                                else
                                    newfile.LastModified = DateTime.MinValue;
                                files.Add(newfile);
                            }
                        }
                    hasNextPage = apsFolderContent.Links.Next != null;
                    pageNumber++;
                }
                catch (Exception exception)
                {
                    Log.Error(exception.Message);
                    Log.Error(exception.StackTrace ?? string.Empty);
                    failures++;
                    if (failures > 5) throw;
                }
            }
            return (folders, files);
        }

        private static bool IsValidTopFolder(string name)
        {
            if (name.Contains("checklist_") || name.Contains("submittals-attachments") || name.Contains("Photos") ||
                name.Contains("ProjectTb") || name.Contains("dailylog_") || name.Contains("issue_") ||
                name.Contains("correspondence-project") || name.Contains("meetings-project") || name.Contains("issues_")
                || name.Contains("COST Root Folder") || name.Contains("Recycle Bin") || Guid.TryParse(name, out _)
                || name.Contains("quantification_") || name.Contains("rfis_project_") || name.Contains("assets_"))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        #endregion


        #region Items
        public static async Task<Autodesk.DataManagement.Model.Item> CreateFirstVersion(string projectId, string fileName, string folderId, string objectId)
        {
            try
            {
                projectId = projectId.Split(".")[0] == "b" ? projectId : "b." + projectId;  // Is it required?
                var token = await TwoLeggedTokenManager.GetTwoLeggedToken();

                Autodesk.DataManagement.Model.ItemPayload itemPayload = new()
                {
                    Jsonapi = new() { _Version = Autodesk.DataManagement.Model.VersionNumber._10 },
                    Data = new()
                    {
                        Type = Autodesk.DataManagement.Model.Type.Items,
                        Attributes = new()
                        {
                            DisplayName = fileName,
                            Extension = new()
                            {
                                Type = Autodesk.DataManagement.Model.Type.ItemsautodeskBim360File, // "items:autodesk.bim360:File",,
                                _Version = Autodesk.DataManagement.Model.VersionNumber._10
                            }
                        },
                        Relationships = new()
                        {
                            Tip = new()
                            {
                                Data = new()
                                {
                                    Type = Autodesk.DataManagement.Model.Type.Versions,
                                    Id = "1"
                                }
                            },
                            Parent = new()
                            {
                                Data = new()
                                {
                                    Type = Autodesk.DataManagement.Model.Type.Folders,
                                    Id = folderId
                                }
                            }
                        }
                    },
                    Included = new()
                    {
                        new() {
                            Type = Autodesk.DataManagement.Model.Type.Versions,
                            Id = "1",
                            Attributes = new()
                            {
                                Name = fileName,
                                Extension = new()
                                {
                                    Type = Autodesk.DataManagement.Model.Type.VersionsautodeskBim360File, // "versions:autodesk.bim360:File",
                                    _Version = Autodesk.DataManagement.Model.VersionNumber._10
                                }
                            },
                            Relationships = new()
                            {
                                Storage = new()
                                {
                                    Data = new()
                                    {
                                        Type = Autodesk.DataManagement.Model.Type.Objects,
                                        Id = $"urn:adsk.objects:os.object:wip.dm.prod/{objectId}"
                                    }
                                }
                            }
                        }
                    }
                };
                var apsCreateItems = await APSClientHelper.DataManagement.CreateItemAsync(projectId, itemPayload: itemPayload, accessToken: token);
                // Issue with the new SDK:  how to handle 429 status response for retry.
                return apsCreateItems;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to upload the first version of file " + fileName);
                Log.Error("Failed to upload the first version of file " + fileName);
                Console.WriteLine(ex.Message);
                Log.Error(ex.Message);
                throw;
            }
        }
        #endregion


        #region Versions
        public static async Task<Autodesk.DataManagement.Model.ModelVersion> CreateNextVersion(string projectId, string fileName, string itemId, string objectId)
        {
            try
            {
                projectId = projectId.Split(".")[0] == "b" ? projectId : "b." + projectId;
                var token = await TwoLeggedTokenManager.GetTwoLeggedToken();

                Autodesk.DataManagement.Model.VersionPayload versionPayload = new()
                {
                    Jsonapi = new() { _Version = Autodesk.DataManagement.Model.VersionNumber._10 },
                    Data = new()
                    {
                        Type = Autodesk.DataManagement.Model.Type.Versions,
                        Attributes = new()
                        {
                            DisplayName = fileName,
                            Extension = new()
                            {
                                Type = Autodesk.DataManagement.Model.Type.VersionsautodeskBim360File, // "versions:autodesk.bim360:File",,
                                _Version = Autodesk.DataManagement.Model.VersionNumber._10
                            }
                        },
                        Relationships = new()
                        {
                            Item = new()
                            {
                                Data = new()
                                {
                                    Type = Autodesk.DataManagement.Model.Type.Items,
                                    Id = itemId
                                }
                            },
                            Storage = new()
                            {
                                Data = new()
                                {
                                    Type = Autodesk.DataManagement.Model.Type.Objects,
                                    Id = $"urn:adsk.objects:os.object:wip.dm.prod/{objectId}"
                                }
                            }
                        }
                    }
                };
                var apsCreateVersions = await APSClientHelper.DataManagement.CreateVersionAsync(projectId, versionPayload: versionPayload, accessToken: token);
                // Issue with the new SDK:  how to handle 429 status response for retry.
                return apsCreateVersions;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not create next version of folder: " + fileName);
                Log.Error("Could not create next version of folder: " + fileName);
                Console.WriteLine(ex.Message);
                Log.Error(ex.Message);
                throw;
            }
        }
        #endregion


        #region OSS
        public static async Task<string> GetDownloadUrl(string token, string bucketKey, string objectKey, int minutesExpiration = 3)
        {
            try
            {
                var apsS3DownloadUrl = await APSClientHelper.OssApi.SignedS3DownloadAsync(bucketKey, objectKey, accessToken: token, minutesExpiration: minutesExpiration, publicResourceFallback: true);

                if (apsS3DownloadUrl.Content.Status == "complete" || apsS3DownloadUrl.Content.Status == "fallback")
                    return apsS3DownloadUrl.Content.Url;
                else throw new Exception("Signed Response Failed");
            }
            catch (Exception exception)
            {
                Log.Error(exception.Message);
                Log.Error(exception.StackTrace ?? "No Stack");
                throw;
            }
        }
        public static async Task<Stream> GetDownloadStream(string token, string storageUrn)
        {
            try
            {
                var bucketKey = HttpUtility.UrlEncode("wip.dm.prod");
                var objectName = HttpUtility.UrlEncode(storageUrn.Split('/').Last());
                var downloadUrl = await GetDownloadUrl(token, bucketKey, objectName);
                var response = await downloadUrl
                    .WithHeader("ConnectionClose", true)
                    .SendAsync(HttpMethod.Get);
                return await response.GetStreamAsync();
            }
            catch (Exception exception)
            {
                Log.Error(exception.Message);
                Log.Error(exception.StackTrace ?? string.Empty);
                throw;
            }
        }
        /// <summary>
        /// Return the URLs to upload the file
        /// </summary>
        /// <param name="bucketKey">Bucket key</param>
        /// <param name="objectKey">Object key</param>
        /// <param name="parts">[parts=1] How many URLs to generate in case of multi-part upload</param>
        /// <param name="firstPart">B[firstPart=1] Index of the part the first returned URL should point to</param>
        /// <param name="uploadKey">[uploadKey] Optional upload key if this is a continuation of a previously initiated upload</param>
        /// <param name="minutesExpiration">[minutesExpiration] Custom expiration for the upload URLs (within the 1 to 60 minutes range). If not specified, default is 2 minutes.
        public static async Task<Autodesk.Oss.Model.Signeds3uploadResponse> GetUploadUrls(string bucketKey, string objectKey, int? minutesExpiration, int parts = 1, int firstPart = 1, string uploadKey = null)
        {
            var token = await TwoLeggedTokenManager.GetTwoLeggedToken();

            var apsS3UploadUrl = await APSClientHelper.OssApi.SignedS3UploadAsync(bucketKey, objectKey, accessToken: token, minutesExpiration: minutesExpiration, parts: parts, firstPart: firstPart, uploadKey: uploadKey);
            if (apsS3UploadUrl.HttpResponse.IsSuccessStatusCode)
            {
                return apsS3UploadUrl.Content;
            }
            if (apsS3UploadUrl.HttpResponse.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            {
                _ = int.TryParse(apsS3UploadUrl.HttpResponse.Headers.GetValues("Retry-After").FirstOrDefault(), out int retryAfter);
                await Task.Delay(retryAfter);
                return await GetUploadUrls(bucketKey, objectKey, minutesExpiration, parts, firstPart, uploadKey);
            }
            else
            {
                throw new Exception("Failed to get upload URLs");
            }
        }
        public static async Task<bool> CompleteUpload(string bucketKey, string objectKey, string uploadKey)
        {
            try
            {
                var token = await TwoLeggedTokenManager.GetTwoLeggedToken();
                Autodesk.Oss.Model.Completes3uploadBody body = new() { UploadKey = uploadKey };
                var apsCompleteUpload = await APSClientHelper.OssApi.CompleteSignedS3UploadAsync(bucketKey, objectKey, "application/json", body, accessToken: token);
                return apsCompleteUpload.IsSuccessStatusCode;
            }
            catch (Exception exception)
            {
                Log.Error(exception.Message);
                Log.Error(exception.StackTrace ?? string.Empty);
                throw;
            }
        }
        #endregion
    }
}
