using Ac.Net.Authentication;
using Ac.Net.Authentication.Models;
using ApsSettings.Data;
using Ganss.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using MigrationPlugin.Excel;
using MigrationPlugin.Models;
using MigrationPlugin.Plugin;
using Newtonsoft.Json;
using PluginBase.Models;

namespace MigrationPlugin.Controllers
{
    public class BatchDownloadController : Controller
    {
        private const string operation = "download";
        public const string PATH_TEMPLATE = "api/batch/" + operation + "/template";
        public const string PATH_LOAD = "api/batch/" + operation + "/load";

        [HttpGet]
        [Route(PATH_TEMPLATE)]
        public async Task<ActionResult> GetTemplate()
        {
            try
            {
                var fileName = $"Batch.Download.Template.{DateTime.Now.ToString("yy.MM.dd.mm.ss")}.xlsx";
                var path = Path.Combine(Path.GetTempPath(), fileName);
                var bytes = ExcelUtils.MockBatchDownloadXlsx();
                return File(bytes, "application/vnd.ms-excel");
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route(PATH_LOAD)]
        public async Task<ActionResult> UploadData([FromForm] IFormFile FileData)
        {
            try
            {
                string name = FileData.FileName;
                string extension = Path.GetExtension(FileData.FileName);

                using (var memoryStream = new MemoryStream())
                {
                    FileData.CopyTo(memoryStream);
                    var items = new ExcelMapper(memoryStream).Fetch<BatchDownload>().ToList();
                    var context = MigrationPluginObj.Instance.ServiceProvider.GetService<DataContext>()!;
                    foreach (var item in items)
                    {
                        var job = new BatchJob()
                        {
                            Name = item.Name,
                            Type = "download",
                            Data = JsonConvert.SerializeObject(item),
                        };
                        context.Batches.Add(job);
                    }
                    context.SaveChanges();
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
    }
}