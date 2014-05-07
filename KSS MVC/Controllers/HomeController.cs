using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using KSS.Models;
using KSS.Server.Entities;

namespace KSS.Controllers
{
    public class HomeController : Controller
    {
        public HomeController()
        {
        }

        public ActionResult Index()
        {
            var TreeVM = new TreeViewModel();
            return View(TreeVM);
        }
        
    }
}
