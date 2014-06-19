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
            ViewResult view= View(model);
            view.ViewBag.Divisions = new SelectList(DBHelper.GetDivisionStates(),"Id","Division");
            return view;
        }

        public ActionResult Favorites(int startIndex)
        {
            var guid = new Guid(Session["CurrentUser"].ToString());

            var employees = DBHelper.GetFavorites(guid, _pageSize, startIndex);
            ViewResult view = View("FavoritesPage", employees);
            
            int count = DBHelper.GetFavoritesCount(guid) / _pageSize;
            if ((employees.Count % _pageSize) != 0)
                count++;

            view.ViewBag.StartIndex = startIndex;
            view.ViewBag.PageCount = count;
            return view;
        }

        public ActionResult FavoritesWithReplace(int startIndex, Guid userGuid, int delta)
        {
            var guid = new Guid(Session["CurrentUser"].ToString());
            DBHelper.UpdateFavoritePosition(guid, userGuid, delta);

            return Favorites(startIndex);
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
            ViewResult view = View("Help");
            return view;
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

        public ActionResult GetDepartments(Guid divisionId)
        {
            return Json(new SelectList(DBHelper.GetDepartmentStatesByDivision(divisionId), "Id", "Department"),
                JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPositions(Guid departmentId)
        {
            return Json(new SelectList(DBHelper.GetPositionStatesByDepartment(departmentId), "Id", "Title"),
                JsonRequestBehavior.AllowGet);
        }
        
    }
}
