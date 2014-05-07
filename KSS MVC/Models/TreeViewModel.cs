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
            Root = new TreeViewNode(root);
            _dictionaryTree.Add(Root.Id,Root);
        }

        private List<TreeViewNode> SelectDepartmentStateNodes(IEnumerable<DepartmentState> departmentStates, Guid? parentId)
        {
            List<TreeViewNode> children=new List<TreeViewNode>();
            foreach (DepartmentState department in departmentStates)
            {
                var node = new TreeViewNode(department) {ParentId = parentId};
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
                var node = new TreeViewNode(division) { ParentId = parentId };
                _dictionaryTree.Add(node.Id, node);
                children.Add(node);
            }
            return children;
        }
    }
}