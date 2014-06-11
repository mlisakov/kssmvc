using System;
using System.Collections.Generic;
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
        private int _pageCount;

        public int PageCount
        {
            get { return _pageCount; }
        }


        //private static CompanyBaseModel _baseModel;

        public SearchViewModel(HttpSessionStateBase session, Guid? id = null)
        {
            //if (_baseModel == null)
            //    _baseModel = new CompanyBaseModel();
            _session = session;
            if (id.HasValue)
                _id = id.Value;
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

        public List<DivisionState> GetDivisionStates()
        {
            return DBHelper.GetDivisionStates();
        }

        public List<DepartmentState> GetDepartmentStates()
        {
            return DBHelper.GetDepartmentStates();
        }

        public Dictionary<Guid, string> GetCustomLocality()
        {
            return DBHelper.GetCustomLocality();
        }

        public List<EmployeeModel> Search(string employeeName, int page = 0)
        {
            List<EmployeeModel> employees=DBHelper.Search(employeeName,5, page).ToList();
            int _pageCount = employees.Count() / 5;
                if ((employees.Count() % 5) != 0)
                    _pageCount++;
            return employees;
        }
    }
}