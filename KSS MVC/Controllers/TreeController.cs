using System;
using System.Web.Mvc;
using KSS.Models;

namespace KSS.Controllers
{
    public class Tree:Controller
    {
        public ActionResult Index()
        {
            TreeViewModel tree = new TreeViewModel();
            return View("Tree",tree);
        }


        [HttpGet]
        public virtual ActionResult GetDivisionChildren()
        {
            //var city = this._cityRepository.Find(cityId);

            return Json(
                new { Name = "123"},
                JsonRequestBehavior.AllowGet);
        }

       
    }
}