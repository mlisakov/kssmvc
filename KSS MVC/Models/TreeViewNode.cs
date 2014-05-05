using System;
using System.Collections.Generic;
using System.Linq;
using KSS.Server.Entities;

namespace KSS.Models
{
    public class TreeViewNode
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public List<TreeViewNode> Children { get; set; }

        public bool IsLeaf {
            get { return Children == null || !Children.Any(); }
        }

        CompanyBaseModel _baseModel;

        public TreeViewNode()
        {
            _baseModel = new CompanyBaseModel();
        }

        public TreeViewNode(DivisionState divisionState):this()
        {
            InitNode(divisionState);
        }

        public TreeViewNode(DepartmentState departmentState)
            : this()
        {
            InitNode(departmentState);
        }


        private void InitNode(DivisionState divisionState)
        {
            Id = divisionState.Id;
            Name = divisionState.Division;
            Type = "DivisionState";
            List<DivisionState> divisionStates = _baseModel.DivisionStates.Where(i => i.ParentId == Id).ToList();
            foreach (DivisionState divState in divisionStates)
            {
                if (Children==null)
                    Children=new List<TreeViewNode>();
                Children.Add(new TreeViewNode(divState));
            }
                
        }

        private void InitNode(DepartmentState departmentState)
        {
            Id = departmentState.Id;
            Name = departmentState.Department;
            Type = "DepartmentState";
            List<DivisionState> divisionStates = _baseModel.DivisionStates.Where(i => i.ParentId == Id).ToList();
            foreach (DivisionState divState in divisionStates)
            {
                if (Children == null)
                    Children = new List<TreeViewNode>();
                Children.Add(new TreeViewNode(divState));
            }

        }
    }
}