using System.Collections.Generic;
using System.Web.Mvc;
using KSS.DBConnector;
using KSS.DBConnector.Models;

namespace KSS.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            KssDomain domain=new KssDomain();
            IList<DivisionState> divisions = domain.GetDivisions();
            ViewBag.Divisions = divisions;
            return View();
        }

    }
}
