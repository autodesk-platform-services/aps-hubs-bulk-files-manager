using ApsSettings.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ClearScript.V8;

namespace Bulk_Uploader_Electron.Controllers;

public class ModifyPathDTU
{
    public BulkUpload BulkUpload { get; set; }
    public BulkUploadFile BulkUploadFile { get; set; }
}

public class ScriptingController: ControllerBase
{
    [HttpPost]
    [Route("api/scripting/modifyPath")]
    public ActionResult<BulkUploadFile> ModifyPath([FromBody] ModifyPathDTU dtu)
    {
        try
        {
            var newFile = BulkUploadFile.CreateFile(
                dtu.BulkUpload, 
                dtu.BulkUploadFile.SourceRelativePath,
                dtu.BulkUploadFile.SourceAbsolutePath
                );
            
            return Ok(newFile);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}