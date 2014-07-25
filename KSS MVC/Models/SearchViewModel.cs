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
        private DepartmentState _department;
        private DivisionState _division = null;
        private HttpSessionStateBase _session = null;
        private int _pageCount;

        public int PageCount
        {
            get { return _pageCount; }
        }

        public SearchViewModel(HttpSessionStateBase session, Guid? id = null)
        {
            _session = session;
            if (id.HasValue)
                _id = id.Value;
        }

        public string GetDepartmentName()
        {
            var result = "Без наименования";

            _department = DBHelper.GetDepartmentState(_id);
            if (_department == null)
            {
                _division = DBHelper.GetDivisionState(_id);
                if (_division != null)
                    result = _division.Division;
            }
            else
                result = _department.Department;

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

        public List<PositionState> GetPositionStates()
        {
            return DBHelper.GetPositionStates();
        }

        public List<EmployeeModel> GetEmployers()
        {

            return DBHelper.SearchAdvanced(_division == null ? new Guid?() : _division.Id, Guid.Empty, false,
                string.Empty,
                _department == null ? new Guid?() : _department.Id, string.Empty, string.Empty, string.Empty,
                string.Empty, 5, 0);
        }
    }
}