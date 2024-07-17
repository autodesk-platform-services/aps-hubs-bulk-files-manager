using System.Net;
using System.Web;
using Data.Models;
using Data.Models.Forge.FolderContents;
using Data.Models.Forge.Hub;
using Data.Models.Forge.Projects;
using Data.Models.Forge.Metadata;
using Data.Models.Forge.TopFolders;
using Data.Models.Forge.Versions;
using Data.Models.Forge.WebhookResponse;
using Flurl.Http;
using Serilog;
using Newtonsoft.Json;
using RestSharp;
using Data.Managers;
using Data.Models.Forge;
using Data.Models.Forge.Bim360Project;
using Data.Models.Forge.TipResponse;
using Flurl;
using Data.Models.Forge.GetDataHooksResponse;
using Data.Models.Forge.MetadataProperties;
using mass_upload_via_s3_csharp.Models.Forge;
using mass_upload_via_s3_csharp.Models.Forge.ForgeCreateFolder;
using mass_upload_via_s3_csharp.Models.Forge.ForgeSignedS3Upload;
using mass_upload_via_s3_csharp.Models.Forge.ForgeStorageCreation;
using Project = Data.Models.Project;
using mass_upload_via_s3_csharp;
using System.Linq;
using Microsoft.Extensions.Configuration;
using BulkUploaderUtils.Models;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using Included = Data.Models.Forge.FolderContents.Included;
using Autodesk.Forge.Model;
using Ac.Net.Authentication.Models;

namespace Data.Utilities
{
    public static class ForgeHelpers
    {
        public static async Task<List<Account>> GetAccounts(string token)
        {
            var hubResponse = await "https://developer.api.autodesk.com/project/v1/hubs"
                .WithOAuthBearerToken(token)
                .GetJsonAsync<ForgeHub>();

            var accounts = new List<Account>();
            foreach (var hub in hubResponse.data)
            {
                accounts.Add(new Account()
                {
                    AccountId = hub.id,
                    Enabled = false,
                    Region = hub.attributes.region,
                    Name = hub.attributes.name
                });
            }

            return accounts;
        }

        public static async Task<List<Project>> GetProjectsWithBusinessUnits(string accountId, string region)
        {
            var url = region == "US"
                ? $"https://developer.api.autodesk.com/hq/v1/accounts/{accountId.Substring(2)}/projects"
                : $"https://developer.api.autodesk.com/hq/v1/regions/eu/accounts/{accountId.Substring(2)}/projects";

            var offset = 0;
            var limit = 100;

            var projects = new List<Project>();
            
            while (true)
            {
                var token = await TokenManager.GetTwoLeggedToken();

                var projectResponse = await $"{url}?offset={offset}&limit={limit}"
                    .WithOAuthBearerToken(token)
                    .GetJsonAsync<List<ForgeBim360Project>>();
                
                projectResponse.ForEach(project =>
                {
                    projects.Add(new Project()
                    {
                        AccountId = accountId,
                        BusinessUnitId = project.business_unit_id,
                        Name = project.name,
                        ProjectId = $"b.{project.id}"
                    });
                });

                if (projectResponse.Count == 0)
                {
                    break;
                }
                else
                {
                    offset += limit;
                }
            }

            return projects;
        }
        
        public static async Task<ForgeBatchS3Download> GetBatchS3(string bucketKey, List<string> objectIds)
        {
            try
            {
                var uri =
                    $"https://developer.api.autodesk.com/oss/v2/buckets/{bucketKey}/objects/batchsigneds3download?minutesExpiration=60";

                var token = await TokenManager.GetTwoLeggedToken();

                var filteredObjectIds = objectIds.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();

                if (filteredObjectIds.Count == 0)
                {
                    return new ForgeBatchS3Download();
                }

                var request = await uri
                    .WithOAuthBearerToken(token)
                    .PostJsonAsync(new
                    {
                        requests = filteredObjectIds
                            .Select(x => new {objectKey = x.Split("/").Last()})
                            .ToList()
                    });

                return await request.GetJsonAsync<ForgeBatchS3Download>();
            }
            catch (Exception e)
            {
                Log.Debug(e.Message);
                throw;
            }
        }




