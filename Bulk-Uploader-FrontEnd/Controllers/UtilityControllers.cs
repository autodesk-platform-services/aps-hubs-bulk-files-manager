using Ac.Net.Authentication.Models;
using Ac.Net.Authentication;
using ApsSettings.Data;
using ApsSettings.Data.Models;
using Microsoft.AspNetCore.Mvc;
using MigrationPlugin.Excel;
using Bulk_Uploader_Electron.Flows;
using Bulk_Uploader_Electron.Helpers;
using Bulk_Uploader_Electron.Jobs;
using Bulk_Uploader_Electron.Models;
using Bulk_Uploader_Electron.Utilities;
using Hangfire;

namespace Bulk_Uploader_Electron.Controllers;

public class AccountsProjectsMapping
{
    public string AccountName { get; set; }
    public string AccountId { get; set; }
    public string ProjectName { get; set; }
    public  ProjectType ProjectType{ get; set; }
    public string ProjectId { get; set; }
   // public string MigrateToProjectId { get; set; }
}
public class UtilityControllers : ControllerBase
{
    [HttpGet]
    [Route("api/hubs")]
    public async Task<ActionResult> GetHub()
    {
        try
        {
            var outPath = Path.Combine(Path.GetTempPath(), $"Hubs.{DateTime.Now.ToString("yy.MM.dd.mm.ss")}.xlsx");
            var results = await UtilityFlows.GetHubs(outPath);
            var fileName = Path.GetFileName(outPath);
            return File(new FileStream(outPath, FileMode.Open), "application/vnd.ms-excel");
        }
        catch (Exception ex)
        {
            return BadRequest();
        }
    }
    [HttpGet]
    [Route("api/hubs/{hubId}/projects")]
    public async Task<ActionResult> GetProjects(string hubId)
    {
        try
        {
            var outPath = Path.Combine(Path.GetTempPath(), $"Projects.{hubId}.{DateTime.Now.ToString("yy.MM.dd.mm.ss")}.xlsx");
            var results = await UtilityFlows.GetProjects(hubId, outPath);
            return File(new FileStream(outPath, FileMode.Open), "application/vnd.ms-excel");
        }
        catch (Exception ex)
        {
            return BadRequest();
        }
    }
    [HttpGet]
    [Route("api/hubs/{hubId}/projects/{projectId}/folders")]
    public async Task<ActionResult> getFolders(string hubId, string projectId)
    {
        try
        {
            var outPath = Path.Combine(Path.GetTempPath(), $"Folders.{hubId}.{projectId}.{DateTime.Now.ToString("yy.MM.dd.mm.ss")}.xlsx");
            var (allFolders, allFiles) = await UtilityFlows.GetProjectData(projectId, hubId);
            await UtilityFlows.SaveAsExcel(allFolders, outPath, "Folders");
            return File(new FileStream(outPath, FileMode.Open), "application/vnd.ms-excel");
        }
        catch (Exception ex)
        {
            return BadRequest();
        }
    }
    [HttpGet]
    [Route("api/hubs/{hubId}/projects/{projectId}/content")]
    public async Task<ActionResult> getContent(string hubId, string projectId)
    {
        try
        {
            var outPath = Path.Combine(Path.GetTempPath(), $"Content.{hubId}.{projectId}.{DateTime.Now.ToString("yy.MM.dd.mm.ss")}.xlsx");
            var (allFolders, allFiles) = await UtilityFlows.GetProjectData(projectId, hubId);
            await UtilityFlows.SaveAsExcel(allFiles, outPath, "Files");
            return File(new FileStream(outPath, FileMode.Open), "application/vnd.ms-excel");
        }
        catch (Exception ex)
        {
            return BadRequest();
        }
    }

    [HttpGet]
    [Route("/api/utilities/all")]
    public async Task<ActionResult> GetAllAccountsAndProjects()
    {
        var output = new List<AccountsProjectsMapping>();
        var token = await JointTokenManager.GetToken();
        var accounts = await APSHelpers.GetHubs(token);
        foreach (var account in accounts)
        {
            var accountProjects = await APSHelpers.GetHubProjects(account.AccountId, token);
            accountProjects.ForEach(project =>
            {
                output.Add(new AccountsProjectsMapping()
                {
                    AccountId = account.AccountId,
                    AccountName = account.Name,
                    ProjectName = project.Name,
                    ProjectType = project.ProjectType,
                    ProjectId = project.ProjectId,
                 //   MigrateToProjectId = "",
                });
            });
        }

        var outPath = Path.Combine(Path.GetTempPath(), "BulkFileManager_AllAccountsAndProjects.xlsx");
        await UtilityFlows.SaveAsExcel(output, outPath, "Mappings");
        return File(new FileStream(outPath, FileMode.Open), "application/vnd.ms-excel");
    }
}
