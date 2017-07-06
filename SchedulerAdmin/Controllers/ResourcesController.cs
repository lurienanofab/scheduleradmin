using LNF.Models.Scheduler;
using LNF.Repository;
using LNF.Repository.Scheduler;
using SchedulerAdmin.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SchedulerAdmin.Controllers
{
    public class ResourcesController : Controller
    {
        [Route("resources")]
        public ActionResult Index()
        {
            ViewBag.ActiveTab = "resources";

            var model = DA.Current.Query<Resource>().Model<ResourceModel>()
                .OrderByDescending(x => x.ResourceIsActive)
                .ThenBy(x => x.BuildingName)
                .ThenBy(x => x.LabDisplayName)
                .ThenBy(x => x.ProcessTechName)
                .ThenBy(x => x.ResourceName)
                .ThenBy(x => x.ResourceID);

            return View(model);
        }

        [Route("resources/{resourceId}")]
        public ActionResult Edit(int resourceId)
        {
            ViewBag.ActiveTab = "resources";

            ViewBag.Errors = new List<string>();
            ViewBag.Message = string.Empty;
            ViewBag.IconUrl = UploadFileUtility.GetIconURL("resource", resourceId.ToString("000000"));

            ViewBag.UploadImageError = string.Empty;

            if (Session["UploadImageError"] != null)
                ViewBag.UploadImageError = Session["UploadImageError"].ToString();

            return View(resourceId);
        }

        [HttpPost, Route("resources/{resourceId}/image")]
        public ActionResult UploadImage(int resourceId)
        {
            Session.Remove("UploadImageError");

            if (Request.Files.Count == 0)
            {
                Session["UploadImageError"] = "Please select a file.";
            }
            else
            {
                foreach (string upload in Request.Files)
                {
                    try
                    {
                        HttpPostedFileBase file = Request.Files[upload];
                        UploadFileUtility.UploadImage(file, "resource", resourceId.ToString("000000"));
                    }
                    catch(Exception ex)
                    {
                        Path.GetExtension("");
                        Session["UploadImageError"] = ex.Message;
                        break;
                    }
                }
            }

            return RedirectToAction("Edit", "Resources", new { resourceId });
        }

        [HttpGet, Route("resources/{resourceId}/image/delete")]
        public ActionResult DeleteImage(int resourceId)
        {
            UploadFileUtility.DeleteImages("resource", resourceId.ToString("000000"));
            return RedirectToAction("Edit", "Resources", new { resourceId });
        }
    }
}