        public static async Task<List<Project>> GetProjects(string accountId, string token, string userId = null, List<ErrorMessage> errors = null)
        {
            //accountId = accountId.Substring(0, 2) == "b." ? accountId : "b." + accountId;
            var projects = new List<Project>();
            var uri = $"https://developer.api.autodesk.com/project/v1/hubs/{accountId}/projects?page[limit]=100";
            var failures = 0;

            while (uri != null)
            {
                try
                {
                  //  var token = await TokenManager.GetTwoLeggedToken();
                    
                    var projectsResponse = uri
                        .WithOAuthBearerToken(token);

                    if (!string.IsNullOrWhiteSpace(userId))
                    {
                        projectsResponse.WithHeader("x-user-id", userId);
                    }

                    var projectResponse = await projectsResponse.GetJsonAsync<ForgeProjects>();

                    foreach (var project in projectResponse.data)
                    {
                        projects.Add(new Project()
                        {
                            AccountId = accountId,
                            ProjectId = project.id,
                            Name = project.attributes.name,
                            ProjectType = project.attributes.extension.data.projectType == "ACC" ? ProjectType.ACC : ProjectType.BIM360
                        });
                    }

                    uri = projectResponse.links.next?.href;
                }
                catch (Exception exception)
                {
                    if (errors != null)
                    {
                        errors.Add(new ErrorMessage("Projects", exception.Message, exception.StackTrace??""));
                    }
                    Log.Error(exception.Message);
                    Log.Error(exception.StackTrace);
                    failures++;
                    if (failures > 5) throw;
                }
            }

            return projects;
        }
        
        public static async Task<bool> Setup()
        {
            var configuration = new ConfigurationBuilder()
           .AddJsonFile($"appsettings.Local.json", true);
            // .AddEnvironmentVariables();


            var config = configuration.Build();

            //AppSettings.AccountID = config.GetValue<string>("ACCOUNT_ID");
            //AppSettings.ForgeClientId = config.GetValue<string>("AUTODESK_CLIENT_ID");
            //AppSettings.ForgeClientSecret = config.GetValue<string>("AUTODESK_CLIENT_SECRET");
            //AppSettings.ForgeTwoLegScope = config.GetValue<string>("AUTODESK_TWO_LEGGED_SCOPES", "data:read data:write data:create");
            //AppSettings.FileWorkerCount = config.GetValue("FILE_WORKER_COUNT", "10");
            //AppSettings.FolderWorkerCount = config.GetValue("FOLDER_WORKER_COUNT", "10");

            //AppSettings.ProjectId = config.GetValue<string>("PROJECT_ID");
            //AppSettings.LocalParentPath = config.GetValue<string>("LOCAL_PARENT_PATH");
            //AppSettings.ParentFolderUrn = config.GetValue<string>("PARENT_FOLDER_URN");
            //AppSettings.CustomerExcludedFileTypes = config.GetValue<string>("EXCLUDED_FILE_TYPES");
            //AppSettings.CustomerExcludedFolderNames = config.GetValue<string>("EXCLUDED_FOLDER_NAMES");



            return true;
        }

