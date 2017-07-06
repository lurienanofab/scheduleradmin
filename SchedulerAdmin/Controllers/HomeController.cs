using System.Web.Mvc;

namespace SchedulerAdmin.Controllers
{
    public class HomeController : Controller
    {
        [Route("")]
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Activities");
        }
    }
}