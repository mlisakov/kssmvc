using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using KSS.Models;

namespace KSS.Controllers
{
    public class TreeController:Controller
    {
        private static TreeViewModel _treeViewModel;

        public TreeController()
        {
            if (_treeViewModel == null)
                _treeViewModel = new TreeViewModel(false);
        }

        public ActionResult Index()
        {
            return View("Tree", _treeViewModel);
        }

        public ActionResult TreeNodes(Guid id,string type)
        {
            List<TreeViewNode> children=_treeViewModel.GetChildrens(id,
                type.Equals("DivisionState") ? "DivisionState" : "DepartmentState").ToList();
            UpdateTreeCache(children, id);
            ViewBag.Children = children;
            return View("TreeNodes");
        }

        private void UpdateTreeCache(List<TreeViewNode> children,Guid id)
        {
            if (Session["Tree"] == null)
                Session["Tree"] = _treeViewModel;

            var cachedTree = (TreeViewModel) Session["Tree"];
            TreeViewNode node = cachedTree.GetNode(id);
            node.Children = children;
        }

        public ActionResult SpecificTree()
        {
            var treeModel = new TreeViewModel(true);
            


            return View("Tree", treeModel);
        }
    }
}