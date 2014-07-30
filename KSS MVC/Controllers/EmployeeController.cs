﻿using System;
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

        public ActionResult SaveLocation(Guid employee, Guid city, string street, string edifice, string office)
        {
            DBHelper.UpdateEmployeePlaces(employee, city, street, edifice, office);

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
    }
}