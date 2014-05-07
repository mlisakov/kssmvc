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

                _dictionaryTree[id.Value].Children = SelectDepartmentStateNodes(departmentStates, id);
                return _dictionaryTree[id.Value].Children;
            }
            var divisions =
                _baseModel.DivisionStates.Where(t => t.ParentId == id.Value);
            var departments = _baseModel.DepartmentStates.Where(t => t.DivisionId == id.Value && t.ParentId==null);
            if (departments.Any())
            {
                _dictionaryTree[id.Value].Children.AddRange(SelectDepartmentStateNodes(departments, id));
            }
            _dictionaryTree[id.Value].Children.AddRange( SelectDivisionStateNodes(divisions, id));
            return _dictionaryTree[id.Value].Children;
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
                if (!_dictionaryTree.ContainsKey(node.Id))
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
                bool hasChildren = _baseModel.DivisionStates.Any(i => i.ParentId == division.Id) ||
                                   _baseModel.DepartmentStates.Any(
                                       t => t.DivisionId == division.Id && t.ParentId == null);
                var node = new TreeViewNode(division, hasChildren) {ParentId = parentId};
                if (!_dictionaryTree.ContainsKey(node.Id))
                    _dictionaryTree.Add(node.Id, node);
                children.Add(node);
            }
            return children;
        }
    }
}