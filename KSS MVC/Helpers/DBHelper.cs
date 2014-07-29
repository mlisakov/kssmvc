using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using KSS.Models;
using KSS.Properties;
using KSS.Server.Entities;
using Microsoft.Ajax.Utilities;

namespace KSS.Helpers
{
// ReSharper disable once InconsistentNaming
    public class DBHelper
    {
        private static CompanyBaseModel _baseModel;

        private static CompanyBaseModel BaseModel
        {
            get { return _baseModel ?? (_baseModel = new CompanyBaseModel()); }
        }

        /// <summary>
        /// Получение департамента по его ид
        /// </summary>
        /// <param name="id">Ид департамента</param>
        /// <returns></returns>
        public static DepartmentState GetDepartmentState(Guid id)
        {
            try
            {
                return BaseModel.DepartmentStates.FirstOrDefault(t => t.Id == id);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Ошибка при получении департамента по его ИД. GetDepartmentState.", ex);
            }
            return null;
        }

        /// <summary>
        /// Получение списка департаментов по ид дивизиона
        /// </summary>
        /// <param name="id">Ид дивизиона</param>
        /// <returns></returns>
        public static IEnumerable<DepartmentState> GetDepartmentStatesByDivision(Guid id)
        {
            try
            {
                return
                    BaseModel.DepartmentStates.Where(i => i.DivisionId == id && i.ExpirationDate == null)
                        .OrderBy(t => t.Department);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Ошибка при получении списка департаментов по ИД дивизиона. GetDepartmentStatesByDivision.", ex);
            }
            return new List<DepartmentState>();
        }

        /// <summary>
        /// Получение списка профессий, по которым работают в департаменте с указанным ИД
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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
            catch (Exception ex)
            {
                LogHelper.WriteLog(
                    "Ошибка при получении списка профессий, по которым работают в департаменте с указанным ИД. GetPositionStatesByDepartment.", ex);
            }
            return new List<PositionState>();
        }

        /// <summary>
        /// Получение дивизиона по его ИД
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static DivisionState GetDivisionState(Guid id)
        {
            try
            {
                return BaseModel.DivisionStates.FirstOrDefault(t => t.Id == id);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Ошибка при получении дивизиона по его ИД. GetDivisionState.", ex);
            }
            return null;
        }

        /// <summary>
        /// Получение списка дивизионов
        /// </summary>
        /// <returns></returns>
        public static List<DivisionState> GetDivisionStates()
        {
            try
            {
                return BaseModel.DivisionStates.Where(t => t.ExpirationDate == null).OrderBy(t => t.Division).ToList();
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Ошибка при получении списка дивизионов. GetDivisionStates.", ex);
            }

            return new List<DivisionState>();
        }

        /// <summary>
        /// Получение списка всех департаментов
        /// </summary>
        /// <returns></returns>
        public static List<DepartmentState> GetDepartmentStates()
        {
            try
            {
                return BaseModel.DepartmentStates.Where(t=>t.ExpirationDate == null).OrderBy(t => t.Department).ToList();
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Ошибка при получении списка всех департаментов. GetDepartmentStates.", ex);
            }
            return new List<DepartmentState>();
        }

        /// <summary>
        /// Получение списка населенных пунктов
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Locality> GetLocality()
        {
            try
            {
                return BaseModel.Localities;
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Ошибка при получении списка населенных пунктов. GetLocality.", ex);
            }

            return new List<Locality>();
        }

        /// <summary>
        /// Получение словаря: ключ - ИД населенного пункта, значение - его наименование
        /// </summary>
        /// <returns></returns>
        public static Dictionary<Guid, string> GetCustomLocality()
        {
            try
            {
                return GetLocality()
                    .OrderBy(t => t.Region)
                    .ThenBy(t => t.Locality1)
                    .ToDictionary(i => i.Id, j => j.Region + "," + j.Locality1);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Ошибка при получении списка населенных пунктов. GetCustomLocality.", ex);
            }

            return new Dictionary<Guid, string>();
        }

