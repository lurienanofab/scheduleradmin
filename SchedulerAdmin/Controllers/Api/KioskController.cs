using LNF.Models.Scheduler;
using LNF.Repository;
using LNF.Repository.Scheduler;
using System.Collections.Generic;
using System.Web.Http;
using System.Linq;

namespace SchedulerAdmin.Controllers.Api
{
    public class KioskController : ApiController
    {
        [Route("api/kiosk")]
        public IEnumerable<KioskModel> Get()
        {
            return DA.Current.Query<Kiosk>().OrderBy(x => x.KioskName).Model<KioskModel>();
        }

        [Route("api/kiosk/{kioskId}")]
        public bool Delete(int kioskId)
        {
            var kiosk = DA.Current.Single<Kiosk>(kioskId);

            if (kiosk == null)
                return false;
            else
            {
                try
                {
                    DA.Current.Delete(kiosk);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}
