using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using KSS.Helpers;
using KSS.Server.Entities;

namespace KSS.Models
{
    public class SpecificChangableModel
    {
//        public SpecificStaff SpecificStaff { get; set; }
//
//        public string Address { get; private set; }
//
//
//        /// <summary>
//        /// Места
//        /// </summary>
//        public List<SpecificStaffPlace> SpecificStaffPlaces { get; set; }
//
//        public Location SpecificStaffLocation { get; private set; }
//        public SpecificStaffPlace SpecificStaffPlaceWithLocation { get; private set; }
//
//
//        public Employee RelatedEmployee
//        {
//            get { return SpecificStaff.Employee; }
//        }
//
//        public PositionState EmployeePositionState { get; private set; }
//        /// <summary>
//        /// Инфа о департаменте
//        /// </summary>
//        public DepartmentState EmployeeDepartmentState { get; set; }
//
//        
//
        public SpecificChangableModel(HttpSessionStateBase session)
        {
//            SpecificStaff = DBHelper.GetSpecificStaff(id);
//            BindPlaceString();
//
//            SpecificStaffPlaces = SpecificStaff.SpecificStaffPlaces.ToList();
//
//            if (RelatedEmployee != null)
//            {                
//                EmployeePositionState = DBHelper.GetEmployeePositionState(RelatedEmployee.Id);
//                EmployeeDepartmentState = DBHelper.GetEmployeeDepartment(RelatedEmployee.Id);
//            }
            
        }
//
//        public string GetImage()
//        {
//            if (RelatedEmployee != null)
//                return DBHelper.GetEmployeePhoto(RelatedEmployee.Id);
//
//            return DBHelper.GetEmployeePhoto(Guid.Empty);
//        }
//
//
//        private void BindPlaceString()
//        {
//            SpecificStaffPlaceWithLocation = SpecificStaff.SpecificStaffPlaces.FirstOrDefault(t => t.Location != null);
//            if (SpecificStaffPlaceWithLocation != null)
//            {
//                SpecificStaffLocation = SpecificStaffPlaceWithLocation.Location;
//                var sb = new StringBuilder();
//                if (SpecificStaffPlaceWithLocation.Location.Locality != null)
//                {
//                    sb.Append(SpecificStaffPlaceWithLocation.Location.Locality.Country);
//                    sb.Append(", ");
//                    sb.Append(SpecificStaffPlaceWithLocation.Location.Locality.Region);
//                    sb.Append(", ");
//                    sb.Append(SpecificStaffPlaceWithLocation.Location.Locality.Locality1);
//                    sb.Append(", ");
//                }
//                sb.Append(SpecificStaffPlaceWithLocation.Location.Street);
//                sb.Append(", ");
//                sb.Append(SpecificStaffPlaceWithLocation.Location.Edifice);
//
//                Address = sb.ToString();
//            }
//        }
//
//        public List<DepartmentState> GetEmployeeFullDepartmentName()
//        {
//            var departments = new List<DepartmentState>();
//            if (EmployeeDepartmentState != null)
//                GetFullDepartmentName(departments, EmployeeDepartmentState.Id);
//            return departments;
//        }
//
//        private void GetFullDepartmentName(List<DepartmentState> departments, Guid? departmentGuid)
//        {
//            if (departmentGuid.HasValue)
//            {
//                var department = DBHelper.GetDepartmentState(departmentGuid.Value);
//                departments.Add(department);
//                GetFullDepartmentName(departments, department.ParentId);
//            }
//        }
//
//        public List<KeyValuePair<Guid, string>> GetPersonsInDivision()
//        {
//            var divisionId = SpecificStaff.DepartmentSpecificState.DivisionId;
//
//            var result = DBHelper.GetPersonsInDivision(divisionId);
//            
//            return result.Select(t => new KeyValuePair<Guid, string>(t.Id, t.Name)).ToList();
//        }

    }
}