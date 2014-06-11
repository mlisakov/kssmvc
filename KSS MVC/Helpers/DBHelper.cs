﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web.Management;
using System.Web.UI;
using KSS.Models;
using KSS.Server.Entities;

namespace KSS.Helpers
{
    public class DBHelper
    {
        private static CompanyBaseModel _baseModel;

        private static CompanyBaseModel baseModel
        {
           get { return _baseModel ?? (_baseModel = new CompanyBaseModel()); }
        }

        public static DepartmentState GetDepartmentState(Guid id)
        {
            try
            {
                return baseModel.DepartmentStates.FirstOrDefault(t => t.Id == id);
            }
            catch (Exception)
            {

                throw;
            }

        }

        public static IEnumerable<DepartmentState> GetDepartmentStatesByDivision(Guid id)
        {
            try
            {
                return baseModel.DepartmentStates.Where(i => i.DivisionId == id);
            }
            catch (Exception)
            {

                throw;
            }

        }

        public static DivisionState GetDivisionState(Guid id)
        {
            try
            {
                return baseModel.DivisionStates.FirstOrDefault(t => t.Id == id);
            }
            catch (Exception)
            {

                throw;
            }

        }

        public static List<DivisionState> GetDivisionStates()
        {
            try
            {
                return baseModel.DivisionStates.ToList();
            }
            catch (Exception)
            {

                throw;
            }

        }

        public static List<DepartmentState> GetDepartmentStates()
        {
            try
            {
                return baseModel.DepartmentStates.ToList();
            }
            catch (Exception)
            {

                throw;
            }

        }

        public static DepartmentState GetEmployeeDepartment(Guid userGuid)
        {
            try
            {
                DepartmentState depSt = (from employee in baseModel.Employees
                                      join staff in baseModel.Staffs on employee.Id equals staff.Id into employeeStaff
                                      from m in employeeStaff.DefaultIfEmpty()
                                      join depState in baseModel.DepartmentStates on m.DepartmentId equals depState.Id into
                                          departmentState
                                      from ds in departmentState.DefaultIfEmpty()
                                      where employee.Id == userGuid && m.ExpirationDate == null
                                      select ds).FirstOrDefault();
                if (depSt != null)
                    return depSt;
                return new DepartmentState() {Department = "Не задано!"};
            }
            catch (Exception ex)
            {
                return new DepartmentState() { Department = "Не удалось получить данные!" };
            }

        }

        public static DivisionState GetEmployeeDivision(Guid userGuid)
        {
            try
            {
                DivisionState dState=(from employee in baseModel.Employees
                    join staff in baseModel.Staffs on employee.Id equals staff.Id into employeeStaff
                from m in employeeStaff.DefaultIfEmpty()
                    join departmentState in baseModel.DepartmentStates on m.DepartmentId equals departmentState.Id into
                    depState
                from ds in depState.DefaultIfEmpty()
                    join divisionState in baseModel.DivisionStates on ds.DivisionId equals divisionState.Id into divState
                from divS in divState.DefaultIfEmpty()
                where employee.Id == userGuid && m.ExpirationDate == null && ds.ExpirationDate == null
                select divS).FirstOrDefault();
                if (dState != null)
                    return dState;
                return new DivisionState(){Division = "Не задано!"};
            }
            catch (Exception)
            {
                return new DivisionState() { Division = "Не удалось получить данные!" };
            }

        }

        public static Tuple<Guid, string> GetEmployeeFullName(string userLogin)
        {
            var t = from employee in baseModel.Employees
                where employee.AccountName.Equals(userLogin)
                select new {employee.Id, employee.Name};
            return t.Any()
                ? new Tuple<Guid, string>(t.First().Id, t.First().Name)
                : new Tuple<Guid, string>(Guid.Empty, string.Empty);
        }

