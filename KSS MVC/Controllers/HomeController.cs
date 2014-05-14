using System.Web.Mvc;
using KSS.Models;

namespace KSS.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var TreeVM = new TreeViewModel();
            return View(TreeVM);
        }

        public ActionResult Favorites()
        {
            return View();
        }

        public ActionResult Help()
        {
            return View();
        }
        
    }
}
