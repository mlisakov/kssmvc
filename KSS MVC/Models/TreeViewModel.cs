using System;
using System.Collections.Generic;
using System.Linq;
using KSS.Server.Entities;

namespace KSS.Models
{
    public class TreeViewModel
    {
        private static CompanyBaseModel _baseModel;
        public List<TreeViewNode> RootNodes;
        private readonly Dictionary<Guid,TreeViewNode> _dictionaryTree;

        public TreeViewModel()
        {
            if (_baseModel == null)
                _baseModel = new CompanyBaseModel();
            _dictionaryTree=new Dictionary<Guid, TreeViewNode>();
            RootNodes=new List<TreeViewNode>();
            FillRootDivision();
        }

        public TreeViewNode GetNode(Guid id)
        {
            return _dictionaryTree[id];
        }

        public IEnumerable<TreeViewNode> GetChildrens(Guid? id, string type)
        {
            //Thread.Sleep(5000);
            if (!id.HasValue) return null;

            if (_dictionaryTree.ContainsKey(id.Value))
                _dictionaryTree[id.Value].Expanded = !_dictionaryTree[id.Value].Expanded;

            if (!_dictionaryTree[id.Value].Expanded)
                return null;

            //Берем данные их кэш
            if (_dictionaryTree.ContainsKey(id.Value) && _dictionaryTree[id.Value].Children.Any())
            {
                return _dictionaryTree[id.Value].Children;
            }
            if (type.Equals("DepartmentState"))
            {
                var departmentStates =
                    _baseModel.DepartmentStates.Where(t => t.ParentId == id.Value && t.ExpirationDate == null);

                _dictionaryTree[id.Value].Children = SelectDepartmentStateNodes(departmentStates, id);

                return _dictionaryTree[id.Value].Children;
            }
            var divisions =
                _baseModel.DivisionStates.Where(t => t.ParentId == id.Value && t.ExpirationDate == null)
                    .OrderBy(t => t.Division);

            var departments =
                _baseModel.DepartmentStates.Where(t => t.DivisionId == id.Value && t.ParentId == null && t.ExpirationDate == null)
                    .OrderBy(t => t.Department);

            var children = new List<TreeViewNode>();
            if (departments.Any())
            {
                children.AddRange(SelectDepartmentStateNodes(departments, id));
            }
            children.AddRange(SelectDivisionStateNodes(divisions, id));
            _dictionaryTree[id.Value].Children = children;
            return _dictionaryTree[id.Value].Children;
        }

        private void FillRootDivision()
        {
            List<DivisionState> rootNodes =
                _baseModel.DivisionStates.Where(i => i.ParentId == null && i.ExpirationDate == null).ToList();

            foreach (DivisionState divisionState in rootNodes)
            {
                bool hasChildren =
                    _baseModel.DivisionStates.Any(i => i.ParentId == divisionState.Id && i.ExpirationDate == null) ||
                    _baseModel.DepartmentStates.Any(i => i.DivisionId == divisionState.Id && i.ExpirationDate == null);

                var rootNode = new TreeViewNode(divisionState, hasChildren);
                RootNodes.Add(rootNode);
                _dictionaryTree.Add(rootNode.Id, rootNode);
            }
        }

        private List<TreeViewNode> SelectDepartmentStateNodes(IEnumerable<DepartmentState> departmentStates, Guid? parentId)
        {
            var children=new List<TreeViewNode>();
            foreach (DepartmentState department in departmentStates)
            {
                bool hasChildren =
                    _baseModel.DepartmentStates.Any(i => i.ParentId == department.Id && i.ExpirationDate == null);

                var node = new TreeViewNode(department, hasChildren) { ParentId = parentId };
                if (!_dictionaryTree.ContainsKey(node.Id))
                    _dictionaryTree.Add(node.Id, node);
                children.Add(node);
            }
            return children;
        }

        private IEnumerable<TreeViewNode> SelectDivisionStateNodes(IEnumerable<DivisionState> divisionStates, Guid? parentId)
        {
            var children = new List<TreeViewNode>();
            foreach (DivisionState division in divisionStates)
            {
                bool hasChildren =
                    _baseModel.DivisionStates.Any(i => i.ParentId == division.Id && i.ExpirationDate == null) ||
                    _baseModel.DepartmentStates.Any(
                        t => t.DivisionId == division.Id && t.ParentId == null && t.ExpirationDate == null);

                var node = new TreeViewNode(division, hasChildren) {ParentId = parentId};
                if (!_dictionaryTree.ContainsKey(node.Id))
                    _dictionaryTree.Add(node.Id, node);
                children.Add(node);
            }
            return children;
        }
    }
}