using System;
using System.Collections.Generic;
using System.Linq;
using KSS.Server.Entities;

namespace KSS.Models
{
    public class TreeViewModel
    {
        private static CompanyBaseModel _baseModel;
        public TreeViewNode Root;
        private Dictionary<Guid,TreeViewNode> _dictionaryTree;

        public TreeViewModel()
        {
            if (_baseModel == null)
                _baseModel = new CompanyBaseModel();
            _dictionaryTree=new Dictionary<Guid, TreeViewNode>();
            FillRootDivision();
        }

        public IEnumerable<TreeViewNode> GetChildrens(Guid? id,string type)
        {
            if (!id.HasValue) return null;

            if (type.Equals("DepartmentState"))
            {
                var departmentStates =
                    _baseModel.DepartmentStates.Where(t => t.ParentId == id.Value);

                List<TreeViewNode> departmentChildren = SelectDepartmentStateNodes(departmentStates, id);
                _dictionaryTree[id.Value].Children = departmentChildren;
                return departmentChildren;
            }
            var divisionStates =
                _baseModel.DivisionStates.Where(t => t.ParentId == id.Value);
            List<TreeViewNode> divisionChildren = SelectDivisionStateNodes(divisionStates, id);
            _dictionaryTree[id.Value].Children = divisionChildren;
            return divisionChildren;
        }

        private void FillRootDivision()
        {
            DivisionState root = _baseModel.DivisionStates.First(i => i.ParentId == null);
            bool hasChildren = _baseModel.DivisionStates.Any(i => i.ParentId == root.Id) ||
                               _baseModel.DepartmentStates.Any(i => i.ParentId == root.Id);
            Root = new TreeViewNode(root, hasChildren);
            _dictionaryTree.Add(Root.Id,Root);
        }

        private List<TreeViewNode> SelectDepartmentStateNodes(IEnumerable<DepartmentState> departmentStates, Guid? parentId)
        {
            List<TreeViewNode> children=new List<TreeViewNode>();
            foreach (DepartmentState department in departmentStates)
            {
                bool hasChildren = _baseModel.DepartmentStates.Any(i => i.ParentId == department.Id);
                var node = new TreeViewNode(department, hasChildren) { ParentId = parentId };
                _dictionaryTree.Add(node.Id, node);
                children.Add(node);
            }
            return children;
        }

        private List<TreeViewNode> SelectDivisionStateNodes(IEnumerable<DivisionState> divisionStates, Guid? parentId)
        {
            List<TreeViewNode> children = new List<TreeViewNode>();
            foreach (DivisionState division in divisionStates)
            {
                bool hasChildren = _baseModel.DivisionStates.Any(i => i.ParentId == division.Id);
                var node = new TreeViewNode(division, hasChildren) { ParentId = parentId };
                _dictionaryTree.Add(node.Id, node);
                children.Add(node);
            }
            return children;
        }
    }
}