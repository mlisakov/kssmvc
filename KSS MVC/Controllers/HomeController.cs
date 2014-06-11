using System;
using System.Collections.Generic;
using System.Linq;
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
            ViewResult view= View(model);
            view.ViewBag.Divisions = new SelectList(DBHelper.GetDivisionStates(),"Id","Division");
            return view;
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

        public ActionResult GetDepartments(Guid divisionId)
        {
            return Json(new SelectList(DBHelper.GetDepartmentStatesByDivision(divisionId), "Id", "Department"),
                JsonRequestBehavior.AllowGet);
        }
        
    }
}
