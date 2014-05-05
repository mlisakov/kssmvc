using System;
using System.Linq;
using KSS.Server.Entities;

namespace KSS.Models
{
    public class TreeViewModel
    {
        CompanyBaseModel _baseModel;
        public TreeViewNode Root;

        public TreeViewModel()
        {
            _baseModel=new CompanyBaseModel();
            FillRootDivision();
        }

        private void FillRootDivision()
        {
            DivisionState root=_baseModel.DivisionStates.First(i => i.ParentId == null);
            Root=new TreeViewNode(root);
        }


    }
}