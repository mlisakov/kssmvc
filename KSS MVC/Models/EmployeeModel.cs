using System;
using System.Collections.Generic;
using KSS.Helpers;
using KSS.Server.Entities;

namespace KSS.Models
{
    public class EmployeeModel
    {
        public Employee Employee { get; set; }
        public DepartmentState DepartmentState { get; set; }
        public DivisionState DivisionState { get; set; }
        public PositionState PositionState { get; set; }
        public List<SpecificStaffPlace> SpecificStaffPlaces { get; set; }
        public List<EmployeePlace> EmployeePlaces { get; set; }

        public EmployeeModel(Guid employeeGuid)
        {
            Employee = DBHelper.GetEmployee(employeeGuid);
            DepartmentState = DBHelper.GetEmployeeDepartment(employeeGuid);
            DivisionState = DBHelper.GetEmployeeDivision(employeeGuid);
            PositionState = DBHelper.GetEmployeePositionState(employeeGuid);
            SpecificStaffPlaces = DBHelper.GetEmployeeSpecificStaffPlaces(employeeGuid);
            EmployeePlaces = DBHelper.GetEmployeePlaces(employeeGuid);
        }

        public EmployeeModel(Guid employeeGuid, bool loadDepartmentInfo,
            bool loadDivision,
            bool loadPosition,
            bool loadSpecificStaffs,
            bool loadEmployeePlace)
        {
            Employee = DBHelper.GetEmployee(employeeGuid);

            if (loadDepartmentInfo)
                DepartmentState = DBHelper.GetEmployeeDepartment(employeeGuid);

            if (loadDivision)
                DivisionState = DBHelper.GetEmployeeDivision(employeeGuid);

            if (loadPosition)
                PositionState = DBHelper.GetEmployeePositionState(employeeGuid);

            if (loadSpecificStaffs)
                SpecificStaffPlaces = DBHelper.GetEmployeeSpecificStaffPlaces(employeeGuid);

            if (loadEmployeePlace)
                EmployeePlaces = DBHelper.GetEmployeePlaces(employeeGuid);
        }
    }
}