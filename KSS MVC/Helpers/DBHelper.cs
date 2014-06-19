using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Management;
using System.Web.UI;
using System.Web.UI.WebControls.Expressions;
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

        public static IEnumerable<PositionState> GetPositionStatesByDepartment(Guid id)
        {
            try
            {
                List<PositionState> t = (from posState in baseModel.PositionStates
                    join pos in baseModel.Positions on posState.Id equals pos.Id into positionState
                    from m in positionState.DefaultIfEmpty()
                    where m.DepartmentId == id
                    select posState).ToList();
                return t;

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

        public static IEnumerable<Locality> GetLocality()
        {
            try
            {
                return baseModel.Localities;
            }
            catch (Exception)
            {

                throw;
            }

        }

        public static Dictionary<Guid,string> GetCustomLocality()
        {
            try
            {
                return GetLocality().ToDictionary(i=>i.Id,j=>j.Region+","+j.Locality1);
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

//        public static List<EmployeeModel> GetFavorites(Guid employeeGuid)
//        {
//            List<Guid> favorites =
//                (from fe in baseModel.Favorites
//                 join emp in baseModel.Employees on fe.LinkedEmployeeId equals emp.Id into employees
//                    from e in employees.DefaultIfEmpty()
//                    where fe.EmployeeId == employeeGuid
//                    select e.Id).ToList();
//            return favorites.Select(i => new EmployeeModel(i)).ToList();
//        }

        /// <summary>
        /// Получение количества человек в избранном для юзера
        /// </summary>
        /// <param name="employeeGuid"></param>
        /// <returns></returns>
        public static int GetFavoritesCount(Guid employeeGuid)
        {
            return
                (from fe in baseModel.Favorites
                    join emp in baseModel.Employees on fe.LinkedEmployeeId equals emp.Id into employees
                    from e in employees.DefaultIfEmpty()
                    where fe.EmployeeId == employeeGuid
                    orderby e.Position
                    select e.Id).Count();
        }

        /// <summary>
        /// Получение списка избранных для юзера, начиная с startIndex и количеством pageSize
        /// </summary>
        /// <param name="employeeGuid"></param>
        /// <param name="pageSize"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public static List<EmployeeModel> GetFavorites(Guid employeeGuid, int pageSize, int startIndex)
        {
            List<Guid> favorites =
                (from fe in baseModel.Favorites
                    join emp in baseModel.Employees on fe.LinkedEmployeeId equals emp.Id into employees
                    from e in employees.DefaultIfEmpty()
                    where fe.EmployeeId == employeeGuid
                    orderby e.Position
                    select e.Id).Skip(pageSize*startIndex).Take(pageSize).ToList();

            return favorites.Select(i => new EmployeeModel(i)).ToList();
        }

        /// <summary>
        /// Поиск максимальной позиции в избранных для указанного пользователя
        /// </summary>
        /// <param name="employeeGuid"></param>
        /// <returns></returns>
        public static int GetFavoritesMaxPosition(Guid employeeGuid)
        {
            return
                (from fe in baseModel.Favorites
                    join emp in baseModel.Employees on fe.LinkedEmployeeId equals emp.Id into employees
                    from e in employees.DefaultIfEmpty()                    
                    where fe.EmployeeId == employeeGuid
                    select e.Position).Max();
        }

        private static Employee GetFavoriteOnNextPosition(Guid employeeGuid, int position)
        {
            var guids =
                (from fe in baseModel.Favorites
                    join emp in baseModel.Employees on fe.LinkedEmployeeId equals emp.Id into employees
                    from e in employees.DefaultIfEmpty()
                    where fe.EmployeeId == employeeGuid && e.Position > position
                    orderby e.Position
                    select e.Id);
            if (guids.Any())
            {
                return GetEmployee(guids.First());
            }
            return null;
        }

        private static Employee GetFavoriteOnPreviousPosition(Guid employeeGuid, int position)
        {
            var guids =
                (from fe in baseModel.Favorites
                    join emp in baseModel.Employees on fe.LinkedEmployeeId equals emp.Id into employees
                    from e in employees.DefaultIfEmpty()
                    where fe.EmployeeId == employeeGuid && e.Position < position
                    orderby e.Position
                    select e.Id).ToList();
            if (guids.Any())
            {
                return GetEmployee(guids.Last());
            }
            return null;
        }

        private static List<Employee> GetPreviousFavorites(Guid employeeGuid, int position)
        {
            var guids =
                (from fe in baseModel.Favorites
                    join emp in baseModel.Employees on fe.LinkedEmployeeId equals emp.Id into employees
                    from e in employees.DefaultIfEmpty()
                    where fe.EmployeeId == employeeGuid && e.Position < position
                    orderby e.Position
                    select e.Id).ToList().ConvertAll(GetEmployee);
            return guids;
        }

        private static List<Employee> GetNextFavorites(Guid employeeGuid, int position)
        {
            var guids =
                (from fe in baseModel.Favorites
                 join emp in baseModel.Employees on fe.LinkedEmployeeId equals emp.Id into employees
                 from e in employees.DefaultIfEmpty()
                 where fe.EmployeeId == employeeGuid && e.Position > position
                 orderby e.Position
                 select e.Id).ToList().ConvertAll(GetEmployee);
            return guids;
        }

        /// <summary>
        /// Смена позиции у избранного
        /// </summary>
        /// <param name="user">гуид пользователя</param>
        /// <param name="favorite">гуид избранного, которому меняем позицию</param>
        /// <param name="delta">Смещение. +1 (вниз), -1 (вверх), 0 (самый верх), int.MaxValue (самый низ) </param>
        public static void UpdateFavoritePosition(Guid user, Guid favorite, int delta)
        {
            //найти избранного
            var favoriteUser = GetEmployee(favorite);

            int temp = favoriteUser.Position;
            bool hasChanges = false;
            switch (delta)
            {
                case 1:
                    var nextUser = GetFavoriteOnNextPosition(user, favoriteUser.Position);
                    if (nextUser != null)
                    {
                        favoriteUser.Position = nextUser.Position;
                        nextUser.Position = temp;
                        hasChanges = true;
                    }
                    break;
                case -1:
                    var previousUser = GetFavoriteOnPreviousPosition(user, favoriteUser.Position);
                    if (previousUser != null)
                    {
                        favoriteUser.Position = previousUser.Position;
                        previousUser.Position = temp;
                        hasChanges = true;
                    }
                    break;
                case 0:
                    var list = GetPreviousFavorites(user, favoriteUser.Position);
                    list.Insert(0, favoriteUser);

                    for (int i = 0; i < list.Count - 1; i++)
                    {
                        int index = list[i].Position;
                        list[i].Position = list[i + 1].Position;
                        list[i + 1].Position = index;
                    }
                    if (list.Count > 1)
                        hasChanges = true;
                    break;
                case int.MaxValue:
                    var nextList = GetNextFavorites(user, favoriteUser.Position);
                    nextList.Add(favoriteUser);

                    for (int i = nextList.Count - 1; i > 0; i--)
                    {
                        int index = nextList[i].Position;
                        nextList[i].Position = nextList[i - 1].Position;
                        nextList[i - 1].Position = index;
                    }
                    if (nextList.Count > 1)
                        hasChanges = true;
                    break;
            }

            try
            {

                if (hasChanges)
                    baseModel.SaveChanges();
            }
            catch (Exception)
            {
             //проблемы с сохранением из-за отсутствия ключей в базе!   
//                throw;
            }

        }

        public static bool AddToFavorites(Guid idCurrentUser, Guid idFavoriteUser)
        {
            try
            {
                var maxPosition = GetFavoritesMaxPosition(idCurrentUser);

                //add favorites
                var favoriteEmp = new Favorite {EmployeeId = idCurrentUser, LinkedEmployeeId = idFavoriteUser};
                baseModel.Favorites.Add(favoriteEmp);

                //set position
                var favoritePerson = baseModel.Employees.First(e => e.Id == idFavoriteUser);
                favoritePerson.Position = maxPosition + 1;

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

        public static bool CheckBirthdaysAtDay(DateTime date)
        {
            return
                baseModel.Employees.Any(
                    t => t.BirthDay.HasValue && t.BirthDay.Value.Day == date.Day && t.BirthDay.Value.Month == date.Month);
        }

        public static List<EmployeeModel> GetBirthdayPeople(string guid)
        {
            if (string.IsNullOrEmpty(guid))
                return new List<EmployeeModel>();

            var today = DateTime.Today;
            var tommorow = today.AddMonths(1);
            var division = new Guid(guid);

            var result = (from employee in baseModel.Employees
                join staff in baseModel.Staffs on employee.Id equals staff.Id into employeeStaff
                where
                    employee.BirthDay.HasValue &&
                    ((employee.BirthDay.Value.Month == today.Month && employee.BirthDay.Value.Day >= today.Day) ||
                     (employee.BirthDay.Value.Month == tommorow.Month && employee.BirthDay.Value.Day <= tommorow.Day))

                from m in employeeStaff.DefaultIfEmpty()
                join departmentState in baseModel.DepartmentStates on m.DepartmentId equals departmentState.Id into
                    depState

                from ds in depState.DefaultIfEmpty()
                where ds.DivisionId.Equals(division) && ds.ExpirationDate == null
                select employee.Id).ToList();

            return
                result.Select(t => new EmployeeModel(t, true, false, true, false, false))
                    .OrderBy(t => t.Employee.BirthDay.Value.Month)
                    .ThenBy(t => t.Employee.BirthDay.Value.Day)
                    .ToList();
        }
    }
}