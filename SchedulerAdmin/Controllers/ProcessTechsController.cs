using LNF.Models.Scheduler;
using LNF.Repository;
using LNF.Repository.Scheduler;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SchedulerAdmin.Controllers
{
    public class ProcessTechsController : Controller
    {
        [Route("proctechs")]
        public ActionResult Index()
        {
            ViewBag.ActiveTab = "proctechs";

            var model = DA.Current.Query<ProcessTech>().Model<ProcessTechModel>()
                .OrderByDescending(x => x.ProcessTechIsActive)
                .ThenBy(x => x.BuildingName)
                .ThenBy(x => x.LabDisplayName)
                .ThenBy(x => x.ProcessTechName)
                .ThenBy(x => x.ProcessTechID);

            return View(model);
        }

        [Route("proctechs/{processTechId}")]
        public ActionResult Edit(int processTechId)
        {
            ViewBag.ActiveTab = "proctechs";

            ViewBag.Errors = new List<string>();
            ViewBag.Message = string.Empty;

            ViewBag.Groups = DA.Current.Query<ProcessTechGroup>().Model<ProcessTechGroupModel>().OrderBy(x => x.GroupName).ToList();
            ViewBag.Labs = DA.Current.Query<Lab>().Where(x => x.IsActive).Model<LabModel>().OrderBy(x => x.BuildingName).ThenBy(x => x.LabDisplayName).ToList();

            ProcessTechModel model;

            if (processTechId == 0)
                model = new ProcessTechModel() { ProcessTechIsActive = true };
            else
                model = DA.Current.Single<ProcessTech>(processTechId).Model<ProcessTechModel>();

            return View(model);
        }

        [HttpPost, Route("proctechs/{processTechId}")]
        public ActionResult Edit(ProcessTechModel model)
        {
            ViewBag.ActiveTab = "proctechs";

            List<string> errors = new List<string>();

            if (string.IsNullOrEmpty(model.ProcessTechName))
                errors.Add("Name is required.");

            ViewBag.Errors = errors;
            ViewBag.Message = string.Empty;

            ViewBag.Groups = DA.Current.Query<ProcessTechGroup>().Model<ProcessTechGroupModel>().OrderBy(x => x.GroupName).ToList();
            ViewBag.Labs = DA.Current.Query<Lab>().Where(x => x.IsActive).Model<LabModel>().OrderBy(x => x.BuildingName).ThenBy(x => x.LabDisplayName).ToList();

            if (errors.Count > 0)
                return View(model);

            ProcessTech pt;

            if (model.ProcessTechID == 0)
            {
                pt = new ProcessTech()
                {
                    Group = DA.Current.Single<ProcessTechGroup>(model.GroupID),
                    Lab = DA.Current.Single<Lab>(model.LabID),
                    ProcessTechName = model.ProcessTechName,
                    Description = model.ProcessTechDescription,
                    IsActive = model.ProcessTechIsActive
                };

                DA.Current.Insert(pt);

                model.ProcessTechID = pt.ProcessTechID;

                ViewBag.Message = "The new process tech has been added.";
            }
            else
            {
                pt = DA.Current.Single<ProcessTech>(model.ProcessTechID);
                pt.Group = DA.Current.Single<ProcessTechGroup>(model.GroupID);
                pt.Lab = DA.Current.Single<Lab>(model.LabID);
                pt.ProcessTechName = model.ProcessTechName;
                pt.Description = model.ProcessTechDescription;
                pt.IsActive = model.ProcessTechIsActive;

                ViewBag.Message = "The process tech has been modified.";
            }

            string command = Request.Form["Command"];

            if (command == "SaveAndReturn")
                return RedirectToAction("Index", "ProcessTechs");
            else
                return View(model);

        }

        [Route("proctechs/groups/{groupId?}")]
        public ActionResult ManageGroups(int? groupId = null)
        {
            ViewBag.ActiveTab = "proctechs";

            var model = DA.Current.Query<ProcessTechGroup>().Model<ProcessTechGroupModel>().OrderBy(x => x.GroupName);

            ViewBag.GroupID = groupId;

            return View(model);
        }

        [HttpPost, Route("proctechs/groups/{groupId}/save")]
        public ActionResult ManageGroupsSave(int? groupId = null)
        {
            string groupName;

            string command = Request.Form["command"];

            if (command == "add")
            {
                groupName = Request.Form["add_group_name"];
                if (!string.IsNullOrEmpty(groupName))
                    DA.Current.Insert(new ProcessTechGroup() { GroupName = groupName });
            }
            else if (command == "modify")
            {
                groupName = Request.Form["modify_group_name"];
                var grp = DA.Current.Single<ProcessTechGroup>(groupId.GetValueOrDefault(0));
                if (grp != null && !string.IsNullOrEmpty(groupName))
                    grp.GroupName = groupName;
            }

            return RedirectToAction("ManageGroups", new { groupId = default(int?) });
        }
    }
}