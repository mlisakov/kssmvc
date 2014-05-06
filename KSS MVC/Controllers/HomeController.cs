using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using KSS.Models;
using KSS.Server.Entities;

namespace KSS.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        readonly CompanyBaseModel _baseModel;

        public HomeController()
        {
            _baseModel = new CompanyBaseModel();
        }

        public ActionResult Index()
        {
            var TreeVM = new TreeViewModel();
            return View(TreeVM);
        }


        public ActionResult TreeNodesDivision(Guid id)
        {
            ViewBag.Children = GetChildrens(id);
            return View("TreeNodes");
        }

        public ActionResult TreeNodesDepartment(Guid id, Guid? division)
        {
            ViewBag.Children = GetDepartmentChildrens(id, division);
            return View("TreeNodes");
        }

        public IEnumerable<TreeViewNode> GetChildrens(Guid? id)
        {
            if (id.HasValue)
            {
                var departmentStates = _baseModel.DepartmentStates.Where(t => t.DivisionId == id.Value && t.ParentId == null);
                return SelectNodes(departmentStates, id);
            }
            return null;
        }

        public IEnumerable<TreeViewNode> GetDepartmentChildrens(Guid id, Guid? divisionGuid)
        {
            if (divisionGuid.HasValue)
            {
                var departmentStates =
                    _baseModel.DepartmentStates.Where(t => t.DivisionId == divisionGuid.Value && t.ParentId == id);
                return SelectNodes(departmentStates, divisionGuid);
            }

            return null;
        }


        private IEnumerable<TreeViewNode> SelectNodes(IEnumerable<DepartmentState> departmentStates, Guid? divisionId)
        {
            var list = new List<TreeViewNode>();
            foreach (var state in departmentStates)
            {
                var node = new TreeViewNode
                {
                    Id = state.Id,
                    Name = state.Department,
                    Type = "DepartmentState",
                    HasChilds = _baseModel.DepartmentStates.Any(t => t.ParentId == state.Id)
                };
                if (node.HasChilds)
                    node.DivisionId = divisionId;
                list.Add(node);
            }

            return list;
        }
    }
}
