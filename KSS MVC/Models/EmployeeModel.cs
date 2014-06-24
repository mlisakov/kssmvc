using System;
using System.Collections.Generic;
using KSS.Helpers;
using KSS.Server.Entities;

namespace KSS.Models
{
    public class EmployeeModel
    {
        private readonly Guid _currentUser = Guid.Empty;

        public Employee Employee { get; set; }
        public DepartmentState DepartmentState { get; set; }
        public DivisionState DivisionState { get; set; }
        public PositionState PositionState { get; set; }
        public List<SpecificStaffPlace> SpecificStaffPlaces { get; set; }
        public List<EmployeePlace> EmployeePlaces { get; set; }

        public bool IsFavorite { get; private set; }

        public EmployeeModel(Guid employeeGuid)
        {
            Employee = DBHelper.GetEmployee(employeeGuid);
            DepartmentState = DBHelper.GetEmployeeDepartment(employeeGuid);
            DivisionState = DBHelper.GetEmployeeDivision(employeeGuid);
            PositionState = DBHelper.GetEmployeePositionState(employeeGuid);
            SpecificStaffPlaces = DBHelper.GetEmployeeSpecificStaffPlaces(employeeGuid);
            EmployeePlaces = DBHelper.GetEmployeePlaces(employeeGuid);
         
        }

        public EmployeeModel(Guid employeeGuid, Guid currentUser)
            : this(employeeGuid)
        {
            IsFavorite = DBHelper.CheckIsFavorite(currentUser, employeeGuid);
            _currentUser = currentUser;
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

        public void ChangeFavoriteStatus()
        {
            if (_currentUser != Guid.Empty)
            {
                bool result = false;
                result = IsFavorite
                    ? DBHelper.RemoveFromFavorites(_currentUser, Employee.Id)
                    : DBHelper.AddToFavorites(_currentUser, Employee.Id);

                if (result)
                    IsFavorite = !IsFavorite;
            }
        }

        public void LoadLocation()
        {
            
        }
    }
}