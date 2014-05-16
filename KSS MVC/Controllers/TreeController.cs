using System;
using System.Collections.Generic;
using System.Linq;
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
            //Session["Tree"] = _treeVM;
            List<TreeViewNode> children=_treeVM.GetChildrens(id,
                type.Equals("DivisionState") ? "DivisionState" : "DepartmentState").ToList();
            UpdateTreeCache(children, id);
            ViewBag.Children = children;
            return View("TreeNodes");
        }

        private void UpdateTreeCache(List<TreeViewNode> children,Guid id)
        {
            if (Session["Tree"] == null)
                Session["Tree"] = _treeVM;

            TreeViewModel cachedTree = (TreeViewModel) Session["Tree"];
            TreeViewNode node = cachedTree.GetNode(id);
            node.Children = children;
        }
    }
}