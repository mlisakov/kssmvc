using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KSS.Helpers;
using KSS.Server.Entities;

namespace KSS.Models
{
    public class SpecificSearchViewModel
    {
        private Guid? _id;
        private DepartmentSpecificState _specificState;
        private HttpSessionStateBase _session = null;
        private int _pageCount;

        public int PageCount
        {
            get { return _pageCount; }
        }

        public int StartIndex { get; set; }

        public string SpecificDepartmentName { get; set; }

        public Guid ID
        {
            get
            {
                if (_id.HasValue)
                    return _id.Value;
                return Guid.Empty;
            }
        }

        public SpecificSearchViewModel(HttpSessionStateBase session, Guid? id = null)
        {
            _session = session;
            if (id.HasValue)
                _id = id.Value;

            SpecificDepartmentName = GetSpecificDepartmentName();
            StartIndex = 0;
        }

        private string GetSpecificDepartmentName()
        {
            var result = "";

            if (_id.HasValue)
            {
                _specificState = DBHelper.GetDepartmentSpecificState(_id.Value);

                result = _specificState != null ? _specificState.Name : string.Empty;
            }

            return result;
        }

        public List<SpecificStaffModel> GetSpecificStaffs()
        {
            if (_id.HasValue)
            {
                var itemsCount = DBHelper.GetSpecificStaffsCount(_id.Value);

                _pageCount = itemsCount / 5;

                if ((itemsCount % 5) != 0)
                    _pageCount++;

                var f = DBHelper.GetSpecificStaffs(_id.Value, 5, StartIndex);

                return f;
//                return DBHelper.SearchAdvanced(divisionId, new Guid?(), false, string.Empty, departmentID, string.Empty,
//                    string.Empty, string.Empty, string.Empty, 5, true, guid);
            }
//
//            _pageCount = 0;
            return new List<SpecificStaffModel>();
        }
    }
}