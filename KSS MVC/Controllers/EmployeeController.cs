using System;
using System.Web.Mvc;
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
            return view;
        }

        public ActionResult ChangeFavoriteStatus(Guid id)
        {
            var currentUser = new Guid(Session["CurrentUser"].ToString());

            var employeeViewModel = new EmployeeModel(id, currentUser);
            employeeViewModel.ChangeFavoriteStatus();

            var view = View("PersonCard", employeeViewModel);
            
            bool isAdmin = Convert.ToBoolean(Session["IsAdministrator"]);
            view.ViewBag.IsAdmin = isAdmin;

            return view;
        }
    }
}