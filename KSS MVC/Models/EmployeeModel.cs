using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using KSS.Helpers;
using KSS.Server.Entities;

namespace KSS.Models
{
    public class EmployeeModel
    {
        private readonly Guid _currentUser = Guid.Empty;

        /// <summary>
        /// Инфа о сотруднике
        /// </summary>
        public Employee Employee { get; set; }
        /// <summary>
        /// Инфа о департаменте
        /// </summary>
        public DepartmentState DepartmentState { get; set; }
        /// <summary>
        /// Инфа о дивизионе
        /// </summary>
        public DivisionState DivisionState { get; set; }
        /// <summary>
        /// Инфа о работе
        /// </summary>
        public PositionState PositionState { get; set; }
        /// <summary>
        /// Специфичные места
        /// </summary>
        public List<SpecificStaffPlace> SpecificStaffPlaces { get; set; }
        /// <summary>
        /// Места
        /// </summary>
        public List<EmployeePlace> EmployeePlaces { get; set; }


        /// <summary>
        /// Локация
        /// </summary>
        public Location Location { get; private set; }
        /// <summary>
        /// Основное место
        /// </summary>
        public EmployeePlace Place { get; private set; }

        public bool IsFavorite { get; private set; }

        [Required, FileExtensions(Extensions = "png",
             ErrorMessage = "Specify a CSV file. (Comma-separated values)")]
        public HttpPostedFileBase File { get; set; }

        public EmployeeModel(Guid employeeGuid, Guid currentUser)
        {
            Employee = DBHelper.GetEmployee(employeeGuid);
            DepartmentState = DBHelper.GetEmployeeDepartment(employeeGuid);
            DivisionState = DBHelper.GetEmployeeDivision(employeeGuid);
            PositionState = DBHelper.GetEmployeePositionState(employeeGuid);
            SpecificStaffPlaces = DBHelper.GetEmployeeSpecificStaffPlaces(employeeGuid);
            EmployeePlaces = DBHelper.GetEmployeePlaces(employeeGuid);

            if (EmployeePlaces.Any(t => t.Location != null))
            {
                Place = EmployeePlaces.First(t => t.Location != null);
                Location = Place.Location;
            }

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
            {
                EmployeePlaces = DBHelper.GetEmployeePlaces(employeeGuid);
                if (EmployeePlaces.Any(t => t.Location != null))
                {
                    Location = EmployeePlaces.First(t => t.Location != null).Location;
                }                
            }
        }

        public string GetImage()
        {
            return DBHelper.GetEmployeePhoto(Employee.Id);
        }

        /// <summary>
        /// Добавление/удаление избранного. 
        /// </summary>
        /// <returns>Статус человека. True - если он в избранном, false - нет.</returns>
        public bool ChangeFavoriteStatus()
        {
            if (_currentUser != Guid.Empty)
            {
                bool result = IsFavorite
                    ? DBHelper.RemoveFromFavorites(_currentUser, Employee.Id)
                    : DBHelper.AddToFavorites(_currentUser, Employee.Id);

                if (result)
                    IsFavorite = !IsFavorite;
            }
            return IsFavorite;
        }

//        public bool LoadLocation()
//        {
//            var f = DBHelper.GetEmployeePlaces(_currentUser);
//            return true;
//        }

        public List<DepartmentState> GetFullDepartmentName()
        {
            var departments = new List<DepartmentState>();
            if (DepartmentState != null)
                GetFullDepartmentName(departments, DepartmentState.Id);
            return departments;
        }

        private void GetFullDepartmentName(List<DepartmentState> departments, Guid? departmentGuid)
        {
            if (departmentGuid.HasValue && departmentGuid.Value!= Guid.Empty)
            {
                var department = DBHelper.GetDepartmentState(departmentGuid.Value);
                departments.Add(department);
                GetFullDepartmentName(departments, department.ParentId);
            }
        }

        
        public List<string> GetRegions()
        {
            return DBHelper.GetRegions(DivisionState.Id);
        }
    }
}