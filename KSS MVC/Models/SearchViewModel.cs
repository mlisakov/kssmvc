using System;
using System.Linq;
using System.Web;
using KSS.Helpers;
using KSS.Server.Entities;

namespace KSS.Models
{
    public class SearchViewModel
    {
        private Guid _id;
        private HttpSessionStateBase _session;

        private static CompanyBaseModel _baseModel;

        public SearchViewModel(HttpSessionStateBase session,Guid id)
        {
            //if (_baseModel == null)
            //    _baseModel = new CompanyBaseModel();
            _session = session;
            _id = id;
        }

        public string GetDepartmentName()
        {
            var result = "Без наименования";
            // var item = _baseModel.DepartmentStates.FirstOrDefault(t => t.Id == _id);
            var item = DBHelper.GetDepartmentState(_id);
            if (item == null)
            {
                //var innerItem = _baseModel.DivisionStates.FirstOrDefault(t => t.Id ==_id);
                var innerItem = DBHelper.GetDivisionState(_id);
                if (innerItem != null)
                    result = innerItem.Division;
            }
            else
            {
                result = item.Department;
            }
            return result;
        }
    }
}