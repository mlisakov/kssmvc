using System.Collections.Generic;
using System.Web.Mvc;
using KSS.Server.Entities;

namespace KSS.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        private CompanyBaseModel _baseModel = new CompanyBaseModel();

        public ActionResult Index()
        {
            IEnumerable<DivisionState> states = _baseModel.DivisionStates;
            ViewBag.DivisionState = states;
            return View();
        }

    }
}
