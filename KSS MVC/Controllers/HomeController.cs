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

            ViewBag.UserName = Session["UserName"];
            ViewBag.UserID = Session["CurrentUser"];

            return View(homeViewModel);
        }

        public ActionResult SearchView(Guid id)
        {
            SearchViewModel model = id == Guid.Empty ? new SearchViewModel(Session) : new SearchViewModel(Session, id);

            ViewResult view= View(model);

            view.ViewBag.StartIndex = 0;            ;
            view.ViewBag.IsAdvanced = true;
            view.ViewBag.Search = "";            
            view.ViewBag.SearchPlace = null;
            view.ViewBag.SearchIsMember = false;

            view.ViewBag.SearchPhoneNumber = "";            
            view.ViewBag.SearchDateStart = "";
            view.ViewBag.SearchDateEnd = "";
            view.ViewBag.SearchJob = "";


            Session["BackLink"] = Url.Action("SearchView", "Home", new { id = id });
            return view;
        }

        public ActionResult Favorites(int startIndex)
        {
            var guid = new Guid(Session["CurrentUser"].ToString());


            var favoritesCount = DBHelper.GetFavoritesCount(guid);

            var employees = new List<EmployeeModel>();
            int pagesCount = 0;
            if (favoritesCount > 0)
            {
                DBHelper.CheckAndOrderFavorites(guid);

                employees = DBHelper.GetFavorites(guid, _pageSize, startIndex);

                pagesCount = favoritesCount / _pageSize;
                if ((favoritesCount % _pageSize) != 0)
                    pagesCount++;
            }

            ViewResult view = View("FavoritesPage", employees);
            view.ViewBag.StartIndex = startIndex;
            view.ViewBag.PageCount = pagesCount;


            Session["BackLink"] = Url.Action("Favorites", "Home", new {startIndex = startIndex});
            
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
            Session["BackLink"] = Url.Action("Help", "Home");
            return view;
        }

        public ActionResult SearchEmployees(string employeeName, int startIndex = 0)
        {
            var guid = new Guid(Session["CurrentUser"].ToString());

            var employees = DBHelper.Search(guid, employeeName, _pageSize, startIndex);
            ViewResult view = View("SearchEmployeeResult", employees);

            var count = DBHelper.GetSearchResultCount(employeeName)/_pageSize;

            if ((count % _pageSize) != 0)
                count++;

            view.ViewBag.StartIndex = startIndex;
            view.ViewBag.PageCount = count;
            view.ViewBag.IsAdvanced = false;
            view.ViewBag.Search = employeeName;

            Session["BackLink"] = Url.Action("SearchEmployees", "Home",
                new {employeeName = employeeName, startIndex = startIndex});
            return view;
        }

        public ActionResult SearchEmployeesAdvanced(Guid? divisionId, Guid? placeId, bool isMemberOfHeadquarter,
            string phoneNumber, Guid? departmentId, string dateStart, string dateEnd, string job, string employeeName, int startIndex = 0)
        {
            var guid = new Guid(Session["CurrentUser"].ToString());

            var employees = DBHelper.SearchAdvanced(divisionId, placeId, isMemberOfHeadquarter, phoneNumber,
                departmentId,
                dateStart, dateEnd, job, employeeName, _pageSize, false, guid, startIndex);

            ViewResult view = View("SearchEmployeeResult", employees);

            var count = DBHelper.GetAdvancedSearchResultCount(divisionId, placeId, isMemberOfHeadquarter, phoneNumber,
                departmentId,
                dateStart, dateEnd, job, employeeName)/_pageSize;

            if ((count % _pageSize) != 0)
                count++;

            view.ViewBag.StartIndex = startIndex;
            view.ViewBag.PageCount = count;
            view.ViewBag.IsAdvanced = true;
            view.ViewBag.Search = employeeName;
            view.ViewBag.SearchDivision = divisionId;
            view.ViewBag.SearchPlace = placeId;
            view.ViewBag.SearchIsMember = isMemberOfHeadquarter;

            view.ViewBag.SearchPhoneNumber = phoneNumber;
            view.ViewBag.SearchDepartment = departmentId;
            view.ViewBag.SearchDateStart = dateStart;
            view.ViewBag.SearchDateEnd = dateEnd;
            view.ViewBag.SearchJob = job;


            Session["BackLink"] = Url.Action("SearchEmployeesAdvanced", "Home",
                new
                {
                    divisionId = divisionId,
                    placeId = placeId,
                    isMemberOfHeadquarter = isMemberOfHeadquarter,
                    phoneNumber = phoneNumber,
                    departmentId = departmentId,
                    dateStart = dateStart,
                    dateEnd = dateEnd,
                    job = job,
                    employeeName = employeeName,
                    startIndex = startIndex,
                });
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
