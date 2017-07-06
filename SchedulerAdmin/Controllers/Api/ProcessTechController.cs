using LNF.Models.Scheduler;
using LNF.Repository;
using LNF.Repository.Scheduler;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace SchedulerAdmin.Controllers.Api
{
    public class ProcessTechController : ApiController
    {
        [Route("api/proctech")]
        public IEnumerable<ProcessTechModel> Get()
        {
            var query = DA.Current.Query<ProcessTech>().Where(x => x.IsActive).ToList();
            var result = query.Model<ProcessTechModel>().OrderBy(x => x.BuildingName).ThenBy(x => x.LabDisplayName).ThenBy(x => x.ProcessTechName).ToList();
            return result;
        }
    }
}
