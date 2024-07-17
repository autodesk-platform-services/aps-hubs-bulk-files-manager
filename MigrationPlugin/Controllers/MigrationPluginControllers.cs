//using Ac.Net.Authentication;
//using Ac.Net.Authentication.Models;

//using Flurl.Http;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using MigrationPlugin.Excel;
//using System.IO;
//using System.Text;

//namespace MigrationPlugin.Controllers
//{
//    public class MIgrationPluginController : Controller
//    {
//        public IActionResult Index()
//        {
//            return View();
//        }

//        [HttpGet]
//        [Route("api/templates/download")]
//        public async Task<ActionResult> getDownloadTemplate()
//        {
//            try
//            {
//                ITokenManager tm = ThreeLeggedMananger.Instance;

//                var fileName = $"Batch.Download.Template.{DateTime.Now.ToString("yy.MM.dd.mm.ss")}.xlsx";
//                var path = Path.Combine(Path.GetTempPath(), fileName);

//              //  ExcelUtils.MockBatchDownladXlsx(path);

//                return File(new FileStream(path, FileMode.Open), "application/vnd.ms-excel");

//            }
//            catch (Exception ex)
//            {
//                return BadRequest();
//            }
//        }

//        [HttpPost]
//        [Route("api/batch/download")]
//        public async Task<ActionResult> LoadDownloadTemplate([FromForm] IFormFile FileData)
//        {

//            try
//            {
//                string name = FileData.FileName;
//                string extension = Path.GetExtension(FileData.FileName);

//                using (var memoryStream = new MemoryStream())
//                {
//                    FileData.CopyTo(memoryStream);
//            //        var items = ExcelUtils.LoadBatchDownloadXlsx(memoryStream);

//                    //await BatchFlows.ProcessDownloadBatch(ThreeLeggedMananger.Instance, DataContext context, items);
//                }

//                return Ok();

//            }
//            catch (Exception ex)
//            {
//                return BadRequest();
//            }
//        }



//        [HttpGet]
//        [Route("api/templates/upload")]
//        public async Task<ActionResult> getUploadTemplate()
//        {
//            try
//            {
//                ITokenManager tm = ThreeLeggedMananger.Instance;

//                var fileName = $"Batch.Upload.Template.{DateTime.Now.ToString("yy.MM.dd.mm.ss")}.xlsx";
//                var path = Path.Combine(Path.GetTempPath(), fileName);

//           //     ExcelUtils.MockBatchUploadXlsx(path);

//                return File(new FileStream(path, FileMode.Open), "application/vnd.ms-excel");

//            }
//            catch (Exception ex)
//            {
//                return BadRequest();
//            }
//        }

//        [HttpGet]
//        [Route("api/templates/migrate")]
//        public async Task<ActionResult> getMigrateTemplate()
//        {
//            try
//            {
//                ITokenManager tm = ThreeLeggedMananger.Instance;

//                var fileName = $"Batch.Migrate.Template.{DateTime.Now.ToString("yy.MM.dd.mm.ss")}.xlsx";
//                var path = Path.Combine(Path.GetTempPath(), fileName);

//         //       ExcelUtils.MockBatchDownladXlsx(path);

//                return File(new FileStream(path, FileMode.Open), "application/vnd.ms-excel");

//            }
//            catch (Exception ex)
//            {
//                return BadRequest();
//            }
//        }
//    }
//}



