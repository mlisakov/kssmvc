using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using Antlr.Runtime;
using KSS.Helpers;
using KSS.Models;
using KSS.Server.Entities;
using Newtonsoft.Json;

namespace KSS.Controllers
{
    public class HomeController : Controller
    {
        private int _pageSize = 5;
        public ActionResult Index()
        {
            HomeViewModel homeViewModel = new HomeViewModel(Session);


            return View(homeViewModel);
        }

//        public async Task<ActionResult> Index()
//        {
//            HomeViewModel homeViewModel = new HomeViewModel(Session);
//
//            var items = await Task.Run(() => homeViewModel.GetBirthdayPeople());
//            homeViewModel.Birthdays = items;
//
//            return View(homeViewModel);
//        }


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

        public ActionResult SearchEmployees(string employeeName,int startIndex=0)
        {
            var employees = DBHelper.Search(employeeName,_pageSize,startIndex);
            ViewResult view=View("SearchEmployeeResult", employees);

            int count = employees.Count / _pageSize;
            if ((employees.Count % _pageSize) != 0)
                count++;
            int step = _pageSize/2;
            if (startIndex - step > 0)
                startIndex = startIndex - step;
            view.ViewBag.StartIndex = startIndex;
            view.ViewBag.PageCount = count;
            view.ViewBag.Search = employeeName;
            return view;
        }

        public async Task<ActionResult> GetBirthdays()
        {
            List<EmployeeModel> people =
                await Task.Run(() => DBHelper.GetBirthdayPeople(Session["CurrentUserDivision"].ToString()));

            ViewResult view = View("BirthDayResult", people);            
            return view;
        }

    }
}
