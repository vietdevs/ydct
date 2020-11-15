using Abp.Auditing;
using System.Web.Mvc;

namespace newPSG.PMS.Web.Controllers
{
    public class ErrorController : PMSControllerBase
    {
        [DisableAuditing]
        public ActionResult E404()
        {
            return View();
        }
    }
}