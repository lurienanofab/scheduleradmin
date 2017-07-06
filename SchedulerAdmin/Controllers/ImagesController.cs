using SchedulerAdmin.Models;
using System.Web.Mvc;

namespace SchedulerAdmin.Controllers
{
    public class ImagesController : Controller
    {
        [Route("images/{path}/{id}")]
        public ActionResult GetImage(string path, string id)
        {
            string filePath = UploadFileUtility.GetImagePhysicalPath(path, id);
            byte[] fileContents = System.IO.File.ReadAllBytes(filePath);
            return File(fileContents, "image/png");
        }

        [Route("images/{path}/{id}/icon")]
        public ActionResult GetIcon(string path, string id)
        {
            string filePath = UploadFileUtility.GetIconPhysicalPath(path, id);
            byte[] fileContents = System.IO.File.ReadAllBytes(filePath);
            return File(fileContents, "image/png");
        }
    }
}