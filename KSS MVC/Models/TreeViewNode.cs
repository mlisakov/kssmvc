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
            get { return !HasChilds; }
        }

        public bool HasChilds {
            get { return Children != null && Children.Any();} 
        }
        public Guid? ParentId { get; set; }

        public TreeViewNode(DivisionState divisionState)
        {
            InitDivisionStateNode(divisionState);
        }

        public TreeViewNode(DepartmentState departmentState)
           
        {
            InitDepartmentStateNode(departmentState);
        }


        private void InitDivisionStateNode(DivisionState divisionState)
        {
            Id = divisionState.Id;
            Name = divisionState.Division;
            Type = "DivisionState";
        }

        private void InitDepartmentStateNode(DepartmentState departmentState)
        {
            Id = departmentState.Id;
            Name = departmentState.Department;
            Type = "DepartmentState";
        }
    }
}