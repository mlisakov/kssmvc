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

        public ActionResult TreeNodes(Guid id,string type)
        {
            ViewBag.Children = _treeVM.GetChildrens(id,
                type.Equals("DivisionState") ? "DivisionState" : "DepartmentState");
            return View("TreeNodes");
        }
    }
}