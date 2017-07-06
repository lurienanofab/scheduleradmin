using LNF.Models.Data;
using LNF.Models.Scheduler;
using LNF.Repository;
using LNF.Repository.Data;
using LNF.Repository.Scheduler;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SchedulerAdmin.Controllers
{
    public class LabsController : Controller
    {
        [Route("labs")]
        public ActionResult Index()
        {
            ViewBag.ActiveTab = "labs";

            var model = DA.Current.Query<Lab>().Model<LabModel>()
                .OrderByDescending(x => x.LabIsActive)
                .ThenBy(x => x.BuildingName)
                .ThenBy(x => x.LabDisplayName)
                .ThenBy(x => x.LabID);

            return View(model);
        }

        [Route("labs/{labId}")]
        public ActionResult Edit(int labId)
        {
            ViewBag.ActiveTab = "labs";

            ViewBag.Errors = new List<string>();
            ViewBag.Message = string.Empty;

            ViewBag.Buildings = DA.Current.Query<Building>().Where(x => x.IsActive).Model<BuildingModel>().OrderBy(x => x.BuildingName).ToList();
            ViewBag.Rooms = DA.Current.Query<Room>().Where(x => x.Active).Model<RoomModel>().OrderBy(x => x.RoomDisplayName).ToList();

            LabModel model;

            if (labId == 0)
                model = new LabModel() { LabIsActive = true };
            else
                model = DA.Current.Single<Lab>(labId).Model<LabModel>();

            return View(model);
        }

        [HttpPost, Route("labs/{labId}")]
        public ActionResult Edit(LabModel model)
        {
            ViewBag.ActiveTab = "labs";

            List<string> errors = new List<string>();

            if (string.IsNullOrEmpty(model.LabName))
                errors.Add("Name is required.");

            if (string.IsNullOrEmpty(model.LabDisplayName))
                errors.Add("Display Name is required.");

            ViewBag.Errors = errors;
            ViewBag.Message = string.Empty;

            ViewBag.Buildings = DA.Current.Query<Building>().Where(x => x.IsActive).Model<BuildingModel>().OrderBy(x => x.BuildingName).ToList();
            ViewBag.Rooms = DA.Current.Query<Room>().Where(x => x.Active).Model<RoomModel>().OrderBy(x => x.RoomDisplayName).ToList();

            if (errors.Count > 0)
                return View(model);

            Lab lab;

            if (model.LabID == 0)
            {
                lab = new Lab()
                {
                    LabName = model.LabName,
                    Description = model.LabDescription,
                    IsActive = model.LabIsActive,
                    Building = DA.Current.Single<Building>(model.BuildingID),
                    Room = DA.Current.Single<Room>(model.RoomID)
                };

                DA.Current.Insert(lab);

                model.LabID = lab.LabID;

                ViewBag.Message = "The new lab has been added.";
            }
            else
            {
                lab = DA.Current.Single<Lab>(model.LabID);
                lab.LabName = model.LabName;
                lab.DisplayName = model.LabDisplayName;
                lab.Description = model.LabDescription;
                lab.Building = DA.Current.Single<Building>(model.BuildingID);
                lab.Room = DA.Current.Single<Room>(model.RoomID);
                lab.IsActive = model.LabIsActive;
                
                ViewBag.Message = "The lab has been modified.";
            }

            string command = Request.Form["Command"];

            if (command == "SaveAndReturn")
                return RedirectToAction("Index", "Labs");
            else
                return View(model);

        }
    }
}