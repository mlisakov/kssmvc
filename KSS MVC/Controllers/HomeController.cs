using System.Collections.Generic;
using System.Web.Mvc;
using KSS.Models;
using KSS.Server.Entities;

namespace KSS.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        public TreeViewModel TreeVM;

        public ActionResult Index()
        {
            TreeVM = new TreeViewModel();
            return View(TreeVM);
        }

    }
}
