using System.Web.Mvc;
using KSS.Models;

namespace KSS.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //TreeViewModel TreeVM;
            //if (Session["Tree"] == null)
            //    TreeVM = new TreeViewModel();
            //else
            //    TreeVM = (TreeViewModel) Session["Tree"];
            HomeViewModel homeViewModel=new HomeViewModel(Session);
            return View(homeViewModel);
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
