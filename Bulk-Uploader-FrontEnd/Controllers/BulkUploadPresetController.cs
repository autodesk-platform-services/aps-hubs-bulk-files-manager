using ApsSettings.Data;
using ApsSettings.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bulk_Uploader_Electron.Controllers;

public class BulkUploadPresetController: ControllerBase
{
    private readonly DataContext _dataContext;

    public BulkUploadPresetController(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    [HttpGet]
    [Route("api/settings/bulkUploadPresets")]
    public async Task<ActionResult> GetSettings()
    {
        var presets = await _dataContext.BulkUploadPresets.ToListAsync();
        return Ok(presets);
    }

    [HttpPatch]
    [Route("api/setting/bulkUploadPresets")]
    public async Task<ActionResult> PatchSettings([FromBody] List<BulkUploadPreset> presets )
    {
        _dataContext.BulkUploadPresets.UpdateRange(presets);
        await _dataContext.SaveChangesAsync();
        return Ok();
    }

    [HttpPost]
    [Route("api/settings/bulkUploadPresets")]
    public async Task<ActionResult> PostSettings([FromBody] List<BulkUploadPreset> presets )
    {
        await _dataContext.BulkUploadPresets.AddRangeAsync(presets);
        await _dataContext.SaveChangesAsync();
        return Created("api/settings/bulkUploadPresets", presets);
    }

    [HttpDelete]
    [Route("api/settings/bulkUploadPresets/{id}")]
    public async Task<ActionResult> DeleteSettings(int id)
    {
        var preset = await _dataContext.BulkUploadPresets.FirstOrDefaultAsync(x => x.Id == id);
        if (preset == null) return NotFound();
        
        _dataContext.BulkUploadPresets.Remove(preset);
        await _dataContext.SaveChangesAsync();
        return NoContent();
    }
}