        public static Employee GetEmployee(Guid employeeGuid)
        {
            try
            {
                return (from employee in baseModel.Employees
                    where employee.Id.Equals(employeeGuid)
                    select employee).FirstOrDefault();
            }
            catch (Exception)
            {
                return null;
            }

        }

        public static PositionState GetEmployeePositionState(Guid employeeGuid)
        {
            try
            {
                PositionState pState=(from employee in baseModel.Employees
                    join staff in baseModel.Staffs on employee.Id equals staff.Id into employeeStaff
                from m in employeeStaff.DefaultIfEmpty()
                    join posState in baseModel.PositionStates on m.PositionId equals posState.Id into positionState
                from ps in positionState.DefaultIfEmpty()
                where employee.Id == employeeGuid
                select ps
                ).FirstOrDefault();
                if (pState != null)
                    return pState;
                return new PositionState() { Title = "Не задано!" };
            }
            catch (Exception ex)
            {

                return new PositionState() { Title = "Не удалось получить данные!" };
            }

        }

        public static List<SpecificStaffPlace> GetEmployeeSpecificStaffPlaces(Guid employeeGuid)
        {
            IQueryable<SpecificStaffPlace> sSP=(from employee in baseModel.Employees
                    join staff in baseModel.Staffs on employee.Id equals staff.Id into employeeStaff
                from m in employeeStaff.DefaultIfEmpty()
                    join ss in baseModel.SpecificStaffs on m.Id equals ss.EmployeeId into specificStaff
                from specStaff in specificStaff.DefaultIfEmpty()
                    join ssp in baseModel.SpecificStaffPlaces on specStaff.Id equals ssp.SpecificStaffId into
                    specificStaffPlace
                from specStaffPlace in specificStaffPlace.DefaultIfEmpty()
                where employee.Id == employeeGuid
                select specStaffPlace
                );
            if (sSP != null)
                return sSP.Where(i=>i!=null).ToList();
            return new List<SpecificStaffPlace>();
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
            IQueryable<EmployeePlace> eP=(from employee in baseModel.Employees
                    join ep in baseModel.EmployeePlaces on employee.Id equals ep.EmployeeId into employeePlase
                from empPlace in employeePlase.DefaultIfEmpty()
                where employee.Id == employeeGuid
                select empPlace
                );
            if (eP != null)
                return eP.Where(i => i != null).ToList();
            return new List<EmployeePlace>();
        }

        public static List<EmployeeModel> GetFavorites(Guid employeeGuid)
        {
            List<Guid> favorites =
                (from fe in baseModel.Favorites
                 join emp in baseModel.Employees on fe.LinkedEmployeeId equals emp.Id into employees
                    from e in employees.DefaultIfEmpty()
                    where fe.EmployeeId == employeeGuid
                    select e.Id).ToList();
            return favorites.Select(i => new EmployeeModel(i)).ToList();
        }

        public static bool AddToFavorites(Guid idCurrentUset, Guid idFavoriteUser)
        {
            try
            {
                Favorite favoriteEmp =new Favorite(){EmployeeId = idCurrentUset,LinkedEmployeeId = idFavoriteUser};
                baseModel.Favorites.Add(favoriteEmp);
                baseModel.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public static bool RemoveFromFavorites(Guid idCurrentUset, Guid idFavoriteUser)
        {
            try
            {
                Favorite favoriteEmp =
                    baseModel.Favorites.First(
                        j => j.EmployeeId == idCurrentUset && j.LinkedEmployeeId == idFavoriteUser);
                baseModel.Favorites.Remove(favoriteEmp);
                baseModel.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public static List<EmployeeModel> Search(string employeeName,int pageSize,int startIndex)
        {
            List<Employee> employees= (from employee in baseModel.Employees
                    where employee.Name.Contains(employeeName)
                    select employee
                ).OrderBy(j=>j.Name).Skip(pageSize*startIndex).Take(pageSize).ToList();
            var t = employees.Select(i => new EmployeeModel(i.Id)).ToList();
            return t;
        }
    }
}