        public static async Task<List<SimpleFolder>> GetTopFolders(string token, string accountId, string projectId)
        {
         //   AppSettings.ProjectId = projectId;

            while (true){
                try
                {
                    var topFolderResponse =
                        await
                            $"https://developer.api.autodesk.com/project/v1/hubs/{accountId}/projects/{projectId}/topFolders"
                                .WithOAuthBearerToken(token)
                                .GetJsonAsync<ForgeTopFolders>();

                    var folders = new List<SimpleFolder>();
                    foreach (var folder in topFolderResponse.data)
                    {
                        //TODO: Filter out folders
                        if (IsValidTopFolder(folder.attributes.name))
                        {
                            folders.Add(new SimpleFolder()
                            {
                                FolderId = folder.id,
                                Name = folder.attributes.name,
                                Url = folder?.links?.webView?.href ?? "",
                                IsRoot = false
                            });
                        }
                    }

                    return folders;
                }
                //catch (FlurlHttpException exception)
                //{
                //    if (exception.StatusCode == 403)
                //    {
                //        throw;
                //    }
                //    else if (exception.StatusCode == 429)
                //    {
                //        await Task.Delay(15000);
                //    }
                //    else
                //    {
                //        Log.Error(exception.Message);
                //        Log.Error(exception.StackTrace);
                //        throw;
                //    }
                //}
                catch (Exception exception)
                {
                    Log.Error(exception.Message);
                    Log.Error(exception.StackTrace);
                    throw;
                }
            }
        }
        private static bool IsValidTopFolder(string name)
        {
            Guid guidResult;
            if (name.Contains("checklist_") || name.Contains("submittals-attachments") || name.Contains("Photos") ||
                name.Contains("ProjectTb") || name.Contains("dailylog_") || name.Contains("issue_") || 
                name.Contains("correspondence-project") || name.Contains("meetings-project") || name.Contains("issues_") 
                || name.Contains("COST Root Folder") || name.Contains("Recycle Bin") || Guid.TryParse(name, out guidResult))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static async Task<List<SimpleFolder>> GetSubFolders(string token, string projectId, string folderId, string userId = null, int? limit = null)
        {
           

            //projectId = projectId.Substring(0, 2) == "b." ? projectId : "b." + projectId;

            var (folders, files) = (new List<SimpleFolder>(), new List<SimpleFile>());
            var uri = $"https://developer.api.autodesk.com/data/v1/projects/{projectId}/folders/{folderId}/contents";

            if (limit != null) uri += $"?page[limit]={limit}";

           // var failures = 0;

            while (uri != null)
            {
                try
                {
                    var request = uri
                        .WithOAuthBearerToken(token);

                    if (userId != null)
                    {
                        request = request.WithHeader("x-user-id", userId);
                    }

                    var contentsResponse = await request
                        .GetJsonAsync<ForgeFolderContents>();

                    if (contentsResponse.data != null)
                    {
                        foreach (var item in contentsResponse.data)
                        {
                            if (item.type == "folders")
                            {
                                folders.Add(new SimpleFolder()
                                {
                                    FolderId = item.id,
                                    Name = item.attributes.name,
                                });
                            }
                        }
                    }

                    uri = contentsResponse.links.next?.href ?? null;
                }
                //catch (FlurlHttpException exception)
                //{
                //    if (exception.StatusCode == 403)
                //    {
                //        Log.Warning($"GetFolderContents returned 403");
                //        throw;
                //    }
                //    //else if (exception.StatusCode == 429)
                //    //{
                //    //    failures++;
                //    //    Log.Warning($"GetFolderContents returned 429: {failures}");
                //    //    if (failures > 5) throw;
                //    //    await Task.Delay(15000);

                //    //}
                //    //else
                //    //{
                //    //    Log.Error(exception.Message);
                //    //    Log.Error(exception.StackTrace);
                //    //    failures++;

                //    //    if (failures > 5) throw;
                //    //}
                //}
                catch (Exception exception)
                {
                    Log.Error(exception.Message);
                    Log.Error(exception.StackTrace);
                    //failures++;

                    //if (failures > 5) throw;
                    throw;
                }
            }

            return folders;
        }

        public static async Task<(List<SimpleFolder>, List<SimpleFile>)> GetFolderContents(
            string token, 
            string projectId, 
            string folderId, 
            string userId, 
            bool folderOnly = false, 
            int? limit = null)
        {
            
      //      projectId = projectId.Substring(0, 2) == "b." ? projectId : "b." + projectId;

            var (folders, files) = (new List<SimpleFolder>(), new List<SimpleFile>());
            var uri = $"https://developer.api.autodesk.com/data/v1/projects/{projectId}/folders/{folderId}/contents";

            if (limit != null) uri += $"?page[limit]={limit}";
            
           // var failures = 0;

            while (uri != null)
            {
                try
                {
                    var request = uri
                        .WithOAuthBearerToken(token);

                    if (userId != null)
                    {
                        request = request.WithHeader("x-user-id", userId);
                    }

                    var contentsResponse = await request
                        .GetJsonAsync<ForgeFolderContents>();

                    if (contentsResponse.data != null)
                    {
                        foreach (var item in contentsResponse.data)
                        {
                            if (item.type == "folders")
                            {
                                folders.Add(new SimpleFolder()
                                {
                                    FolderId = item?.id ?? "Unknown",
                                    Name = item?.attributes?.name ?? "Unknown",
                                    Url = item?.links?.webView?.href ?? "",
                                    IsRoot = false
                                }); ;
                            }
                        }
                    }

                    if (!folderOnly && contentsResponse.included != null)
                    {
                        foreach (Included item in contentsResponse.included )
                        {
                            if ((item?.type ?? "") == "versions" && (item?.attributes?.fileType ?? null) != null)
                            {
                                files.Add(new SimpleFile()
                                {
                                    VersionId = item?.id ?? "Unknown",
                                    Name = item?.attributes?.name ?? "Unknown",
                                    FileType = item?.attributes?.fileType ?? "UKN",
                                    ItemId = item?.relationships?.item?.data?.id ?? "Unknown",
                                    DerivativeId = item?.relationships?.derivatives?.data?.id ?? "Missing",
                                    ObjectId = item?.relationships?.storage?.data?.id ?? "Missing",
                                    Url = item?.links?.webView?.href ?? "Missing",
                                    LastModified = item?.attributes?.lastModifiedTime ?? DateTime.MinValue,
                                    Size = item?.attributes.storageSize ?? 100
                                }) ;
                            }
                        }
                    }

                    uri = contentsResponse.links.next?.href ?? null;
                }
                //catch (FlurlHttpException exception)
                //{
                //    if (exception.StatusCode == 403)
                //    {
                //        Log.Warning($"GetFolderContents returned 403");
                //        throw;
                //    }
                //    else if (exception.StatusCode == 429)
                //    {
                //        failures++;
                //        Log.Warning($"GetFolderContents returned 429: {failures}");
                //        if (failures > 5) throw;
                //        await Task.Delay(15000);

                //    }
                //    else
                //    {
                //        Log.Error(exception.Message);
                //        Log.Error(exception.StackTrace);
                //        failures++;

                //        if (failures > 5) throw;
                //    }
                //}
                catch (Exception exception)
                {
                    Log.Error(exception.Message);
                    Log.Error(exception.StackTrace);
                   // failures++;
                   throw;
                }
            }

            return (folders, files);
        }

        public static async Task<List<SimpleHook>> CreateDataWebhooks(string folderId, string region, string twoLeggedToken, string callbackUrl)
        {
            var hookResponse = await $"https://developer.api.autodesk.com/webhooks/v1/systems/data/hooks"
                .WithOAuthBearerToken(twoLeggedToken)
                //.WithHeader("Authorization", "Bearer " + twoLeggedToken)
                //.WithHeader("Content-Type", "application/json")
                .WithHeader("x-ads-region", region)
                .AllowHttpStatus(HttpStatusCode.Conflict)
                .PostJsonAsync(new
                {
                    callbackUrl = callbackUrl,
                    scope = new
                    {
                        folder = folderId
                    }
                });

            var hookIds = new List<SimpleHook>();

            if (hookResponse.StatusCode == 409)
            {
                hookIds = await GetWebhooks("data", twoLeggedToken);
                return hookIds;
            }
                
            var hookResults = await hookResponse.GetJsonAsync<ForgeWebhookResponse>();
            
            foreach (var hook in hookResults.hooks)
            {
                hookIds.Add(new SimpleHook()
                {
                    System = "data",
                    HookId = hook.hookId,
                    Event = hook._event
                });
            }

            return hookIds;
        }
        public static async Task DeleteWebhooks(string system, string webhookEvent, string hookId, string twoLeggedToken)
        {
            var hookResponse = await $"https://developer.api.autodesk.com/webhooks/v1/systems/{system}/events/{webhookEvent}/hooks/{hookId}"
                .WithOAuthBearerToken(twoLeggedToken)
                .DeleteAsync();

            return;
        }
        public static async Task<List<SimpleHook>> GetWebhooks(string system, string twoLeggedToken, string next = "/hooks")
        {
            ForgeGetDataHooksResponse pageResults = null;
            List<SimpleHook> hooks = new List<SimpleHook>();
            while(pageResults == null || pageResults.links.next != null)
            {
                pageResults = await $"https://developer.api.autodesk.com/webhooks/v1/systems/{system}{next}"
                    .WithOAuthBearerToken(twoLeggedToken)
                    .GetJsonAsync<ForgeGetDataHooksResponse>();

                foreach(var hook in pageResults.data)
                {
                    hooks.Add(new SimpleHook()
                    {
                        Event = hook._event,
                        HookId = hook.hookId,
                        System = hook.system,
                        Callback = hook.callbackUrl,
                        FolderId = hook.tenant
                    }); ;
                }

                next = pageResults.links.next;
            }

            return hooks;
        }

        public static async Task<ForgeVersions> GetVersion(string projectId, string version)
        {
            projectId = projectId.Substring(0, 2) == "b." ? projectId : "b." + projectId;
            var token = await TokenManager.GetTwoLeggedToken();

            string encodedUrn = HttpUtility.UrlEncode(version);
            
            var request =
                await $"https://developer.api.autodesk.com/data/v1/projects/{projectId}/versions/{encodedUrn}"
                    .WithOAuthBearerToken(token)
                    .AllowHttpStatus(HttpStatusCode.TooManyRequests)
                    .GetAsync();
                        

            if (request.StatusCode == 429)
            {

                await Task.Delay(15000);
                return await GetVersion(projectId, version);
            }
                
            return await request.GetJsonAsync<ForgeVersions>();
        }

        public static async Task<ForgeMetadata> GetMetadata(string encodedUrn)
        {
            var token = await TokenManager.GetTwoLeggedToken();
            
            var metadataResponse = await $"https://developer.api.autodesk.com/modelderivative/v2/designdata/{encodedUrn}/metadata"

                .WithOAuthBearerToken(token)
                .GetJsonAsync<ForgeMetadata>();

            return metadataResponse;
        }

        public static async Task<ForgeMetadataProperties> GetMetaDataProperties(string encodedUrn, string guid)
        {
            var token = await TokenManager.GetTwoLeggedToken();
            
            //Get the full data set if possible using forceget
            var propertiesResponse = await $"https://developer.api.autodesk.com/modelderivative/v2/designdata/{encodedUrn}/metadata/{guid}/properties"
                .SetQueryParam("forgeget", "true")
                .WithOAuthBearerToken(token)
                .AllowHttpStatus(HttpStatusCode.Conflict)
                .GetJsonAsync<ForgeMetadataProperties>();

            return propertiesResponse;
        }

        public static async Task<ForgeSupportedFormats> GetSupportedFormats()
        {
            var token = await TokenManager.GetTwoLeggedToken();
            
            //Get the full data set if possible using forceget
            var response = await $"https://developer.api.autodesk.com/modelderivative/v2/designdata/formats"
                .WithOAuthBearerToken(token)
                .AllowHttpStatus(HttpStatusCode.Conflict)
                .GetJsonAsync<ForgeSupportedFormats>();

            return response;
        }

        public static async Task<List<ForgeAttributesBatchGetResult>> GetCustomAttributes(string projectId, List<string> urns)
        {
            projectId = projectId.StartsWith("b.") ? projectId.Substring(2) : projectId;
            var token = await TokenManager.GetTwoLeggedToken();
            
            var request =
                await $"https://developer.api.autodesk.com/bim360/docs/v1/projects/{projectId}/versions:batch-get"
                    .WithOAuthBearerToken(token)
                    .AllowHttpStatus(HttpStatusCode.TooManyRequests)
                    .PostJsonAsync(new
                    {
                        urns
                    });

            if (request.StatusCode == 429)
            {
                await Task.Delay(15000);
                return await GetCustomAttributes(projectId, urns);
            }

            var response = await request.GetJsonAsync<ForgeAttributesBatchGet>();

            return response.results;
        }

        public static async Task<string> GetDownloadUrl(string token, string bucketKey, string objectName, int timeout = 3)
        {
            try
            {
                var signedResponse = await $"https://developer.api.autodesk.com/oss/v2/buckets/{bucketKey}/objects/{objectName}/signeds3download"
                    .WithOAuthBearerToken(token)
                    .SetQueryParam("public-resource-fallback", true)
                    .SetQueryParam("minutesExpiration", timeout.ToString())
                    .GetJsonAsync<ForgeSignedS3Url>();

                if (signedResponse.status == "complete" || signedResponse.status == "fallback")
                {
                    return signedResponse.url;
                }
                else
                {
                    throw new Exception("Signed Response Failed");
                }
            }
            catch (Exception exception)
            {
                Log.Error(exception.Message);
                Log.Error(exception.StackTrace?? "No Stack");
                // failures++;
                throw;
            }
            
        }

        public static async Task<Stream> DownloadObject(string token, string bucketKey, string objectName)
        {
            var downloadUrl = await GetDownloadUrl(token, bucketKey, objectName);
            
            var response = await downloadUrl
                .WithHeader("ConnectionClose", true)
                .SendAsync(HttpMethod.Get);

            return await response.GetStreamAsync();
        }

        public static async Task<List<ForgeBusinessUnit>> GetBusinessUnits(string accountId, string region, string twoLeggedToken)
        {
            var url = region.ToUpper() == "US"
                ? $"https://developer.api.autodesk.com/hq/v1/accounts/{accountId}/business_units_structure"
                : $"https://developer.api.autodesk.com/hq/v1/regions/eu/accounts/{accountId}/business_units_structure";

            var businessUnitResponse = await url
                .WithOAuthBearerToken(twoLeggedToken)
                .GetJsonAsync<ForgeBusinessUnitResponse>();

            if (businessUnitResponse.business_units == null)
            {
                return new List<ForgeBusinessUnit>();
            }
            else
            {
                return businessUnitResponse.business_units;
            }
        }

        public static async Task<List<string>> GetProjectManagers(string projectId, string twoLeggedToken)
        {
            projectId = projectId.StartsWith("b.") ? projectId.Remove(0, 2) : projectId;
            
            var url = $"https://developer.api.autodesk.com/bim360/admin/v1/projects/{projectId}/users";

            var response = await url
                .SetQueryParam("filter[accessLevels]", "projectAdmin")
                .WithOAuthBearerToken(twoLeggedToken)
                .GetAsync();

            var projectManagers = await response.GetJsonAsync<ForgeProjectUsers>();
            return projectManagers.results.Select(x => x.email).ToList();
        }
        
        
        public static async Task<ForgeItemTipResponse> GetItemVersion(string projectId, string versionId, string userId)
        {

            var token = await TokenManager.GetTwoLeggedToken();
            var request =
                $"https://developer.api.autodesk.com/data/v1/projects/{projectId}/versions/{HttpUtility.UrlEncode(versionId)}"
                    .WithOAuthBearerToken(token);

            if (userId != null)
            {
                request.WithHeader("x-user-id", userId);
            }
            
            var versionResponse = await request.GetJsonAsync<ForgeItemTipResponse>();

            return versionResponse;
        }
        
        public static async Task<Stream> GetDownloadStream(string token, string storageUrn)
        {
            var bucketKey = HttpUtility.UrlEncode("wip.dm.prod");
            var objectName = HttpUtility.UrlEncode(storageUrn.Split('/').Last());

            return await DownloadObject(token, bucketKey, objectName);
        }

        public static async Task<List<(string, bool)>> CheckItemPermissions(string projectId, string userId, List<string> permissions,
            List<string> itemIds)
        {
            var batchSize = 50;
            var offset = 0;
            var resolvedPermissions = new List<(string, bool)>();
            while (offset < itemIds.Count)
            {

                try
                {

                    var body = new
                    {
                        jsonapi = new {version = "1.0"},
                        data = new
                        {
                            type = "commands",
                            attributes = new
                            {
                                extension = new
                                {
                                    type = "commands:autodesk.core:CheckPermission",
                                    version = "1.0.0",
                                    data = new
                                    {
                                        requiredActions = permissions.ToList()
                                    }
                                }
                            },
                            relationships = new
                            {
                                resources = new
                                {
                                    data = itemIds
                                        .Skip(offset)
                                        .Take(batchSize)
                                        .Select(x => new
                                        {
                                            type = "versions",
                                            id = x
                                        })
                                        .ToList()
                                }
                            }
                        }
                    };

                    var token = await TokenManager.GetTwoLeggedToken();
                    var request =
                        await $"https://developer.api.autodesk.com/data/v1/projects/{projectId}/commands"
                            .WithOAuthBearerToken(token)
                            .WithHeader("x-user-id", userId)
                            .PostJsonAsync(body);

                    var response = await request
                        .GetJsonAsync<ForgeCheckPermissionResponse>();

                    var result = response.data.attributes.extension.data.permissions.Select(x => (x.id, x.permission))
                        .ToList();
                    
                    resolvedPermissions.AddRange(result);
                }
                catch (FlurlHttpException e)
                {
                    if (e.StatusCode == 403)
                    {
                        resolvedPermissions.AddRange(itemIds.Select(x => (x, false)).ToList());
                    }
                    else
                    {
                        
                        throw;
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e.Message);
                    throw;
                }
                finally
                {
                    offset += batchSize;
                }
            }

            return resolvedPermissions;
        }
        public static async Task<bool> CheckItemPermissions(string projectId, string userId, List<string> permissions,
            string itemId)
        {
            var itemPermissions = await CheckItemPermissions(projectId, userId, permissions, new List<string>() {itemId});
            
            Nullable<(string, bool)> result = itemPermissions.FirstOrDefault();
            return result.HasValue && result.Value.Item2;
        }

        public static async Task<ForgeStorageCreation> CreateStorageLocation(string projectId, string fileName, string folderUrn)
        {
            try{
                projectId = projectId.StartsWith("b.") ? projectId : $"b.{projectId}";
                var token = await TokenManager.GetTwoLeggedToken();

                var request = await $"https://developer.api.autodesk.com/data/v1/projects/{projectId}/storage"
                    .WithOAuthBearerToken(token)
                    .PostJsonAsync(new
                    {
                        jsonapi = new {version = "1.0"},
                        data = new
                        {
                            type = "objects",
                            attributes = new
                            {
                                name = fileName
                            },
                            relationships = new
                            {
                                target = new
                                {
                                    data = new
                                    {
                                        type = "folders",
                                        id = folderUrn
                                    }
                                }
                            }
                        }
                    });

                var response = await request.GetJsonAsync<ForgeStorageCreation>();

                return response;
            }
            catch (FlurlHttpException exception)
            {
                var response = await exception.GetResponseJsonAsync();
                Log.Error(exception.Message);
                throw;
            }
            catch (Exception exception)
            {
                Log.Error(exception.Message);
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
        public static async Task<dynamic> getUploadUrls(string bucketKey, string objectKey, int? minutesExpiration, int parts = 1, int firstPart = 1, string uploadKey = null)
        {
            string endpoint = $"/buckets/{bucketKey}/objects/{HttpUtility.UrlEncode(objectKey)}/signeds3upload";
            var token = await TokenManager.GetTwoLeggedToken();

            var BASE_URL = "https://developer.api.autodesk.com/oss/v2";
            RestClient client = new RestClient(BASE_URL);
            RestRequest request = new RestRequest(endpoint, RestSharp.Method.Get);
            request.AddHeader("Authorization", "Bearer " + token);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("parts", parts, ParameterType.QueryString);
            request.AddParameter("firstPart", firstPart, ParameterType.QueryString);

            if (!string.IsNullOrEmpty(uploadKey))
            {
                request.AddParameter("uploadKey", uploadKey, ParameterType.QueryString);
            }

            if (minutesExpiration != null)
            {
                request.AddParameter("minutesExpiration", minutesExpiration, ParameterType.QueryString);
            }

            var response = await client.ExecuteAsync(request);

            //Here we handle 429 for Get Upload URLs
            if (response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                int retryAfter = 0;
                int.TryParse(response.Headers.ToList()
                    .Find(x => x.Name == "Retry-After")
                    .Value.ToString(), out retryAfter);
                Task.WaitAll(Task.Delay(retryAfter));
                return await getUploadUrls(bucketKey, objectKey, minutesExpiration, parts, firstPart, uploadKey);
            }

            return JsonConvert.DeserializeObject(response.Content);
        }


        public static async Task<ForgeSignedS3Upload> GetUploadUrls(string bucketKey, string objectKey, int partIndex, int partCount, string uploadKey = null)
        {
            try{
                var token = await TokenManager.GetTwoLeggedToken();
            
                var request = await $"https://developer.api.autodesk.com/oss/v2/buckets/{bucketKey}/objects/{objectKey}/signeds3upload"
                    .SetQueryParam("minutesExpiration", "60")
                    .SetQueryParam("firstPart", partIndex + 1) //1-indexed
                    .SetQueryParam("parts", partCount)
                    .WithOAuthBearerToken(token)
                    .GetJsonAsync<ForgeSignedS3Upload>();

                return request;
            }
            catch (FlurlHttpException exception)
            {
                var response = await exception.GetResponseJsonAsync();
                Log.Error(exception.Message);
                throw;
            }
            catch (Exception exception)
            {
                Log.Error(exception.Message);
                throw;
            }
        }

        public static async Task<dynamic> CompleteUpload(string bucketKey, string objectKey, string uploadKey)
        {
            var token = await TokenManager.GetTwoLeggedToken();
            string endpoint = $"/buckets/{bucketKey}/objects/{HttpUtility.UrlEncode(objectKey)}/signeds3upload";
            RestClient client = new RestClient($"https://developer.api.autodesk.com/oss/v2");
            RestRequest request = new RestRequest(endpoint, Method.Post);
            

            request.AddHeader("Authorization", "Bearer " + token);
            request.AddHeader("Content-Type", "application/json");

            request.AddJsonBody(new { uploadKey = $"{uploadKey}" });

            var response = await client.ExecuteAsync(request);

            return response;
        }

        public static async Task<ForgeFirstVersionResponse> CreateFirstVersion(string projectId, string fileName, string folderId,
            string objectId)
        {
            try
            {
                projectId = projectId.Split(".")[0] == "b" ? projectId : "b." + projectId;
                var token = await TokenManager.GetTwoLeggedToken();

                var request = await $"https://developer.api.autodesk.com/data/v1/projects/{projectId}/items"
                    .WithOAuthBearerToken(token)
                    .AllowHttpStatus(HttpStatusCode.TooManyRequests)
                    .PostJsonAsync(new
                    {
                        jsonapi = new {version = "1.0"},
                        data = new
                        {
                            type = "items",
                            attributes = new
                            {
                                displayName = fileName,
                                extension = new
                                {
                                    type = "items:autodesk.bim360:File",
                                    //type = "items:autodesk.core:File",
                                    version = "1.0"
                                }
                            },
                            relationships = new
                            {
                                tip = new
                                {
                                    data = new
                                    {
                                        type = "versions",
                                        id = "1"
                                    }
                                },
                                parent = new
                                {
                                    data = new
                                    {
                                        type = "folders",
                                        id = folderId
                                    }
                                }
                            }
                        },
                        included = new List<dynamic>()
                        {
                            new
                            {
                                type = "versions",
                                id = "1",
                                attributes = new
                                {
                                    name = fileName,
                                    extension = new
                                    {
                                        type = "versions:autodesk.bim360:File",
                                        version = "1.0"
                                    }
                                },
                                relationships = new
                                {
                                    storage = new
                                    {
                                        data = new
                                        {
                                            type = "objects",
                                            id = $"urn:adsk.objects:os.object:wip.dm.prod/{objectId}"
                                        }
                                    }
                                }
                            }
                        }
                    });

                if (request.StatusCode == 429)
                {
                    await Task.Delay(15000);
                    return await CreateFirstVersion(projectId, fileName, folderId, objectId);
                }

                var response = await request.GetJsonAsync<ForgeFirstVersionResponse>();
                
                return response;
            }


            catch (Exception exception)
            {
                Console.WriteLine("Failed to upload the first version of file " + fileName);
                Log.Error("Failed to upload the first version of file " + fileName);
                throw;
            }
        }

        public static async Task<ForgeFirstVersionResponse> CreateNextVersion(string projectId, string fileName, string itemId,
            string objectId)
        {
            try
            {
                var token = await TokenManager.GetTwoLeggedToken();
                projectId = projectId.Split(".")[0] == "b" ? projectId : "b." + projectId;
                var request = await $"https://developer.api.autodesk.com/data/v1/projects/{projectId}/versions"
                    .WithOAuthBearerToken(token)
                    .AllowHttpStatus(HttpStatusCode.TooManyRequests)
                    .PostJsonAsync(new
                    {
                        jsonapi = new {version = "1.0"},
                        data = new
                        {
                            type = "versions",
                            attributes = new
                            {
                                displayName = fileName,
                                extension = new
                                {
                                    type = "versions:autodesk.bim360:File",
                                    version = "1.0"
                                }
                            },
                            relationships = new
                            {
                                item = new
                                {
                                    data = new
                                    {
                                        type = "items",
                                        id = itemId
                                    }
                                },
                                storage = new
                                {
                                    data = new
                                    {
                                        type = "objects",
                                        id = $"urn:adsk.objects:os.object:wip.dm.prod/{objectId}"

                                    }
                                }
                            }
                        }});

                if (request.StatusCode == 429)
                {
                    await Task.Delay(15000);
                    return await CreateNextVersion(projectId, fileName, itemId, objectId);
                }

                var response = await request.GetJsonAsync<ForgeFirstVersionResponse>();

                return response;
            }

            catch (Exception exception)
            {
                Console.WriteLine("Could not create next version of folder: " + fileName);
                Console.WriteLine(exception.Message);
                Log.Error(exception.Message);

                throw;
            }
        }

        public static async Task<ForgeCreateFolder> CreateFolder(string projectId, string parentFolderId, string folderName)
        {
            await Task.Delay(10000);
            try
            {
                projectId = projectId.StartsWith("b.") ? projectId : $"b.{projectId}";
                var token = await TokenManager.GetTwoLeggedToken();

                var request = await $"https://developer.api.autodesk.com/data/v1/projects/{projectId}/folders"
                    .WithOAuthBearerToken(token)
                    .AllowHttpStatus(HttpStatusCode.TooManyRequests)
                    .PostJsonAsync(new
                    {
                        jsonapi = new {version = "1.0"},
                        data = new
                        {
                            type = "folders",
                            attributes = new
                            {
                                name = folderName,
                                extension = new
                                {
                                    type = "folders:autodesk.bim360:Folder",
                                    version = "1.0"
                                }
                            },
                            relationships = new
                            {
                                parent = new
                                {
                                    data = new
                                    {
                                        type = "folders",
                                        id = parentFolderId
                                    }
                                }
                            }
                        }
                    });

                if (request.StatusCode == 429)
                {
                    await Task.Delay(15000);
                    return await CreateFolder(projectId, parentFolderId, folderName);
                }

                var response = await request.GetJsonAsync<ForgeCreateFolder>();

                return response;
            }

            catch (Exception exception)
            {
                Log.Error("Error Creating Folder: " + folderName);
                Console.WriteLine("Error Creating Folder: " + folderName);
                throw;
            }
        }
    }
}