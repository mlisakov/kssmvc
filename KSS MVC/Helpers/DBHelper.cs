using System;
using System.Collections.Generic;
using System.Linq;
using KSS.Models;
using KSS.Server.Entities;
using Microsoft.Ajax.Utilities;

namespace KSS.Helpers
{
    public class DBHelper
    {
        private static CompanyBaseModel _baseModel;

        private static CompanyBaseModel BaseModel
        {
            get { return _baseModel ?? (_baseModel = new CompanyBaseModel()); }
        }

        public static DepartmentState GetDepartmentState(Guid id)
        {
            try
            {
                return BaseModel.DepartmentStates.FirstOrDefault(t => t.Id == id);
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
                return BaseModel.DepartmentStates.Where(i => i.DivisionId == id);
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
                List<PositionState> t = (from posState in BaseModel.PositionStates
                    join pos in BaseModel.Positions on posState.Id equals pos.Id into positionState
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
                return BaseModel.DivisionStates.FirstOrDefault(t => t.Id == id);
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
                return BaseModel.DivisionStates.Where(t => t.ExpirationDate == null).OrderBy(t => t.Division).ToList();
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
                return BaseModel.DepartmentStates.Where(t=>t.ExpirationDate == null).OrderBy(t => t.Department).ToList();
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
                return BaseModel.Localities;
            }
            catch (Exception)
            {

                throw;
            }

        }

        public static Dictionary<Guid, string> GetCustomLocality()
        {
            try
            {
                return GetLocality()
                    .OrderBy(t => t.Region)
                    .ThenBy(t => t.Locality1)
                    .ToDictionary(i => i.Id, j => j.Region + "," + j.Locality1);
            }
            catch (Exception)
            {

                throw;
            }

        }

        public static List<PositionState> GetPositionStates()
        {
            try
            {
                List<PositionState> list = (from staff in BaseModel.Staffs
                    join position in BaseModel.PositionStates on staff.PositionId equals position.Id into posInfo

                    from pos in posInfo.DefaultIfEmpty()

                    where pos != null && staff.ExpirationDate == null && pos.ExpirationDate == null

                    select pos
                    ).Distinct().OrderBy(t => t.Title).ToList();

                return list.Select(t => new PositionState { Id = t.Id, Title = t.Title }).ToList();
//
//
//                var f = BaseModel.PositionStates.Where(t => !string.IsNullOrEmpty(t.Title) && t.ExpirationDate == null)
//                    .OrderBy(t => t.Title)
//                    .ToList();

//                PositionState f = f4.First();

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
                DepartmentState depSt = (from employee in BaseModel.Employees
                    join staff in BaseModel.Staffs on employee.Id equals staff.Id into employeeStaff
                    from m in employeeStaff.DefaultIfEmpty()
                    join depState in BaseModel.DepartmentStates on m.DepartmentId equals depState.Id into
                        departmentState
                    from ds in departmentState.DefaultIfEmpty()
                    where employee.Id == userGuid && m.ExpirationDate == null
                    select ds).FirstOrDefault();
                if (depSt != null)
                    return depSt;
                return new DepartmentState() {Department = "-"};
            }
            catch (Exception ex)
            {
                return new DepartmentState() {Department = "Не удалось получить данные."};
            }

        }

        public static DivisionState GetEmployeeDivision(Guid userGuid)
        {
            try
            {
                DivisionState dState = (from employee in BaseModel.Employees
                    join staff in BaseModel.Staffs on employee.Id equals staff.Id into employeeStaff
                    from m in employeeStaff.DefaultIfEmpty()
                    join departmentState in BaseModel.DepartmentStates on m.DepartmentId equals departmentState.Id into
                        depState
                    from ds in depState.DefaultIfEmpty()
                    join divisionState in BaseModel.DivisionStates on ds.DivisionId equals divisionState.Id into
                        divState
                    from divS in divState.DefaultIfEmpty()
                    where employee.Id == userGuid && m.ExpirationDate == null && ds.ExpirationDate == null
                    select divS).FirstOrDefault();
                if (dState != null)
                    return dState;
                return new DivisionState() {Division = "Не задано!"};
            }
            catch (Exception)
            {
                return new DivisionState() {Division = "Не удалось получить данные!"};
            }

        }

        public static Tuple<Guid, string> GetEmployeeFullName(string userLogin)
        {
            var t = from employee in BaseModel.Employees
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
                return (from employee in BaseModel.Employees
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
                PositionState pState = (from employee in BaseModel.Employees
                    join staff in BaseModel.Staffs on employee.Id equals staff.Id into employeeStaff
                    from m in employeeStaff.DefaultIfEmpty()
                    join posState in BaseModel.PositionStates on m.PositionId equals posState.Id into positionState
                    from ps in positionState.DefaultIfEmpty()
                    where employee.Id == employeeGuid
                    select ps
                    ).FirstOrDefault();
                if (pState != null)
                    return pState;
                return new PositionState() {Title = "Не задано!"};
            }
            catch (Exception ex)
            {

                return new PositionState() {Title = "Не удалось получить данные!"};
            }

        }

        public static List<SpecificStaffPlace> GetEmployeeSpecificStaffPlaces(Guid employeeGuid)
        {
            IQueryable<SpecificStaffPlace> sSP = (from employee in BaseModel.Employees
                join staff in BaseModel.Staffs on employee.Id equals staff.Id into employeeStaff
                from m in employeeStaff.DefaultIfEmpty()
                join ss in BaseModel.SpecificStaffs on m.Id equals ss.EmployeeId into specificStaff
                from specStaff in specificStaff.DefaultIfEmpty()
                join ssp in BaseModel.SpecificStaffPlaces on specStaff.Id equals ssp.SpecificStaffId into
                    specificStaffPlace
                from specStaffPlace in specificStaffPlace.DefaultIfEmpty()
                where employee.Id == employeeGuid
                select specStaffPlace
                );
            if (sSP != null)
                return sSP.Where(i => i != null).ToList();
            return new List<SpecificStaffPlace>();
        }

        public static List<EmployeePlace> GetEmployeePlaces(Guid employeeGuid)
        {
            IQueryable<EmployeePlace> eP = (from employee in BaseModel.Employees
                join ep in BaseModel.EmployeePlaces on employee.Id equals ep.EmployeeId into employeePlase
                from empPlace in employeePlase.DefaultIfEmpty()
                where employee.Id == employeeGuid
                select empPlace
                );
            return eP.Where(i => i != null).ToList();
        }


        /// <summary>
        /// Получение количества человек в избранном для юзера
        /// </summary>
        /// <param name="employeeGuid"></param>
        /// <returns></returns>
        public static int GetFavoritesCount(Guid employeeGuid)
        {
            return
                (from fe in BaseModel.Favorites
                    join emp in BaseModel.Employees on fe.LinkedEmployeeId equals emp.Id into employees
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
                (from fe in BaseModel.Favorites
                    join emp in BaseModel.Employees on fe.LinkedEmployeeId equals emp.Id into employees
                    from e in employees.DefaultIfEmpty()
                    where fe.EmployeeId == employeeGuid
                    orderby e.Position
                    select e.Id).Skip(pageSize*startIndex).Take(pageSize).ToList();

            return favorites.Select(i => new EmployeeModel(i)).ToList();
        }

        /// <summary>
        /// Проверяет избранных на наличие адекватных значений поля Position. Упорядочивает, если необходимо
        /// </summary>
        /// <param name="guid"></param>
        public static void CheckAndOrderFavorites(Guid employeeGuid)
        {
            try
            {
                List<Employee> favoritesWithZero =
                    (from fe in BaseModel.Favorites
                        join emp in BaseModel.Employees on fe.LinkedEmployeeId equals emp.Id into employees
                        from e in employees.DefaultIfEmpty()
                        where fe.EmployeeId == employeeGuid && e.Position == 0
                        select e).ToList();

                if (favoritesWithZero.Count > 1)
                {
                    int maxPosition = GetFavoritesMaxPosition(employeeGuid);

                    if (maxPosition == 0)
                    {
                        //ситуация, когда все избранные с нулевой позицией.
                        foreach (var employee in favoritesWithZero)
                        {
                            employee.Position = maxPosition;
                            maxPosition++;
                        }
                    }
                    else
                    {
                        //ситуация, когда есть несколько избранных с нулевой позицией.
                        foreach (var employee in favoritesWithZero)
                        {
                            maxPosition++;
                            employee.Position = maxPosition;
                        }
                    }
                    BaseModel.SaveChanges();
                }
            }
            catch (Exception)
            {

                throw;
            }

        }

        /// <summary>
        /// Поиск максимальной позиции в избранных для указанного пользователя
        /// </summary>
        /// <param name="employeeGuid"></param>
        /// <returns></returns>
        public static int GetFavoritesMaxPosition(Guid employeeGuid)
        {
            return
                (from fe in BaseModel.Favorites
                    join emp in BaseModel.Employees on fe.LinkedEmployeeId equals emp.Id into employees
                    from e in employees.DefaultIfEmpty()
                    where fe.EmployeeId == employeeGuid
                    select e.Position).Max();
        }

        private static Employee GetFavoriteOnNextPosition(Guid employeeGuid, int position)
        {
            var guids =
                (from fe in BaseModel.Favorites
                    join emp in BaseModel.Employees on fe.LinkedEmployeeId equals emp.Id into employees
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
                (from fe in BaseModel.Favorites
                    join emp in BaseModel.Employees on fe.LinkedEmployeeId equals emp.Id into employees
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
                (from fe in BaseModel.Favorites
                    join emp in BaseModel.Employees on fe.LinkedEmployeeId equals emp.Id into employees
                    from e in employees.DefaultIfEmpty()
                    where fe.EmployeeId == employeeGuid && e.Position < position
                    orderby e.Position
                    select e.Id).ToList().ConvertAll(GetEmployee);
            return guids;
        }

        private static List<Employee> GetNextFavorites(Guid employeeGuid, int position)
        {
            var guids =
                (from fe in BaseModel.Favorites
                    join emp in BaseModel.Employees on fe.LinkedEmployeeId equals emp.Id into employees
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
                    BaseModel.SaveChanges();
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
                BaseModel.Favorites.Add(favoriteEmp);

                BaseModel.

                

                //set position
                var favoritePerson = BaseModel.Employees.First(e => e.Id == idFavoriteUser);
                favoritePerson.Position = maxPosition + 1;

                BaseModel.SaveChanges();
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
                    BaseModel.Favorites.First(
                        j => j.EmployeeId == idCurrentUset && j.LinkedEmployeeId == idFavoriteUser);
                BaseModel.Favorites.Remove(favoriteEmp);
                BaseModel.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public static List<EmployeeModel> Search(string employeeName, int pageSize, int startIndex)
        {
            if (string.IsNullOrEmpty(employeeName))
                employeeName = "";

            List<Employee> employees = (from employee in BaseModel.Employees
                where employee.Name.Contains(employeeName)
                select employee
                ).OrderBy(j => j.Name).Skip(pageSize*startIndex).Take(pageSize).ToList();
            var t = employees.Select(i => new EmployeeModel(i.Id)).ToList();
            return t;
        }

        public static bool CheckBirthdaysAtDay(DateTime date)
        {
            return
                BaseModel.Employees.Any(
                    t => t.BirthDay.HasValue && t.BirthDay.Value.Day == date.Day && t.BirthDay.Value.Month == date.Month);
        }

        public static List<EmployeeModel> GetBirthdayPeople(string guid)
        {
            if (string.IsNullOrEmpty(guid))
                return new List<EmployeeModel>();

            var today = DateTime.Today;
            var tommorow = today.AddMonths(1);
            var division = new Guid(guid);

            var result = (from employee in BaseModel.Employees
                join staff in BaseModel.Staffs on employee.Id equals staff.Id into employeeStaff
                where
                    employee.BirthDay.HasValue &&
                    ((employee.BirthDay.Value.Month == today.Month && employee.BirthDay.Value.Day >= today.Day) ||
                     (employee.BirthDay.Value.Month == tommorow.Month && employee.BirthDay.Value.Day <= tommorow.Day))

                from m in employeeStaff.DefaultIfEmpty()
                join departmentState in BaseModel.DepartmentStates on m.DepartmentId equals departmentState.Id into
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

        public static bool CheckIsFavorite(Guid currentUser, Guid employeeGuid)
        {
            return
                (from fe in BaseModel.Favorites
                    join emp in BaseModel.Employees on fe.LinkedEmployeeId equals emp.Id into employees
                    where fe.EmployeeId == currentUser && fe.LinkedEmployeeId == employeeGuid
                    select fe.Id).Any();
        }

        public static int GetSearchResultCount(string employeeName)
        {
            return BaseModel.Employees.Count(e => e.Name.Contains(employeeName));
        }

        public static List<EmployeeModel> SearchAdvanced(Guid? divisionId, Guid? departmentId,int pageSize, int startIndex)
        {
            List<Employee> employees = new List<Employee>();
            if (departmentId.HasValue)
            {
                if (divisionId.HasValue)
                {
                    //сотрудники из подразделения в дивизионе
                    employees = (from employee in BaseModel.Employees
                                 join staff in BaseModel.Staffs on employee.Id equals staff.Id into employeeStaff

                                 from m in employeeStaff.DefaultIfEmpty()
                                 join departmentState in BaseModel.DepartmentStates on m.DepartmentId equals departmentState.Id into
                                     depState

                                 from ds in depState.DefaultIfEmpty()
                                 join divisionState in BaseModel.DivisionStates on ds.DivisionId equals divisionState.Id into
                                     divState

                                 from divS in divState.DefaultIfEmpty()

                                 where
                                     divS.Id == divisionId && divS.ExpirationDate == null && m.ExpirationDate == null &&
                                     ds.ExpirationDate == null && ds.Id == departmentId

                                 select employee).OrderBy(j => j.Name).Skip(pageSize * startIndex).Take(pageSize).ToList();
                }
                else
                {
                    //сотрудники из подраздления
                    employees = (from employee in BaseModel.Employees
                                 join staff in BaseModel.Staffs on employee.Id equals staff.Id into employeeStaff

                                 from m in employeeStaff.DefaultIfEmpty()
                                 join departmentState in BaseModel.DepartmentStates on m.DepartmentId equals departmentState.Id into
                                     depState

                                 from ds in depState.DefaultIfEmpty()
                                 join divisionState in BaseModel.DivisionStates on ds.DivisionId equals divisionState.Id into
                                     divState

                                 from divS in divState.DefaultIfEmpty()

                                 where
                                      divS.ExpirationDate == null && m.ExpirationDate == null &&
                                     ds.ExpirationDate == null && ds.Id == departmentId

                                 select employee).OrderBy(j => j.Name).Skip(pageSize * startIndex).Take(pageSize).ToList();
                }



            }

            if (divisionId.HasValue)
            {
                //сотрудники из дивизиона (корневые департаменты)
                employees = (from employee in BaseModel.Employees
                    join staff in BaseModel.Staffs on employee.Id equals staff.Id into employeeStaff

                    from m in employeeStaff.DefaultIfEmpty()
                    join departmentState in BaseModel.DepartmentStates on m.DepartmentId equals departmentState.Id into
                        depState

                    from ds in depState.DefaultIfEmpty()
                    join divisionState in BaseModel.DivisionStates on ds.DivisionId equals divisionState.Id into
                        divState

                    from divS in divState.DefaultIfEmpty()

                    where
                        divS.Id == divisionId && divS.ExpirationDate == null && m.ExpirationDate == null &&
                        ds.ExpirationDate == null && ds.ParentId == null

                    select employee).OrderBy(j => j.Name).Skip(pageSize*startIndex).Take(pageSize).ToList();
            }


            var t = employees.Select(i => new EmployeeModel(i.Id)).ToList();
                return t;
        }

        public int GetAdvancedSearchResultCount(Guid divisionId)
        {
            return (from employee in BaseModel.Employees
                join staff in BaseModel.Staffs on employee.Id equals staff.Id into employeeStaff

                from m in employeeStaff.DefaultIfEmpty()
                join departmentState in BaseModel.DepartmentStates on m.DepartmentId equals departmentState.Id into
                    depState

                from ds in depState.DefaultIfEmpty()
                join divisionState in BaseModel.DivisionStates on ds.DivisionId equals divisionState.Id into divState

                from divS in divState.DefaultIfEmpty()

                    where divS.Id == divisionId && divS.ExpirationDate == null && m.ExpirationDate == null
                //                                             where employee.Id == userGuid && m.ExpirationDate == null && ds.ExpirationDate == null
                select employee).Count();
        }

        public static Tuple<Guid, string, bool> GetLoginingUser(string login)
        {            
            if (!string.IsNullOrEmpty(login))
            {
                var user = BaseModel.Employees.FirstOrDefault(t => string.Equals(t.AccountName, login));
                if (user != null)
                    return new Tuple<Guid, string, bool>(user.Id, user.Name,
                        user.isAdministrator.HasValue && user.isAdministrator.Value);
            }

            return new Tuple<Guid, string, bool>(Guid.Empty, "Неопознанный пользователь", false);
        }


        public static List<string> GetRegions(string country)
        {
            if (string.IsNullOrEmpty(country))
                //возвращаем всех
                return BaseModel.Localities.DistinctBy(t => t.Region).OrderBy(t=>t.Region).Select(t => t.Region).ToList();

            return BaseModel.Localities.Where(t => string.Equals(t.Country, country))
                .DistinctBy(t => t.Region)
                .OrderBy(t=>t.Region)
                .Select(t => t.Region)
                .ToList();
        }

        public static List<KeyValuePair<Guid, string>> GetLocalities(string country, string region)
        {
            IQueryable<Locality> result;
            if (string.IsNullOrEmpty(country))
            {
                if (string.IsNullOrEmpty(region))
                {
                    //все пункты, все регионы, все страны
                    result = BaseModel.Localities;
                }
                else
                    //все пункты, конкретный регион, все страны
                    result = BaseModel.Localities.Where(t => string.Equals(t.Region, region));
            }
            if (string.IsNullOrEmpty(region))
            {
                //все пункты, все регионы, конкретная страна
                result =
                    BaseModel.Localities.Where(t => string.Equals(t.Country, country));
            }
            else
            //все пункты, конкретный регион, конкретная страна
            result =
                BaseModel.Localities.Where(t =>
                    string.Equals(t.Country, country) && string.Equals(t.Region, region));
//                    .Select(t => new KeyValuePair<Guid, string>(t.Id, t.Locality1))
//                    .ToList();
            return
                result.OrderBy(t => t.Locality1)
                    .ToList()
                    .Select(t => new KeyValuePair<Guid, string>(t.Id, t.Locality1))
                    .ToList();
        }
    }
}