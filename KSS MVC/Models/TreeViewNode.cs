using System;
using System.Collections.Generic;
using System.Linq;
using KSS.Server.Entities;

namespace KSS.Models
{
    public class TreeViewNode
    {
        private bool _hasChildren;
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        /// <summary>
        /// Развернут нод или нет
        /// </summary>
        public bool Expanded { get; set; }

        public List<TreeViewNode> Children { get; set; }

        public bool IsLeaf {
            get { return !HasChilds; }
        }

        public bool HasChilds {
            get
            {
                return _hasChildren || Children != null && Children.Any();
            } 
        }
        public Guid? ParentId { get; set; }

        public TreeViewNode()
        {
            Children = new List<TreeViewNode>();
        }

        public TreeViewNode(DivisionState divisionState,bool hasChildren):this()
        {
            InitDivisionStateNode(divisionState,hasChildren);
        }

        public TreeViewNode(DepartmentState departmentState, bool hasChildren): this()
        {
            InitDepartmentStateNode(departmentState,hasChildren);
        }


        public TreeViewNode(DepartmentSpecificState departmentSpecificState, bool hasChildren)
            : this()
        {
            Id = departmentSpecificState.Id;
            Name = departmentSpecificState.Name;
            Type = "departmentSpecificState";
            _hasChildren = hasChildren;
        }


        private void InitDivisionStateNode(DivisionState divisionState,bool hasChildren)
        {
            Id = divisionState.Id;
            Name = divisionState.Division;
            Type = "DivisionState";
            _hasChildren = hasChildren;
        }

        private void InitDepartmentStateNode(DepartmentState departmentState,bool hasChildren)
        {
            Id = departmentState.Id;
            Name = departmentState.Department;
            Type = "DepartmentState";
            _hasChildren = hasChildren;
        }
    }
}