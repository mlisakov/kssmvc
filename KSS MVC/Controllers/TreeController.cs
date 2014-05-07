using System;
using System.Web.Mvc;
using KSS.Models;

namespace KSS.Controllers
{
    public class TreeController:Controller
    {
        private static TreeViewModel _treeVM;

        public TreeController()
        {
            if (_treeVM == null)
                _treeVM = new TreeViewModel();
        }

        public ActionResult Index()
        {
            return View("Tree", _treeVM);
        }

        public ActionResult TreeNodesDivision(Guid id)
        {
            ViewBag.Children = _treeVM.GetChildrens(id,"DivisionState");
            return View("TreeNodes");
        }

        public ActionResult TreeNodesDepartment(Guid id)
        {
            ViewBag.Children = _treeVM.GetChildrens(id, "DepartmentState");
            return View("TreeNodes");
        }

       
    }
}