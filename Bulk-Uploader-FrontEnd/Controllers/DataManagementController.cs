
using Ac.Net.Authentication;
using Bulk_Uploader_Electron.Helpers;
using Bulk_Uploader_Electron.Managers;
using Bulk_Uploader_Electron.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace Bulk_Uploader_Electron.Controllers;

public class DataManagementController : ControllerBase
{
    public class SimpleDataManagementResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    [HttpGet]
    [Route("/api/dm/hubs")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<SimpleDataManagementResponse>))]
    public async Task<IActionResult> GetHubs()
    {
        var token = await JointTokenManager.GetToken();
        var accounts = await APSHelpers.GetHubs(token);
        var response = accounts.Select(x => new SimpleDataManagementResponse()
        {
            Id = x.AccountId,
            Name = x.Name
        })
            .OrderBy(x=>x.Name)
            .ToList();
        
        return Ok(response);
    }

    [HttpGet]
    [Route("/api/dm/projects")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<SimpleDataManagementResponse>))]
    public async Task<IActionResult> GetProjects([FromQuery] string hubId)
    {
       // var token = await TokenManager.GetTwoLeggedToken();
        var token = await JointTokenManager.GetToken();
        var accountProjects = await APSHelpers.GetHubProjects(hubId, token);

        var response = accountProjects.Select(x => new SimpleDataManagementResponse()
        {
            Id = x.ProjectId,
            Name = x.Name
        })
            .OrderBy(x=>x.Name)
            .ToList();

        return Ok(response);
    }

    [HttpGet]
    [Route("/api/dm/topFolders")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<SimpleDataManagementResponse>))]
    public async Task<IActionResult> GetTopFolders([FromQuery] string hubId, [FromQuery] string projectId)
    {
       // var token = await TokenManager.GetTwoLeggedToken();
        var token = await JointTokenManager.GetToken();
        var topFolders = await APSHelpers.GetTopFolders(token, hubId, projectId);
        
        var response = topFolders.Select(x => new SimpleDataManagementResponse()
        {
            Id = x.FolderId,
            Name = x.Name
        })
            .OrderBy(x=>x.Name)
            .ToList();

        return Ok(response);
    }

    [HttpGet]
    [Route("/api/dm/folderContent")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<SimpleDataManagementResponse>))]
    public async Task<IActionResult> GetFolderContents([FromQuery] string projectId, [FromQuery] string folderId)
    {
        //var token = await TokenManager.GetTwoLeggedToken();
        var token = await JointTokenManager.GetToken();
        (var folders , var files ) = await APSHelpers.GetFolderContents(token, projectId, folderId);

        var response = folders.Select(x => new SimpleDataManagementResponse()
        {
            Id = x.FolderId,
            Name = x.Name
        })
            .OrderBy(x=>x.Name)
            .ToList();

        return Ok(response.Count > 0 ? response : new List<SimpleDataManagementResponse>());
    }
}