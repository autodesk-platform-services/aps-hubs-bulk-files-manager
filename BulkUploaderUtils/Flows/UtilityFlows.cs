using Ac.Net.Authentication;
using Ac.Net.Authentication.Models;
using Autodesk.Forge.Model;
using BulkUploaderUtils.Excel;
using BulkUploaderUtils.Models;
using Data.Managers;
using Data.Models;
using Data.Utilities;
using EPPlus.Core.Extensions;
using OfficeOpenXml;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using Project = Data.Models.Project;

namespace BulkUploaderUtils.Flows
{
    public class UtilityFlows
    {
        public static async Task<List<Account>> GetHubs(string? filePath)
        {
            var results = new List<Account>();
            try
            {
                var errors = new List<ErrorMessage>();

                try
                {
                    var token = await ThreeLeggedMananger.Instance.GetToken();
                    results = await ForgeHelpers.GetAccounts(token);
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

                        WorksheetWrapper<Account>? wsw = null;
                        WorksheetWrapper<ErrorMessage>? emw = null;
                        ExcelPackage? excelPackage = null;

                        wsw = ExcelUtils.CreateWorksheet(results, "Hubs");

                        if (wsw != null)
                        {
                            if (errors.Count > 0) wsw.NextWorksheet(errors, "Errors");
                            excelPackage = wsw.ToExcelPackage();
                        }
                        else
                        {
                            emw = ExcelUtils.CreateWorksheet(errors, "Errors");
                            excelPackage = wsw.ToExcelPackage();
                        }

                        ExcelUtils.AddStyleToTable(excelPackage, 0, ExcelUtils.WsColor.BlueWhite);

                        using (var stream = File.Create(filePath))
                        {
                            excelPackage.SaveAs(stream);
                            stream.Close();
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
            var results = new List<Project>();
            try
            {
                var errors = new List<ErrorMessage>();

                try
                {
                    var token = await ThreeLeggedMananger.Instance.GetToken();

                    results = await ForgeHelpers.GetProjects(
                        accountId: hubId,
                        token: token,
                        errors: errors);
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
                        WorksheetWrapper<Project>? wsw = null;
                        WorksheetWrapper<ErrorMessage>? emw = null;
                        ExcelPackage? excelPackage = null;

                        if (!Directory.Exists(outDir))
                        {
                            Directory.CreateDirectory(outDir);
                        }

                        wsw = ExcelUtils.CreateWorksheet(results, "Projects");

                        if (wsw != null)
                        {
                            if (errors.Count > 0) wsw.NextWorksheet(errors, "Errors");
                            excelPackage = wsw.ToExcelPackage();
                        }
                        else
                        {
                            emw = ExcelUtils.CreateWorksheet(errors, "Errors");
                            excelPackage = wsw.ToExcelPackage();
                        }

                        ExcelUtils.AddStyleToTable(excelPackage, 0, ExcelUtils.WsColor.BlueWhite);

                        using (var stream = File.Create(filePath))
                        {
                            excelPackage.SaveAs(stream);
                            stream.Close();
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

        public static async Task<(List<SimpleFolder>, List<SimpleFile>)> GetFolderContent(
            ITokenManager tokenManager,
            string projectId,
            string folderId,
            bool foldersOnly,
            bool recurse,
            bool concurrent = false,
            BlockingCollection<ErrorMessage>? errors = null)
        {
            try
            {
                var (parentFolders, parentFiles) = await ForgeHelpers.GetFolderContents(
                            token: await tokenManager.GetToken(),
                            projectId: projectId,
                            folderId: folderId,
                            folderOnly: foldersOnly,
                            userId: ""
                           );

                if (recurse)
                {
                    foreach (var parentFolder in parentFolders)
                    {
                        var (childFolders, childFiles) = await GetFolderContent(
                            tokenManager: tokenManager,
                            projectId: projectId,
                            folderId: parentFolder.FolderId,
                            foldersOnly: foldersOnly,
                            recurse: recurse,
                            concurrent: concurrent,
                            errors: errors);

                        foreach (var folder in childFolders)
                        {
                            folder.ParentPath = parentFolder.Path;
                            folder.Path = Path.Combine(folder.ParentPath ?? "", folder.Name);
                            parentFolders.Add(folder);
                        }

                        if (!foldersOnly)
                        {
                            foreach (var file in childFiles)
                            {
                                file.ParentPath = parentFolder.Path;
                                file.Path = Path.Combine(file?.ParentPath ?? "Unknown", file?.Name ?? "Unknown");
                                parentFiles.Add(file!);
                            }
                        }
                    }
                }

                return (parentFolders, parentFiles);
            }
            catch (Exception ex)
            {
                if (errors != null)
                    errors.Add(new ErrorMessage($"Get Folder: {folderId}", ex.Message, ex.StackTrace ?? "No Stack"));
            }

            return (new List<SimpleFolder>(), new List<SimpleFile>());
        }

        public static async Task<(List<SimpleFolder>, List<SimpleFile>)> GetProjectContentWithOutput(
            ITokenManager tokenManger,
            string hubId,
            string projectId,
            bool foldersOnly,
            bool includeProjectName,
            string? filePath,
            BlockingCollection<ErrorMessage>? errors = null)
        {
            List<SimpleFolder> folderResults = new List<SimpleFolder>();
            List<SimpleFile> fileResults = new List<SimpleFile>();
            
            try
            {
                (folderResults, fileResults) =
                          await GetProjectContent(
                              tokenManger: tokenManger,
                              hubId: hubId,
                              projectId: projectId,
                              foldersOnly: foldersOnly,
                              includeProjectName: includeProjectName,
                              errors: errors);

                if (!string.IsNullOrWhiteSpace(filePath))
                {
                    var outDir = Path.GetDirectoryName(filePath);

                    if (!string.IsNullOrWhiteSpace(outDir))
                    {
                       
                        ExcelPackage? excelPackage = null;
                        var ef = ExcelUtils.CreateWorksheet(folderResults, "Folders");
                        var eff = foldersOnly ? null : ef.AddWorksheet(fileResults, "Files");
                        if (errors != null)
                        {
                            var err = eff == null ? ef.AddWorksheet(errors, "Errors") : eff.NextWorksheet(errors, "Errors");
                            excelPackage = err.ToExcelPackage();
                            ExcelUtils.AddStyleToTable(excelPackage!, 0, ExcelUtils.WsColor.BlueWhite);
                        }
                        var cnt = 0;
                        if (eff != null)
                        {
                            ExcelUtils.AddStyleToTable(excelPackage!, 1, ExcelUtils.WsColor.GreenWhite);
                            cnt = 1;
                        }
                        if (errors != null)
                        {
                            ExcelUtils.AddStyleToTable(excelPackage!, cnt + 1, ExcelUtils.WsColor.GrayWhite);
                        }

                        using (var stream = File.Create(filePath))
                        {
                            excelPackage!.SaveAs(stream);
                            stream.Close();
                        }
                    }
                }

                return (folderResults, fileResults);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{MethodBase.GetCurrentMethod()?.Name ?? "Unknown"}: {ex.Message}");
                throw;
            }
        }

        public static async Task<(List<SimpleFolder>, List<SimpleFile>)> GetProjectContent(
            ITokenManager tokenManger, 
            string hubId, 
            string projectId, 
            bool foldersOnly, 
            bool includeProjectName, 
            BlockingCollection<ErrorMessage>? errors)
        {
            var files = new List<SimpleFile>();
            var folders = new List<SimpleFolder>();

            try
            {
                var token = await tokenManger.GetToken();

                if (includeProjectName)
                {
                    var project = await ForgeHelpers.GetProject()
                }

                var topFolders = await ForgeHelpers.GetTopFolders(
                    accountId: hubId,
                    projectId: projectId,
                    token: token);

                var nameReplace = topFolders.Count < 2;

                //foreach (var topFolder in topFolders)
                //{
                    
                //    topFolder.ParentPath =  "";
                //    topFolder.Path = (includeProjectName) ? nameReplace ? pr "Unknown", topFolder.Name) : topFolder.Name;
                //    folders.Add(topFolder);
                //    var (childFolders, childFiles) = await GetFolderContent(
                //        tokenManager: tokenManger,
                //        projectId: projectId,
                //        folderId: topFolder.FolderId,
                //        foldersOnly: foldersOnly,
                //        recurse: true,
                //        errors: errors
                //        );
                //    foreach (var folder in childFolders)
                //    {
                //        folder.ParentPath = (root != null) ? Path.Combine(root!.Path ?? "Unknown", topFolder.Name) : topFolder.Name;
                //        folder.Path = Path.Combine(folder.ParentPath, folder.Name);
                //        folders.Add(folder);
                //    }

                //    if (!foldersOnly)
                //    {
                //        foreach (var file in childFiles)
                //        {
                //            file.ParentPath = (root != null) ? Path.Combine(root!.Path ?? "Unknown", topFolder.Name) : topFolder.Name;
                //            file.Path = Path.Combine(file.ParentPath, file.Name);
                //            files.Add(file);
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                if (errors != null)
                    errors.Add(new ErrorMessage("Error exporting projects", ex.Message, ""));
            }
            return (folders, files);
        }

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

        public static async Task GetContents(string hubId, string projectId)
        {
            try
            {
                var outDir = ExcelUtils.GetPath();
                var fileName = $"Project.Folders.{DateTime.Now.ToString("yy.MM.dd.mm.ss")}.xlsx";
                var path = Path.Combine(outDir!, fileName);
                var errors = new List<ErrorMessage>();
                WorksheetWrapper<Project>? wsw = null;
                WorksheetWrapper<ErrorMessage>? emw = null;
                ExcelPackage? excelPackage = null;

                try
                {
                    var token = await ThreeLeggedMananger.Instance.GetToken();
                    //  var token2 = await TwoLeggedManager.Instance.GetToken();
                    var projects = await ForgeHelpers.GetProjects(
                        accountId: hubId,
                        token: token,
                        errors: errors);

                    wsw = ExcelUtils.CreateWorksheet(projects, "Projects");
                }
                catch (Exception ex)
                {
                    errors.Add(new ErrorMessage("Error exporting projects", ex.Message, ""));
                }

                if (wsw != null)
                {
                    if (errors.Count > 0) wsw.NextWorksheet(errors, "Errors");
                    excelPackage = wsw.ToExcelPackage();
                }
                else
                {
                    emw = ExcelUtils.CreateWorksheet(errors, "Errors");
                    excelPackage = wsw.ToExcelPackage();
                }

                ExcelUtils.AddStyleToTable(excelPackage, 0, ExcelUtils.WsColor.BlueWhite);

                using (var stream = File.Create(path))
                {
                    excelPackage.SaveAs(stream);
                    stream.Close();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{MethodBase.GetCurrentMethod()?.Name ?? "Unknown"}: {ex.Message}");
                throw;
            }
        }
    }
}