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
            HomeViewModel favoritesViewModel = new HomeViewModel(Session);
            return View(favoritesViewModel);
        }

        public bool RemoveFavorite(Guid id)
        {
            HomeViewModel favoritesViewModel = new HomeViewModel(Session);
            return favoritesViewModel.RemoveFromFavorite(id);
        }

        public bool AddFavorite(Guid id)
        {
            HomeViewModel favoritesViewModel = new HomeViewModel(Session);
            return favoritesViewModel.AddToFavorite(id);
        }

        public ActionResult Help()
        {
            return View();
        }
        
    }
}
