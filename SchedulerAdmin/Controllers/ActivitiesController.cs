using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LNF;
using LNF.Repository;
using LNF.Repository.Scheduler;
using LNF.Models.Scheduler;

namespace SchedulerAdmin.Controllers
{
    public class ActivitiesController : Controller
    {
        [Route("activities")]
        public ActionResult Index()
        {
            ViewBag.ActiveTab = "activities";

            var query = DA.Current.Query<Activity>()
                .OrderByDescending(x => x.IsActive)
                .ThenBy(x => x.ListOrder)
                .ThenBy(x => x.ActivityName)
                .ThenBy(x => x.ActivityID);

            var model = query.Model<ActivityModel>();

            return View(model);
        }

        [Route("activities/{activityId}")]
        public ActionResult Edit(int activityId)
        {
            ViewBag.ActiveTab = "activities";

            ViewBag.AuthLevels = DA.Current.Query<AuthLevel>().OrderBy(x => x.AuthLevelID).ToList();

            ViewBag.Errors = new List<string>();
            ViewBag.Message = string.Empty;

            ActivityModel model;

            if (activityId == 0)
                model = new ActivityModel() { IsActive = true };
            else
                model = DA.Current.Single<Activity>(activityId).Model<ActivityModel>();

            return View(model);
        }

        [HttpPost, Route("activities/{activityId}")]
        public ActionResult Edit(ActivityModel model)
        {
            ViewBag.ActiveTab = "activities";

            ViewBag.AuthLevels = DA.Current.Query<AuthLevel>().OrderBy(x => x.AuthLevelID).ToList();

            List<string> errors = new List<string>();

            if (string.IsNullOrEmpty(model.ActivityName))
                errors.Add("Name is required.");

            ViewBag.Errors = errors;
            ViewBag.Message = string.Empty;

            if (errors.Count > 0)
                return View(model);

            Activity act;

            if (model.ActivityID == 0)
            {
                act = new Activity()
                {
                    ActivityName = model.ActivityName,
                    Description = model.Description,
                    ListOrder = model.ListOrder,
                    Chargeable = model.Chargeable,
                    Editable = model.Editable,
                    AccountType = model.AccountType,
                    UserAuth = model.UserAuth,
                    InviteeType = model.InviteeType,
                    InviteeAuth = model.InviteeAuth,
                    StartEndAuth = model.StartEndAuth,
                    NoReservFenceAuth = model.NoReservFenceAuth,
                    NoMaxSchedAuth = model.NoMaxSchedAuth,
                    IsFacilityDownTime = model.IsFacilityDownTime,
                    IsActive = model.IsActive
                };

                DA.Current.Insert(act);

                model.ActivityID = act.ActivityID;

                ViewBag.Message = "The new activity has been added.";
            }
            else
            {
                act = DA.Current.Single<Activity>(model.ActivityID);
                act.ActivityName = model.ActivityName;
                act.Description = model.Description;
                act.ListOrder = model.ListOrder;
                act.Chargeable = model.Chargeable;
                act.Editable = model.Editable;
                act.AccountType = model.AccountType;
                act.UserAuth = model.UserAuth;
                act.InviteeType = model.InviteeType;
                act.InviteeAuth = model.InviteeAuth;
                act.StartEndAuth = model.StartEndAuth;
                act.NoReservFenceAuth = model.NoReservFenceAuth;
                act.NoMaxSchedAuth = model.NoMaxSchedAuth;
                act.IsFacilityDownTime = model.IsFacilityDownTime;
                act.IsActive = model.IsActive;

                ViewBag.Message = "The activity has been modified.";
            }

            string command = Request.Form["Command"];

            if (command == "SaveAndReturn")
                return RedirectToAction("Index", "Activities");
            else
                return View(model);
            
        }
    }
}