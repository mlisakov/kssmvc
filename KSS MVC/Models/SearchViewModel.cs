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
        private Guid? _id;
        private DepartmentState _department;
        private DivisionState _division = null;
        private HttpSessionStateBase _session = null;
        private int _pageCount;

        public int PageCount
        {
            get { return _pageCount; }
        }

        public Guid? DepartmentID
        {
            get { return _department == null ? new Guid?() : _department.Id; }
        }

        public Guid? DivisionID
        {
            get
            {
                return _division == null
                    ? (_department == null
                        ? new Guid?()
                        : _department.DivisionId)
                    : _division.Id;
            }
        }

        public string DepartmentName { get; set; }

        public SearchViewModel(HttpSessionStateBase session, Guid? id = null)
        {
            _session = session;
            if (id.HasValue)
                _id = id.Value;

            DepartmentName = GetDepartmentName();
        }

        private string GetDepartmentName()
        {
            var result = "";

            if (_id.HasValue)
            {
                _department = DBHelper.GetDepartmentState(_id.Value);
                if (_department == null)
                {
                    _division = DBHelper.GetDivisionState(_id.Value);
                    if (_division != null)
                        result = _division.Division;
                }
                else
                    result = _department.Department;
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

        public List<PositionState> GetPositionStates()
        {
            return DBHelper.GetPositionStates();
        }

        public List<EmployeeModel> GetEmployers()
        {
            if (_id.HasValue)
            {
                var guid = new Guid(_session["CurrentUser"].ToString());
                var divisionId = DivisionID;
                var departmentID = DepartmentID;

                var itemsCount = DBHelper.GetAdvancedSearchResultCount(divisionId, new Guid?(), false, string.Empty,
                    departmentID, string.Empty, string.Empty, string.Empty, string.Empty);

                _pageCount = itemsCount / 5;

                if ((itemsCount % 5) != 0)
                    _pageCount++;

                return DBHelper.SearchAdvanced(divisionId, new Guid?(), false, string.Empty, departmentID, string.Empty,
                    string.Empty, string.Empty, string.Empty, 5, true, guid);
            }

            _pageCount = 0;
            return new List<EmployeeModel>();
        }
    }
}