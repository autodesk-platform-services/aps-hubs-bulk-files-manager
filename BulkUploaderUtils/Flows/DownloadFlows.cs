using Ac.Net.Authentication.Models;
using BulkUploaderUtils.Models;
using Data.Models;
using Data.Utilities;
using Microsoft.EntityFrameworkCore.Query;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Flurl;
using Flurl.Http;
using System.Collections;
using BulkUploaderUtils.Excel;
using OfficeOpenXml;
using EPPlus.Core.Extensions;

namespace BulkUploaderUtils.Flows
{
    public class DownloadFlows
    {
        public static async Task<(List<SimpleFolder>, List<SimpleFile>)> DownloadProjecct(
             ITokenManager tokenManger,
             string hubId,
             string projectId,
             string outputDirectory,
             string? filePath,
             BlockingCollection<ErrorMessage>? errors = null)
        {
            List<SimpleFolder> folderResults = new List<SimpleFolder>();
            List<SimpleFile> fileResults = new List<SimpleFile>();

            try
            {
                (folderResults, fileResults) = await UtilityFlows.GetProjectContent(
                              tokenManger: tokenManger,
                              hubId: hubId,
                              projectId: projectId,
                              foldersOnly: false,
                              includeProjectName: true,
                              errors: errors);

                var folderList = new Dictionary<string, SimpleFolder>();

                foreach (var simpleFolder in folderResults)
                {
                    if (string.IsNullOrEmpty(simpleFolder.Path) || simpleFolder.Path == "Unknown")
                    {
                        continue;
                    }
                    var pathsegments = simpleFolder.Path.Split(Path.DirectorySeparatorChar);
                    var current = simpleFolder.Path.ToUpper().Trim();
                    if (!folderList.ContainsKey(current))
                    {
                        folderList[current] = simpleFolder;
                    }
                }

                var root = new DirectoryInfo(outputDirectory);
                if (!root.Exists)
                {                   
                    root.Create();
                }

                foreach (var key in folderList.Keys.ToList())
                {
                    var fi = new DirectoryInfo(Path.Combine(outputDirectory, key));
                    if (!fi.Exists)
                    {
                        fi.Create();
                    }
                }

                foreach (var item in fileResults)
                {
                    item.Status = "Started";
                    try
                    {
                        var parts = item.ObjectId.Split(":");
                        var p = parts[parts.Length - 1];
                        parts = p.Split("/");
                        if (parts.Length != 2)
                        {
                            var ex = new Exception("Item Id Not the Expected Type");
                            if (errors != null)
                            {
                                errors.Add(new ErrorMessage("Error geting s3 url", ex));
                            }
                            throw ex;
                        }
                        var timeout = item.Size > 1000000000 ? 6 : item.Size > 500000000 ? 4 : item.Size > 100000000 ? 3 : 2;
                        var signedUrl = await ForgeHelpers.GetDownloadUrl(await tokenManger.GetToken(), parts[0], parts[1], timeout);
                        var path = Path.Combine(outputDirectory, item.Path!);
                        var stream = await ForgeHelpers.GetDownloadStream(await tokenManger.GetToken(), item.ObjectId);

                        byte[]? buffer;
                        using (var memoryStream = new MemoryStream())
                        {
                            stream.CopyTo(memoryStream);
                            buffer = memoryStream.ToArray();
                        }
                        //stream.Write(buffer, 0, (int)stream.Length);

                        using (FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
                        {
                            fileStream.Write(buffer, 0, buffer.Length);
                        }

                        item.Status = "Downloaded";
                    }
                    catch (Exception ex)
                    {
                        item.Status = "Error";
                        if (errors != null)
                            errors.Add(new ErrorMessage($"Error downloading {item.Name} {item.ItemId}", ex)); ;
                    }
                    

                   
                }



                

            }
            catch (Exception ex)
            {
                if (errors != null)
                    errors.Add(new ErrorMessage("Error exporting projects", ex.Message, ""));
            }

            if (!string.IsNullOrWhiteSpace(filePath))
            {
                var outDir = Path.GetDirectoryName(filePath);

                if (!string.IsNullOrWhiteSpace(outDir))
                {
                    WorksheetWrapper<SimpleFile>? wsw = null;
                    WorksheetWrapper<ErrorMessage>? emw = null;
                    ExcelPackage? excelPackage = null;

                    if (!Directory.Exists(outDir))
                    {
                        Directory.CreateDirectory(outDir);
                    }

                    wsw = ExcelUtils.CreateWorksheet(fileResults, "Downloads");

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

            return (folderResults, fileResults);

            //var ef = ExcelUtils.CreateWorksheet(folderResults, "Folders");
            //var eff = foldersOnly ? null : ef.AddWorksheet(fileResults, "Files");
            //if (errors != null)
            //{
            //    var err = eff == null ? ef.AddWorksheet(errors, "Errors") : eff.NextWorksheet(errors, "Errors");
            //    excelPackage = err.ToExcelPackage();
            //    ExcelUtils.AddStyleToTable(excelPackage!, 0, ExcelUtils.WsColor.BlueWhite);

            //}
            //var cnt = 0;
            //if (eff != null)
            //{
            //    ExcelUtils.AddStyleToTable(excelPackage!, 1, ExcelUtils.WsColor.GreenWhite);
            //    cnt = 1;
            //}
            //if (errors != null)
            //{
            //    ExcelUtils.AddStyleToTable(excelPackage!, cnt + 1, ExcelUtils.WsColor.GrayWhite);
            //}

            //using (var stream = File.Create(path))
            //{
            //    excelPackage!.SaveAs(stream);
            //    stream.Close();
            //}

        }
    }
}