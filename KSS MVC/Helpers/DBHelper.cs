using System;
using System.Collections.Generic;
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

        public static DepartmentState GetEmployeeDepartment(Guid userGuid)
        {
            return (from employee in _baseModel.Employees
                    join staff in _baseModel.Staffs on employee.Id equals staff.Id into employeeStaff
                    from m in employeeStaff.DefaultIfEmpty()
                    join depState in _baseModel.DepartmentStates on m.DepartmentId equals depState.Id into departmentState
                    from ds in departmentState.DefaultIfEmpty()
                    where employee.Id == userGuid && m.ExpirationDate == null
                    select ds).First();
        }

        public static DivisionState GetEmployeeDivision(Guid userGuid)
        {
            return (from employee in _baseModel.Employees
                    join staff in _baseModel.Staffs on employee.Id equals staff.Id into employeeStaff
                    from m in employeeStaff.DefaultIfEmpty()
                    join departmentState in _baseModel.DepartmentStates on m.DepartmentId equals departmentState.Id into
                        depState
                    from ds in depState.DefaultIfEmpty()
                    join divisionState in _baseModel.DivisionStates on ds.DivisionId equals divisionState.Id into divState
                    from divS in divState.DefaultIfEmpty()
                    where employee.Id == userGuid && m.ExpirationDate == null && ds.ExpirationDate == null
                    select divS).First();
        }

        public static Tuple<Guid,string> GetEmployeeFullName(string userLogin)
        {
            var t = from employee in _baseModel.Employees
                where employee.AccountName.Equals(userLogin)
                select new {employee.Id, employee.Name};
            return t.Any()
                ? new Tuple<Guid, string>(t.First().Id, t.First().Name)
                : new Tuple<Guid, string>(Guid.Empty, string.Empty);
        }

        public static Employee GetEmployee(Guid employeeGuid)
        {
            return (from employee in _baseModel.Employees
                   where employee.Id.Equals(employeeGuid)
                   select employee).First();
        }

        public static PositionState GetEmployeePositionState(Guid employeeGuid)
        {
            return (from employee in _baseModel.Employees
                join staff in _baseModel.Staffs on employee.Id equals staff.Id into employeeStaff
                from m in employeeStaff.DefaultIfEmpty()
                join posState in _baseModel.PositionStates on m.PositionId equals posState.Id into positionState
                from ps in positionState.DefaultIfEmpty()
                where employee.Id == employeeGuid
                select ps
                ).First();
        }

        public static List<SpecificStaffPlace> GetEmployeeSpecificStaffPlaces(Guid employeeGuid)
        {
            return (from employee in _baseModel.Employees
                join staff in _baseModel.Staffs on employee.Id equals staff.Id into employeeStaff
                from m in employeeStaff.DefaultIfEmpty()
                join ss in _baseModel.SpecificStaffs on m.Id equals ss.EmployeeId into specificStaff
                from specStaff in specificStaff.DefaultIfEmpty()
                join ssp in _baseModel.SpecificStaffPlaces on specStaff.Id equals ssp.SpecificStaffId into
                    specificStaffPlace
                from specStaffPlace in specificStaffPlace.DefaultIfEmpty()
                where employee.Id == employeeGuid
                select specStaffPlace
                ).ToList();
        }

        //public static List<SpecificStaff> GetEmployeeSpecificStaffs(Guid employeeGuid)
        //{
        //    return (from employee in _baseModel.Employees
        //            join staff in _baseModel.Staffs on employee.Id equals staff.Id into employeeStaff
        //            from m in employeeStaff.DefaultIfEmpty()
        //            join ss in _baseModel.SpecificStaffs on m.Id equals ss.EmployeeId into specificStaff
        //            from specStaff in specificStaff.DefaultIfEmpty()
        //            select specStaff
        //        ).ToList();
        //}

        public static List<EmployeePlace> GetEmployeePlaces(Guid employeeGuid)
        {
            return (from employee in _baseModel.Employees
                join ep in _baseModel.EmployeePlaces on employee.Id equals ep.EmployeeId into employeePlase
                from empPlace in employeePlase.DefaultIfEmpty()
                where employee.Id == employeeGuid
                select empPlace
                ).ToList();
        }
    }
}