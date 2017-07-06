using LNF.Models.Scheduler;
using LNF.Repository;
using LNF.Repository.Scheduler;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SchedulerAdmin.Controllers
{
    public class BuildingsController : Controller
    {
        [Route("buildings")]
        public ActionResult Index()
        {
            ViewBag.ActiveTab = "buildings";

            var query = DA.Current.Query<Building>()
                .OrderByDescending(x => x.IsActive)
                .ThenBy(x => x.BuildingName)
                .ThenBy(x => x.BuildingID);

            var model = query.Model<BuildingModel>();

            return View(model);
        }

        [Route("buildings/{buildingId}")]
        public ActionResult Edit(int buildingId)
        {
            ViewBag.ActiveTab = "buildings";

            ViewBag.Errors = new List<string>();
            ViewBag.Message = string.Empty;

            BuildingModel model;

            if (buildingId == 0)
                model = new BuildingModel() { BuildingIsActive = true };
            else
                model = DA.Current.Single<Building>(buildingId).Model<BuildingModel>();

            return View(model);
        }

        [HttpPost, Route("buildings/{buildingId}")]
        public ActionResult Edit(BuildingModel model)
        {
            ViewBag.ActiveTab = "buildings";

            List<string> errors = new List<string>();

            if (string.IsNullOrEmpty(model.BuildingName))
                errors.Add("Name is required.");

            ViewBag.Errors = errors;
            ViewBag.Message = string.Empty;

            if (errors.Count > 0)
                return View(model);

            Building bldg;

            if (model.BuildingID == 0)
            {
                bldg = new Building()
                {
                    BuildingName = model.BuildingName,
                    Description = model.BuildingDescription,
                    IsActive = model.BuildingIsActive
                };

                DA.Current.Insert(bldg);

                model.BuildingID = bldg.BuildingID;

                ViewBag.Message = "The new building has been added.";
            }
            else
            {
                bldg = DA.Current.Single<Building>(model.BuildingID);
                bldg.BuildingName = model.BuildingName;
                bldg.Description = model.BuildingDescription;
                bldg.IsActive = model.BuildingIsActive;

                ViewBag.Message = "The building has been modified.";
            }

            string command = Request.Form["Command"];

            if (command == "SaveAndReturn")
                return RedirectToAction("Index", "Buildings");
            else
                return View(model);

        }
    }
}