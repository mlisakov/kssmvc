using System;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using KSS.Server.Entities;

namespace KSS.Helpers
{
    public class DBHelper
    {
        private static CompanyBaseModel _baseModel;

        static DBHelper()
        {
            _baseModel = new CompanyBaseModel();
        }

        public static Guid GetUserDepartment(Guid userGuid)
        {
            var t = from employee in _baseModel.Employees
                join staff in _baseModel.Staffs on employee.Id equals staff.Id into employeeStaff
                from m in employeeStaff.DefaultIfEmpty()
                where employee.Id == userGuid && m.ExpirationDate == null
                select m.DepartmentId;
                return t.FirstOrDefault();
        }

        public static Tuple<Guid, string> GetUserDivision(Guid userGuid)
        {
            var t = from employee in _baseModel.Employees
                join staff in _baseModel.Staffs on employee.Id equals staff.Id into employeeStaff
                from m in employeeStaff.DefaultIfEmpty()
                join departmentState in _baseModel.DepartmentStates on m.DepartmentId equals departmentState.Id into
                    depState
                from ds in depState.DefaultIfEmpty()
                join divisionState in _baseModel.DivisionStates on ds.DivisionId equals divisionState.Id into divState
                from divS in divState.DefaultIfEmpty()
                where employee.Id == userGuid && m.ExpirationDate == null && ds.ExpirationDate == null
                select new {divS.Id, divS.Division};
            return t.Any()
                ? new Tuple<Guid, string>(t.First().Id, t.First().Division)
                : new Tuple<Guid, string>(Guid.Empty, string.Empty);
        }

        public static Tuple<Guid,string> GetUserFullName(string userLogin)
        {
            var t = from employee in _baseModel.Employees
                where employee.AccountName.Equals(userLogin)
                select new {employee.Id, employee.Name};
            return t.Any()
                ? new Tuple<Guid, string>(t.First().Id, t.First().Name)
                : new Tuple<Guid, string>(Guid.Empty, string.Empty);
        }
    }
}