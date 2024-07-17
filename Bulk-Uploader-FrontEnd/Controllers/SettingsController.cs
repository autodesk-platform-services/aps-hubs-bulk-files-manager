using ApsSettings.Data;
using ApsSettings.Data.Models;
using Bulk_Uploader_Electron.Utils;
using Hangfire.Storage.SQLite.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Ac.Net.Authentication;

namespace Bulk_Uploader_Electron.Controllers;

public class SettingsController: ControllerBase
{
    private readonly DataContext _dataContext;

    public SettingsController(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    [HttpGet]
    [Route("api/settings")]
    public async Task<ActionResult> GetSettings()
    {
        var settings = await _dataContext.Settings.ToListAsync();
        return Ok(settings);
    }

    [HttpPatch]
    [Route("api/settings")]
    public async Task<ActionResult> PatchSettings([FromBody] List<Setting> settings )
    {
        _dataContext.Settings.UpdateRange(settings);
        await _dataContext.SaveChangesAsync();
        
        var props = typeof(AppSettings).GetProperties().ToDictionary(x => x.Name, x => x);
        foreach (var item in settings)
        {
            if (props.ContainsKey(item.Key))
            {
                props[item.Key].SetValue(AppSettings.Instance, item.Value);
            }
        }
        return Ok();
    }

    [HttpPost]
    [Route("api/settings/batch")]
    public async Task<ActionResult> BatchPostSettings([FromBody] List<Setting> settings)
    {
        var posts = settings.Where(p => p.SettingId == 0);
        var patch = settings.Where(p => p.SettingId > 0);
        _dataContext.Settings.UpdateRange(patch);
        await _dataContext.Settings.AddRangeAsync(patch);
        await _dataContext.SaveChangesAsync();
        UpdateUsedApsCredentials();
        return Ok();
    }

    [HttpPost]
    [Route("api/settings")]
    public async Task<ActionResult> PostSettings([FromBody] List<Setting> settings )
    {
        await _dataContext.Settings.AddRangeAsync(settings);
        await _dataContext.SaveChangesAsync();
        
        var props = typeof(AppSettings).GetProperties().ToDictionary(x => x.Name, x => x);
        foreach (var item in settings)
        {
            if (props.ContainsKey(item.Key))
            {
                props[item.Key].SetValue(AppSettings.Instance, item.Value);
            }
        }
        return Created("api/settings", settings);
    }

    [HttpDelete]
    [Route("api/settings")]
    public async Task<ActionResult> DeleteSettings([FromBody] List<Setting> settings )
    {
        _dataContext.Settings.RemoveRange(settings);
        await _dataContext.SaveChangesAsync();
        return Ok();
    }

    public async void UpdateUsedApsCredentials()
    {
        var clientIdSetting = await _dataContext.Settings.Where(x => x.Key == "ClientId").FirstOrDefaultAsync();
        var clientSecretSetting = await _dataContext.Settings.Where(x => x.Key == "ClientSecret").FirstOrDefaultAsync();

        if (!string.IsNullOrEmpty(clientIdSetting?.Value) && !string.IsNullOrEmpty(clientSecretSetting?.Value))
        {
            AppSettings.Instance.ClientId = clientIdSetting.Value;
            AppSettings.Instance.ClientSecret = clientSecretSetting.Value;
        }
    }

    
    [HttpGet]
    [Route("api/settings/threeLegged/check")]
    public async Task<ActionResult> CheckThreeLegged()
    {
        return Ok(ThreeLeggedTokenManager.Instance.IsAuthenticated);
    }
    
    [HttpGet]
    [Route("api/settings/threeLegged/getAuthUrl")]
    public async Task<ActionResult> GetAuthUrl()
    {
        try
        {
            var url = ThreeLeggedTokenManager.Instance.AuthUrl();
            return Ok(url);
        }
        catch (Exception e)
        {
            return Ok("");
        }
    }
    
    [HttpPost]
    [Route("api/settings/threeLegged")]
    public async Task<ActionResult> UpdateThreeLegged([FromBody] CodeDTU code)
    {
        await ThreeLeggedTokenManager.Instance.GetTokenFromRequestCode(code.Code);
        return Ok();
    }
    
    [HttpDelete]
    [Route("api/settings/threeLegged")]
    public async Task<ActionResult> ClearThreeLegged()
    {
        ThreeLeggedTokenManager.Instance.Logout();
        return Ok();
    }

    [HttpGet]
    [Route("/code")]
    public async Task<IActionResult> Code([FromQuery] string code)
    {
        await ThreeLeggedTokenManager.Instance.GetTokenFromRequestCode(code);
        return new ContentResult 
        {
            ContentType = "text/html",
            Content = "<html><script>window.close()</script></html>"
        };
    }
    
}

public class CodeDTU
{
    public string Code { get; set; }
}