using System;
using System.Web.Mvc;
using KSS.Models;

namespace KSS.Controllers
{
    public class EmployeeController : Controller
    {
        public ActionResult Index(Guid id)
        {
            EmployeeModel employeeViewModel = new EmployeeModel(id);
            return View(employeeViewModel);
        }
    }
}