using System;
using System.Collections.Generic;
using System.Web.Mvc;
using KSS.Helpers;
using KSS.Models;
using KSS.Server.Entities;
using Newtonsoft.Json;

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
            HomeViewModel homeViewModel = new HomeViewModel(Session);
            return View(homeViewModel);
        }


        public ActionResult SearchView(Guid id)
        {
            SearchViewModel model = new SearchViewModel(Session,id);
            return View(model);
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

        public ActionResult SearchEmployees(string employeeName)
        {
            var employees = DBHelper.Search(employeeName);
            return View("SearchEmployeeResult", employees);
        }
        
    }
}