        /// <summary>
        /// Получение списка профессий
        /// </summary>
        /// <returns></returns>
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

                return list.Select(t => new PositionState {Id = t.Id, Title = t.Title}).ToList();
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Ошибка при получении списка профессий. GetCustomLocality.", ex);
            }

            return new List<PositionState>();
        }

        /// <summary>
        /// Получение департамента, в котором работает указанный сотрудник
        /// </summary>
        /// <param name="userGuid"></param>
        /// <returns></returns>
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
                return new DepartmentState {Department = "-"};
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(
                    "Ошибка при получении департамента, в котором работает указанный сотрудник. GetEmployeeDepartment.", ex);
            }
            return new DepartmentState {Department = "-"};
        }

        /// <summary>
        /// Получение дивизиона, в котором работает указанный сотрудник
        /// </summary>
        /// <param name="userGuid"></param>
        /// <returns></returns>
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
                return new DivisionState {Division = "-"};
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(
                    "Ошибка при получении дивизиона, в котором работает указанный сотрудник. GetEmployeeDivision.", ex);

            }
            return new DivisionState {Division = "Не удалось получить данные!"};
        }

        /// <summary>
        /// Получение полного имени авторизующегося пользователя
        /// </summary>
        /// <param name="userLogin"></param>
        /// <returns></returns>
        public static Tuple<Guid, string> GetEmployeeFullName(string userLogin)
        {
            try
            {
                var t = from employee in BaseModel.Employees
                    where employee.AccountName.Equals(userLogin)
                    select new {employee.Id, employee.Name};
                return t.Any()
                    ? new Tuple<Guid, string>(t.First().Id, t.First().Name)
                    : new Tuple<Guid, string>(Guid.Empty, string.Empty);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(
                    "Ошибка при получении полного имени авторизующегося пользователя. GetEmployeeFullName.", ex);
            }

            return new Tuple<Guid, string>(Guid.Empty, null);
        }

        /// <summary>
        /// Получение сотрудника по его ИД
        /// </summary>
        /// <param name="employeeGuid"></param>
        /// <returns></returns>
        public static Employee GetEmployee(Guid employeeGuid)
        {
            try
            {
                return (from employee in BaseModel.Employees
                    where employee.Id.Equals(employeeGuid)
                    select employee).FirstOrDefault(); 
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Ошибка при получении сотрудника по его ИД. GetEmployee.", ex);
            }
            return null;
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
                return new PositionState {Title = "-"};
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Ошибка при получении профессии сотрудника. GetEmployeePositionState.", ex);
            }

            return new PositionState {Title = "-"};
        }

        public static List<SpecificStaffPlace> GetEmployeeSpecificStaffPlaces(Guid employeeGuid)
        {
            try
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
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Ошибка. GetEmployeeSpecificStaffPlaces.", ex);
            }

            return new List<SpecificStaffPlace>();
        }

        public static List<EmployeePlace> GetEmployeePlaces(Guid employeeGuid)
        {
            try
            {
                IQueryable<EmployeePlace> eP = (from employee in BaseModel.Employees
                    join ep in BaseModel.EmployeePlaces on employee.Id equals ep.EmployeeId into employeePlase
                    from empPlace in employeePlase.DefaultIfEmpty()
                    where employee.Id == employeeGuid
                    select empPlace
                    );                
                return eP.Where(i => i != null).ToList();
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Ошибка. GetEmployeePlaces.", ex);
            }
            return new List<EmployeePlace>();
        }


        /// <summary>
        /// Получение количества человек в избранном для юзера
        /// </summary>
        /// <param name="employeeGuid"></param>
        /// <returns></returns>
        public static int GetFavoritesCount(Guid employeeGuid)
        {
            try
            {
                return
                    (from fe in BaseModel.Favorites
                        join emp in BaseModel.Employees on fe.LinkedEmployeeId equals emp.Id into employees
                        from e in employees.DefaultIfEmpty()
                        where fe.EmployeeId == employeeGuid
                        orderby e.Position
                        select e.Id).Count();
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Ошибка. GetFavoritesCount.", ex);
            }
            return 0;
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
            try
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
            catch (Exception ex)
            {
                LogHelper.WriteLog("Ошибка. GetFavorites.", ex);
            }
            return new List<EmployeeModel>();
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
            catch (Exception ex)
            {
                LogHelper.WriteLog("Ошибка. CheckAndOrderFavorites.", ex);
            }
        }

        /// <summary>
        /// Поиск максимальной позиции в избранных для указанного пользователя
        /// </summary>
        /// <param name="employeeGuid"></param>
        /// <returns></returns>
        public static int GetFavoritesMaxPosition(Guid employeeGuid)
        {
            try
            {
                return
                    (from fe in BaseModel.Favorites
                        join emp in BaseModel.Employees on fe.LinkedEmployeeId equals emp.Id into employees
                        from e in employees.DefaultIfEmpty()
                        where fe.EmployeeId == employeeGuid
                        select e.Position).Max();
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Ошибка. GetFavoritesMaxPosition.", ex);
                throw;
            }            
        }

        private static Employee GetFavoriteOnNextPosition(Guid employeeGuid, int position)
        {
            try
            {
                var guids =
                    (from fe in BaseModel.Favorites
                        join emp in BaseModel.Employees on fe.LinkedEmployeeId equals emp.Id into employees
                        from e in employees.DefaultIfEmpty()
                        where fe.EmployeeId == employeeGuid && e.Position > position
                        orderby e.Position
                        select e.Id);
                if (guids.Any())
                    return GetEmployee(guids.First());
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Ошибка. GetFavoriteOnNextPosition.", ex);
            }

            return null;
        }

        private static Employee GetFavoriteOnPreviousPosition(Guid employeeGuid, int position)
        {
            try
            {
                var guids =
                    (from fe in BaseModel.Favorites
                        join emp in BaseModel.Employees on fe.LinkedEmployeeId equals emp.Id into employees
                        from e in employees.DefaultIfEmpty()
                        where fe.EmployeeId == employeeGuid && e.Position < position
                        orderby e.Position
                        select e.Id).ToList();
                if (guids.Any())
                    return GetEmployee(guids.Last());
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Ошибка. GetFavoriteOnPreviousPosition.", ex);
            }

            return null;
        }

        private static List<Employee> GetPreviousFavorites(Guid employeeGuid, int position)
        {
            try
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
            catch (Exception ex)
            {
                LogHelper.WriteLog("Ошибка. GetPreviousFavorites.", ex);
            }
            return new List<Employee>();
        }

        private static List<Employee> GetNextFavorites(Guid employeeGuid, int position)
        {
            try
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
            catch (Exception ex)
            {
                LogHelper.WriteLog("Ошибка. GetNextFavorites.", ex);
                throw;
            }
            return new List<Employee>();
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
            catch (Exception ex)
            {
                LogHelper.WriteLog("Ошибка. UpdateFavoritePosition.", ex);
            }
        }

        public static bool AddToFavorites(Guid idCurrentUser, Guid idFavoriteUser)
        {
            try
            {
                var maxPosition = GetFavoritesMaxPosition(idCurrentUser);

                //add favorites
                var favoriteEmp = new Favorite
                {
                    EmployeeId = idCurrentUser,
                    LinkedEmployeeId = idFavoriteUser,
                    Id = Guid.NewGuid()
                };
                BaseModel.AddToFavorites(favoriteEmp);
                
                //set position
                var favoritePerson = BaseModel.Employees.First(e => e.Id == idFavoriteUser);
                favoritePerson.Position = maxPosition + 1;

                BaseModel.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Ошибка. AddToFavorites.", ex);
            }
            return false;
        }

        public static bool RemoveFromFavorites(Guid idCurrentUset, Guid idFavoriteUser)
        {
            try
            {
                Favorite favoriteEmp =
                    BaseModel.Favorites.First(
                        j => j.EmployeeId == idCurrentUset && j.LinkedEmployeeId == idFavoriteUser);

                BaseModel.Favorites.DeleteObject(favoriteEmp);
//                BaseModel.Favorites.Remove(favoriteEmp);
                BaseModel.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Ошибка. RemoveFromFavorites.", ex);
            }
            return false;
        }

        public static List<EmployeeModel> Search(string employeeName, int pageSize, int startIndex)
        {
            try
            {
                if (string.IsNullOrEmpty(employeeName))
                    employeeName = string.Empty;

                List<Employee> employees = (from employee in BaseModel.Employees
                    join staff in BaseModel.Staffs on employee.Id equals staff.Id into employeeStaff
                    from m in employeeStaff.DefaultIfEmpty()
                    where
                        m.ExpirationDate == null && employee.Name.Contains(employeeName)

                    orderby m.Position.Ranking
                    select employee
                    ).OrderBy(j => j.Name).Skip(pageSize*startIndex).Take(pageSize).ToList();
                var t = employees.Select(i => new EmployeeModel(i.Id)).ToList();
                return t;
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Ошибка. Search.", ex);
            }
            if (string.IsNullOrEmpty(employeeName))
                employeeName = "";

            return new List<EmployeeModel>();
        }

        public static bool CheckBirthdaysAtDay(DateTime date)
        {
            try
            {
                return
                    BaseModel.Employees.Any(
                        t =>
                            t.BirthDay.HasValue && t.BirthDay.Value.Day == date.Day &&
                            t.BirthDay.Value.Month == date.Month);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Ошибка. CheckBirthdaysAtDay.", ex);
                throw;
            }

        }

        public static List<EmployeeModel> GetBirthdayPeople(string guid)
        {
            try
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
            catch (Exception ex)
            {
                LogHelper.WriteLog("Ошибка. GetBirthdayPeople.", ex);
            }
            return new List<EmployeeModel>();
        }

        public static bool CheckIsFavorite(Guid currentUser, Guid employeeGuid)
        {
            try
            {
                return
                    (from fe in BaseModel.Favorites
                        join emp in BaseModel.Employees on fe.LinkedEmployeeId equals emp.Id into employees
                        where fe.EmployeeId == currentUser && fe.LinkedEmployeeId == employeeGuid
                        select fe.Id).Any();

            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Ошибка. CheckIsFavorite.", ex);
            }
            return false;
        }

        public static int GetSearchResultCount(string employeeName)
        {
            try
            {
                return (from employee in BaseModel.Employees
                    join staff in BaseModel.Staffs on employee.Id equals staff.Id into employeeStaff
                    from m in employeeStaff.DefaultIfEmpty()
                    where
                        m.ExpirationDate == null && employee.Name.Contains(employeeName)
                    select employee
                    ).Count();
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Ошибка. GetSearchResultCount.", ex);
            }
            return 0;
        }

        public static IQueryable<Employee> QueryAdvancedSearch(Guid? divisionId, Guid? placeId, bool isMemberOfHeadquarter,
            string phoneNumber, Guid? departmentId, string dateStart, string dateEnd, string job, string employeeName, bool ignoreIsMember)
        {
            phoneNumber = string.IsNullOrEmpty(phoneNumber) ? string.Empty : phoneNumber;
            job = string.IsNullOrEmpty(job) ? string.Empty : job;
            employeeName = string.IsNullOrEmpty(employeeName) ? string.Empty : employeeName;


            var query = BaseModel.Employees.Where(e => e.Name.Contains(employeeName));


            DateTime startDate;
            DateTime endDate;


            bool hasStartDate = DateTime.TryParse(dateStart, out startDate);
            bool hasEndDate = DateTime.TryParse(dateEnd, out endDate);



            if (divisionId.HasValue)
            {
                if (departmentId.HasValue)
                {
                    query = (from employee in query
                             join staff in BaseModel.Staffs on employee.Id equals staff.Id into employeeStaff

                             from m in employeeStaff.DefaultIfEmpty()
                             join departmentState in BaseModel.DepartmentStates on m.DepartmentId equals
                                 departmentState.Id into
                                 depState

                             from ds in depState.DefaultIfEmpty()
                             join divisionState in BaseModel.DivisionStates on ds.DivisionId equals divisionState.Id into
                                 divState

                             from divS in divState.DefaultIfEmpty()

                             where divS.Id == divisionId && divS.ExpirationDate == null && m.ExpirationDate == null &&
                                 ds.ExpirationDate == null && ds.Id == departmentId

                             select employee);

                    Guid jobGuid;
                    if (Guid.TryParse(job, out jobGuid))
                    {
                        query = (from employee in query
                                 join staff in BaseModel.Staffs on employee.Id equals staff.Id into employeeStaff

                                 from m in employeeStaff.DefaultIfEmpty()

                                 where
                                     m.ExpirationDate == null &&
                                     m.Position.PositionStates.Any(
                                         pos => pos.ExpirationDate == null && pos.Id == jobGuid)

                                 select employee);
                    }
                }
                else
                {
                    //сотрудники из дивизиона (корневые департаменты)
                    query = (from employee in query
                             join staff in BaseModel.Staffs on employee.Id equals staff.Id into employeeStaff

                             from m in employeeStaff.DefaultIfEmpty()
                             join departmentState in BaseModel.DepartmentStates on m.DepartmentId equals
                                 departmentState.Id into
                                 depState

                             from ds in depState.DefaultIfEmpty()
                             join divisionState in BaseModel.DivisionStates on ds.DivisionId equals divisionState.Id into
                                 divState

                             from divS in divState.DefaultIfEmpty()

                             where
                                 divS.Id == divisionId && divS.ExpirationDate == null && m.ExpirationDate == null &&
                                 ds.ExpirationDate == null

                             select employee);
                }
            }


            if (placeId.HasValue)
            {
                //c местом и номером телефона
                query = (from employee in query
                    join employeePlace in BaseModel.EmployeePlaces on employee.Id equals
                        employeePlace.EmployeeId into ep
                    from empPlace in ep.DefaultIfEmpty()
                    where
                        empPlace.Location != null && empPlace.Location.Locality.Id == placeId.Value &&
                        empPlace.PhoneNumber.Contains(phoneNumber)
                    select employee);
            }
            else
            {
                //номер телефона
                query = (from employee in query
                         join employeePlace in BaseModel.EmployeePlaces on employee.Id equals
                             employeePlace.EmployeeId into ep
                         from empPlace in ep.DefaultIfEmpty()
                         where empPlace.PhoneNumber.Contains(phoneNumber)
                         select employee);
            }

            if (!ignoreIsMember)
                query =
                    query.Where(
                        e =>
                            e.IsMemberOfHeadquarter == null
                                ? !isMemberOfHeadquarter
                                : e.IsMemberOfHeadquarter.Value == isMemberOfHeadquarter);


            if (!hasStartDate)
            {
                //без начальной даты рождения

                if (hasEndDate)
                {
                    //с конечной датой рождения
                    query =
                        query.Where(
                            e =>
                                e.BirthDay.HasValue && e.BirthDay.Value.Day <= endDate.Day &&
                                e.BirthDay.Value.Month <= endDate.Month);
                }
            }
            else
            {
                if (!hasEndDate)
                {
                    //c начальной датой рождения
                    //без конечной даты рождения
                    query =
                        query.Where(
                            e =>
                                e.BirthDay.HasValue && e.BirthDay.Value.Day >= startDate.Day &&
                                e.BirthDay.Value.Month >= startDate.Month);

                }
                else
                {
                    query =
                        query.Where(
                            e =>
                                e.BirthDay.HasValue && e.BirthDay.Value.Day >= startDate.Day &&
                                e.BirthDay.Value.Month >= startDate.Month &&
                                e.BirthDay.Value.Day <= endDate.Day &&
                                e.BirthDay.Value.Month <= endDate.Month);
                    //с датами
                }
            }

            return query;
        }

        public static List<EmployeeModel> SearchAdvanced(Guid? divisionId, Guid? placeId, bool isMemberOfHeadquarter,
            string phoneNumber, Guid? departmentId, string dateStart, string dateEnd, string job, string employeeName, int pageSize, bool ignoreIsMember,int startIndex = 0 )
        {
            try
            {
                var query = QueryAdvancedSearch(divisionId, placeId, isMemberOfHeadquarter, phoneNumber, departmentId,
                    dateStart, dateEnd, job, employeeName, ignoreIsMember);

                query = (from employee in query
                    join staff in BaseModel.Staffs on employee.Id equals staff.Id into employeeStaff

                    from m in employeeStaff.DefaultIfEmpty()

                    where
                        m.ExpirationDate == null

                    orderby m.Position.Ranking
                    select employee).Skip(pageSize*startIndex)
                    .Take(pageSize);

                List<Employee> employees = query.ToList();
                var t = employees.Select(i => new EmployeeModel(i.Id)).ToList();
                return t;
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Ошибка. SearchAdvanced.", ex);
            }

            return new List<EmployeeModel>();
        }


        public static int GetAdvancedSearchResultCount(Guid? divisionId, Guid? placeId, bool isMemberOfHeadquarter,
            string phoneNumber, Guid? departmentId, string dateStart, string dateEnd, string job, string employeeName)
        {
            try
            {
                var query = QueryAdvancedSearch(divisionId, placeId, isMemberOfHeadquarter, phoneNumber, departmentId,
                    dateStart, dateEnd, job, employeeName, false);
                return query.Count();
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Ошибка. GetAdvancedSearchResultCount.", ex);
            }
            return 0;
        }

        public static Tuple<Guid, string, bool> GetLoginingUser(string login)
        {
            try
            {
                if (!string.IsNullOrEmpty(login))
                {
                    var user = BaseModel.Employees.FirstOrDefault(t => string.Equals(t.AccountName, login));
                    if (user != null)
                        return new Tuple<Guid, string, bool>(user.Id, user.Name,
                            user.isAdministrator.HasValue && user.isAdministrator.Value);
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Ошибка. GetLoginingUser.", ex);
            }


            return new Tuple<Guid, string, bool>(Guid.Empty, "Неопознанный пользователь", false);
        }


        /// <summary>
        /// Поиск регионов по стране
        /// </summary>
        /// <param name="country"></param>
        /// <returns></returns>
        public static List<string> GetRegions(string country)
        {
            try
            {
                if (string.IsNullOrEmpty(country))
                    //возвращаем всех
                    return
                        BaseModel.Localities.DistinctBy(t => t.Region)
                            .OrderBy(t => t.Region)
                            .Select(t => t.Region)
                            .ToList();

                return BaseModel.Localities.Where(t => string.Equals(t.Country, country))
                    .DistinctBy(t => t.Region)
                    .OrderBy(t => t.Region)
                    .Select(t => t.Region)
                    .ToList();
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Ошибка. GetRegions.", ex);
            }

            return new List<string>();
        }

        /// <summary>
        /// Поиск населенных пункто по стране и региону
        /// </summary>
        /// <param name="country"></param>
        /// <param name="region"></param>
        /// <returns></returns>
        public static List<KeyValuePair<Guid, string>> GetLocalities(string country, string region)
        {
            try
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
            catch (Exception ex)
            {
                LogHelper.WriteLog("Ошибка. GetLocalities.", ex);
            }

            return new List<KeyValuePair<Guid, string>>();
        }


        /// <summary>
        /// Изменение пункта "Член штаба"
        /// </summary>
        /// <param name="employee">Гуид сотрудника</param>
        /// <param name="isMember">Является ли членом штаба</param>
        public static void UpdateMemberOfHeadquarter(Guid employee, bool isMember )
        {
            try
            {
                var user = GetEmployee(employee);
                if (user != null)
                {
                    user.IsMemberOfHeadquarter = isMember;
                    BaseModel.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Ошибка. UpdateMemberOfHeadquarter.", ex);
            }

        }

        /// <summary>
        /// Изменение местоположения сотрудника
        /// </summary>
        /// <param name="employee">Гуид сотрудника</param>
        /// <param name="city">гуид населенного пункта</param>
        /// <param name="street">Улица</param>
        /// <param name="edifice">Здание</param>
        /// <param name="office">Номер офиса</param>
        public static void UpdateEmployeePlaces(Guid employee, Guid city, string street, string edifice, string office)
        {
            try
            {
                var places = GetEmployeePlaces(employee);

                var locality = BaseModel.Localities.FirstOrDefault(t => t.Id.Equals(city));
                var placeWithLocation = places.FirstOrDefault(t => t.Location != null);

                if (locality != null && placeWithLocation != null)
                {
                    placeWithLocation.Location.Street = street;
                    placeWithLocation.Location.Edifice = edifice;
                    placeWithLocation.Location.LocalityId = city;

                    foreach (EmployeePlace place in places)
                    {
                        place.Office = office;
                        place.LocationId = placeWithLocation.Location.Id;
                    }

                    BaseModel.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Ошибка. UpdateEmployeePlaces.", ex);
            }

        }

        /// <summary>
        /// Сохранение изменений номера телефона. Создание нового номера, если не существует записи с ID = place
        /// </summary>
        /// <param name="employee">Сотрудник</param>
        /// <param name="place">Запись о телефоне</param>
        /// <param name="phoneType">гуид телефона (применяется тока при создании нового)</param>
        /// <param name="phone">Номер телефона</param>
        public static void UpdateEmployeePhone(Guid employee, Guid? place, Guid? phoneType, string phone)
        {
            try
            {
                var places = GetEmployeePlaces(employee);

                if (place != null)
                {
                    //update
                    var employeePlace = places.FirstOrDefault(t => t.Id == place);

                    if (employeePlace != null)
                    {
                        employeePlace.PhoneNumber = phone;
                        BaseModel.SaveChanges();
                    }
                }
                else
                {
                    //create

                    var placeWithLocation = places.FirstOrDefault(t => t.Location != null);

                    if (placeWithLocation != null && phoneType.HasValue)
                    {
                        var entity = new EmployeePlace
                        {
                            EmployeeId = employee,
                            LocationId = placeWithLocation.LocationId,
                            PhoneTypeId = phoneType.Value,
                            PhoneNumber = phone,
                            Office = placeWithLocation.Office,
                            Id = Guid.NewGuid()
                        };

                        BaseModel.AddToEmployeePlaces(entity);
                        BaseModel.SaveChanges();
                    }

                    
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Ошибка. UpdateEmployeePhone.", ex);
            }
        }

        public static List<PhoneType> GetPhoneTypes()
        {
            try
            {
                return BaseModel.PhoneTypes.OrderBy(t => t.Type).ToList();
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Ошибка. GetPhoneTypes.", ex);
            }

            return new List<PhoneType>();
        }

        public static string GetEmployeePhoto(Guid id)
        {
            try
            {
                byte[] data = BaseModel.Employees.First(t => t.Id == id).Photo;
                if (data == null)
                {
                    var bmp = Resources.dafaultpic;


                    using (var stream = new MemoryStream())
                    {
                        bmp.Save(stream, ImageFormat.Bmp);

                        data = stream.ToArray();
                    }
                }
                
                return Convert.ToBase64String(data);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Ошибка. GetEmployeePhoto.", ex);
            }

            return null;
        }
    }
}