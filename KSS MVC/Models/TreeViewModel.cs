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

        public TreeViewModel()
        {
            if (_baseModel == null)
                _baseModel = new CompanyBaseModel();
            FillRootDivision();
        }

        public IEnumerable<TreeViewNode> GetChildrens(Guid? id,string type)
        {
            if (!id.HasValue) return null;

            if (type.Equals("DepartmentState"))
            {
                var departmentStates =
                    _baseModel.DepartmentStates.Where(t => t.ParentId == id.Value);
                return SelectDepartmentStateNodes(departmentStates, id);
            }
            var divisionStates =
                _baseModel.DivisionStates.Where(t => t.ParentId == id.Value);
            return SelectDivisionStateNodes(divisionStates, id);
        }

        public IEnumerable<TreeViewNode> GetDepartmentChildrens(Guid id, Guid? divisionGuid)
        {
            if (!divisionGuid.HasValue) return null;

            var departmentStates =
                _baseModel.DepartmentStates.Where(t => t.DivisionId == divisionGuid.Value && t.ParentId == id);
            return SelectDepartmentStateNodes(departmentStates, divisionGuid);
        }

        private void FillRootDivision()
        {
            DivisionState root = _baseModel.DivisionStates.First(i => i.ParentId == null);
            Root = new TreeViewNode(root);
        }

        private IEnumerable<TreeViewNode> SelectDepartmentStateNodes(IEnumerable<DepartmentState> departmentStates, Guid? parentId)
        {
            return departmentStates.Select(state => new TreeViewNode(state) {ParentId = parentId}).ToList();
        }

        private IEnumerable<TreeViewNode> SelectDivisionStateNodes(IEnumerable<DivisionState> divisionStates, Guid? parentId)
        {
            return divisionStates.Select(state => new TreeViewNode(state) {ParentId = parentId}).ToList();
        }
    }
}