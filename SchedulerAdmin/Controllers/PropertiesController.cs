using LNF.Repository;
using LNF.Repository.Data;
using LNF.Scheduler;
using SchedulerAdmin.Models;
using System.Linq;
using System.Web.Mvc;

namespace SchedulerAdmin.Controllers
{
    public class PropertiesController : Controller
    {
        [Route("props")]
        public ActionResult Index()
        {
            ViewBag.ActiveTab = "properties";

            ViewBag.Clients = DA.Current.Query<Client>().Where(x => x.Active).OrderBy(x => x.LName).ThenBy(x => x.FName).ToList().Select(x => new SelectListItem() { Value = x.ClientID.ToString(), Text = x.DisplayName });
            ViewBag.Accounts = DA.Current.Query<Account>().Where(x => x.Active).OrderBy(x => x.Name).ToList().Select(x => new SelectListItem() { Value = x.AccountID.ToString(), Text = x.Name });

            var props = Properties.Current;

            var model = new PropertiesModel()
            {
                LateChargePenaltyMultiplier = props.LateChargePenaltyMultiplier,
                AuthorizationExpirationWarning = props.AuthExpWarning,
                ResourceIpPrefix = props.ResourceIPPrefix,
                SchedulerAdministratorClientID = props.Admin.ClientID,
                GeneralLabAccountID = props.LabAccount.AccountID
            };

            return View(model);
        }

        [HttpPost, Route("props")]
        public ActionResult Index(PropertiesModel model)
        {
            ViewBag.ActiveTab = "properties";

            ViewBag.Clients = DA.Current.Query<Client>().Where(x => x.Active).OrderBy(x => x.LName).ThenBy(x => x.FName).ToList().Select(x => new SelectListItem() { Value = x.ClientID.ToString(), Text = x.DisplayName });
            ViewBag.Accounts = DA.Current.Query<Account>().Where(x => x.Active).OrderBy(x => x.Name).ToList().Select(x => new SelectListItem() { Value = x.AccountID.ToString(), Text = x.Name });

            var props = Properties.Current;

            props.LateChargePenaltyMultiplier = model.LateChargePenaltyMultiplier;
            props.AuthExpWarning = model.AuthorizationExpirationWarning;
            props.ResourceIPPrefix = model.ResourceIpPrefix;
            props.Admin = DA.Current.Single<Client>(model.SchedulerAdministratorClientID);
            props.LabAccount = DA.Current.Single<Account>(model.GeneralLabAccountID);

            props.Save();

            return View(model);
        }
    }
}