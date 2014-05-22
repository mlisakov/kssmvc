using System;
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
            FavoritesModel favoritesViewModel = new FavoritesModel(Session);
            return View(favoritesViewModel);
        }

        public void RemoveFavorite(Guid id)
        {
            
        }

        public ActionResult Help()
        {
            return View();
        }
        
    }
}
