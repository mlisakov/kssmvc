using System;
using System.Text;
using System.Web.Mvc;
using KSS.Helpers;
using KSS.Models;

namespace KSS.Controllers
{
    public class EmployeeController : Controller
    {
        public ActionResult Index(Guid id)
        {
            var currentUser = new Guid(Session["CurrentUser"].ToString());

            bool isAdmin = Convert.ToBoolean(Session["IsAdministrator"]);
            var employeeViewModel = new EmployeeModel(id, currentUser);

            var view = View("PersonCard", employeeViewModel);
            view.ViewBag.IsAdmin = isAdmin;
            view.ViewBag.BackLink = Session["BackLink"];
            return view;
        }

        public ActionResult ChangeFavoriteStatus(Guid id)
        {
            var currentUser = new Guid(Session["CurrentUser"].ToString());

            var employeeViewModel = new EmployeeModel(id, currentUser);
            employeeViewModel.ChangeFavoriteStatus();

            return Index(id);
        }

        public bool ChangeFavoriteStatusInline(Guid id)
        {
            var currentUser = new Guid(Session["CurrentUser"].ToString());

            var employeeViewModel = new EmployeeModel(id, currentUser);
            var result = employeeViewModel.ChangeFavoriteStatus();

            return result;
        }

        [HttpGet]
        public string GetCities(string country, string region)
        {
            var sb = new StringBuilder();
            foreach (var locality in DBHelper.GetLocalities(country, region))
            {
                sb.Append("<option value=\"");
                sb.Append(locality.Key);
                sb.Append("\">");
                sb.Append(locality.Value);
                sb.Append("</option>");
            }
            return sb.ToString();
        }

        public ActionResult SaveLocation(Guid employee, Guid city, Guid edifice, string pavillion, string office)
        {
            DBHelper.UpdateEmployeePlaces(employee, city, edifice, pavillion, office);

            return Index(employee);
        }

        [HttpPost]
        public void ChangeMemberOfHeadquarter(Guid employee, bool isMember)
        {
            DBHelper.UpdateMemberOfHeadquarter(employee, isMember);
        }

        public ActionResult SavePhone(Guid employee, Guid? place, Guid? phoneType, string phone)
        {
            DBHelper.UpdateEmployeePhone(employee, place, phoneType, phone);

            return Index(employee);
        }

        public ActionResult DeletePhone(Guid employee, Guid employPlaceId)
        {
            DBHelper.DeletePhone(employPlaceId);

            return Index(employee);
        }

        public string SavePhoto(Guid employee, string image)
        {
            var result = DBHelper.UpdateEmployeePhoto(employee, image);

            if (!result)
            {
                return "При сохранении новой фотографии произошла ошибка. Попробуйте позднее или обратитесь к администратору.";
            }
            return string.Empty;
        }


        public ActionResult CreateNewLocation(Guid employee, string newDivisionName, Guid? parentDivisionGuid,
            Guid? existedDivision, string newRegion, string existedRegion, string newTerritory, Guid? existedTerritory,
            string city, Guid? existedCity, string innerPhoneCode, string outerPhoneCode, string street,
            Guid? existedStreet, string house, Guid? existedHouse, string pavilion)
        {
            DBHelper.CreateNewLocation(newDivisionName, parentDivisionGuid,
           existedDivision,  newRegion,  existedRegion,  newTerritory,  existedTerritory,
             city,  existedCity,  innerPhoneCode,  outerPhoneCode,  street,
           existedStreet,  house,  existedHouse,  pavilion);

            return Index(employee);
        }


        [HttpGet]
        public string GetTerritories(Guid division)
        {
            var sb = new StringBuilder();
            sb.Append("<option value=\"\" selected>не выбран</option>");
            foreach (var ter in DBHelper.GetTerritories(division))
            {
                sb.Append("<option value=\"");
                sb.Append(ter.Id);
                sb.Append("\">");
                sb.Append(ter.Territory);
                sb.Append("</option>");
            }
            return sb.ToString();
        }


        public string GetRegions(Guid division, Guid? territory)
        {
            var sb = new StringBuilder();
            sb.Append("<option value=\"\" selected>не выбран</option>");
            foreach (var ter in DBHelper.GetRegions(division, territory))
            {
                sb.Append("<option value=\"");
                sb.Append(ter);
                sb.Append("\">");
                sb.Append(ter);
                sb.Append("</option>");
            }
            return sb.ToString();
        }


        [HttpGet]
        public string GetCitiesForCreate(string region)
        {
            var sb = new StringBuilder();
            sb.Append("<option value=\"\" selected>не выбран</option>");
            foreach (var locality in DBHelper.GetLocalities("Россия", region))
            {
                if(string.IsNullOrEmpty(locality.Value))
                    continue;
                
                sb.Append("<option value=\"");
                sb.Append(locality.Key);
                sb.Append("\">");
                sb.Append(locality.Value);
                sb.Append("</option>");
            }
            return sb.ToString();
        }

        [HttpGet]
        public string GetStreets(Guid? locality)
        {
            var sb = new StringBuilder();
            sb.Append("<option value=\"\" selected>не выбран</option>");
            if(locality.HasValue)
                foreach (var street in DBHelper.GetStreets(locality.Value))
                {
                    if (string.IsNullOrEmpty(street.Value))
                        continue;
                    sb.Append("<option value=\"");
                    sb.Append(street.Key);
                    sb.Append("\">");
                    sb.Append(street.Value);
                    sb.Append("</option>");
                }
            return sb.ToString();
        }

        [HttpGet]
        public string GetHouses(Guid? locality, Guid? street)
        {
            var sb = new StringBuilder();
            sb.Append("<option value=\"\" selected>не выбран</option>");
            foreach (var item in DBHelper.GetHouses(locality, street))
            {
                if (string.IsNullOrEmpty(item.Value))
                    continue;
                sb.Append("<option value=\"");
                sb.Append(item.Key);
                sb.Append("\">");
                sb.Append(item.Value);
                sb.Append("</option>");
            }
            return sb.ToString();
        }

        [HttpGet]
        public string GetPavillions(Guid? locality, Guid edifice)
        {
            var sb = new StringBuilder();
            sb.Append("<option value=\"\" selected>не выбран</option>");
            foreach (var item in DBHelper.GetPavillions(locality, edifice))
            {
                if (string.IsNullOrEmpty(item))
                    continue;
                sb.Append("<option value=\"");
                sb.Append(item);
                sb.Append("\">");
                sb.Append(item);
                sb.Append("</option>");
            }
            return sb.ToString();
        }
    }
}