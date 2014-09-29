using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using KSS.Models;
using KSS.Properties;
using KSS.Server.Entities;
using Microsoft.Ajax.Utilities;

namespace KSS.Helpers
{
    // ReSharper disable once InconsistentNaming
    public class DBHelper
    {
        private static object _lockObject = new object();

        private static CompanyBaseModel _baseModel;
        public static CompanyBaseModel BaseModel
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
                return BaseModel.DepartmentStates.FirstOrDefault(t => t.Id == id && t.ExpirationDate == null);
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
        /// Получение спец.департамента по его ИД
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static DepartmentSpecificState GetDepartmentSpecificState(Guid id)
        {
            try
            {
                return BaseModel.DepartmentSpecificStates.FirstOrDefault(t => t.Id == id);
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
        /// Получение списка конечных дивизионов
        /// </summary>
        /// <returns></returns>
        public static List<DivisionState> GetLastDivisionStates()
        {
            try
            {
                var items = BaseModel.DivisionStates.Where(t => t.ParentId != null && t.ExpirationDate == null &&
                                                                (!BaseModel.DivisionStates.Any(d => d.ParentId == t.Id))).OrderBy(t=>t.Division).ToList();

                return items;
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Ошибка при получении списка конечных дивизионов. GetLastDivisionStates.", ex);
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
                return BaseModel.DepartmentStates.Where(t => t.ExpirationDate == null).OrderBy(t => t.Department).ToList();
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

                return list.Select(t => new PositionState { Id = t.Id, Title = t.Title }).ToList();
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
                return new DepartmentState { Department = "-" };
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(
                    "Ошибка при получении департамента, в котором работает указанный сотрудник. GetEmployeeDepartment.", ex);
            }
            return new DepartmentState { Department = "-" };
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
                return new DivisionState { Division = "-", Id = Guid.Empty };
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(
                    "Ошибка при получении дивизиона, в котором работает указанный сотрудник. GetEmployeeDivision.", ex);

            }
            return new DivisionState { Division = "Не удалось получить данные!", Id = Guid.Empty };
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
                        select new { employee.Id, employee.Name };
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
                                        where employee.Id == employeeGuid && m.ExpirationDate == null && ps.ExpirationDate == null
                                        select ps
                    ).FirstOrDefault();
                if (pState != null)
                    return pState;
                return new PositionState { Title = "-" };
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Ошибка при получении профессии сотрудника. GetEmployeePositionState.", ex);
            }

            return new PositionState { Title = "-" };
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
                var eP = BaseModel.EmployeePlaces.Where(t => t.EmployeeId == employeeGuid).ToList();
                return eP;
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
        /// <param name="currentUser"></param>
        /// <param name="pageSize"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public static List<EmployeeModel> GetFavorites(Guid currentUser, int pageSize, int startIndex)
        {
            try
            {
                List<Guid> favorites =
                    (from fe in BaseModel.Favorites
                     join emp in BaseModel.Employees on fe.LinkedEmployeeId equals emp.Id into employees
                     from e in employees.DefaultIfEmpty()
                     where fe.EmployeeId == currentUser
                     orderby e.Position
                     select e.Id).Skip(pageSize * startIndex).Take(pageSize).ToList();

                return favorites.Select(i => new EmployeeModel(i, currentUser)).ToList();
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
        /// <param name="employeeGuid"></param>
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
                lock (_lockObject)
                {
                    if (hasChanges)
                        BaseModel.SaveChanges();
                }
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
                lock (_lockObject)
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
                lock (_lockObject)
                {
                    Favorite favoriteEmp =
                        BaseModel.Favorites.First(
                            j => j.EmployeeId == idCurrentUset && j.LinkedEmployeeId == idFavoriteUser);

                    BaseModel.Favorites.DeleteObject(favoriteEmp);
                    BaseModel.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Ошибка. RemoveFromFavorites.", ex);
            }
            return false;
        }

        public static List<EmployeeModel> Search(Guid currentUser, string employeeName, int pageSize, int startIndex)
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
                    ).OrderBy(j => j.Name).Skip(pageSize * startIndex).Take(pageSize).ToList();
                var t = employees.Select(i => new EmployeeModel(i.Id, currentUser)).ToList();
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

        public static bool CheckBirthdaysAtDay(DateTime date, string guid)
        {
            try
            {
                var division = new Guid(guid);
                var result = (from employee in BaseModel.Employees
                              join staff in BaseModel.Staffs on employee.Id equals staff.Id into employeeStaff

                              where employee.BirthDay.HasValue && employee.BirthDay.Value.Day == date.Day && employee.BirthDay.Value.Month == date.Month


                              from m in employeeStaff.DefaultIfEmpty()
                              where m.ExpirationDate == null

                              join departmentState in BaseModel.DepartmentStates on m.DepartmentId equals departmentState.Id into
                                  depState

                              from ds in depState.DefaultIfEmpty()
                              where ds.DivisionId == division && ds.ExpirationDate == null
                              select employee.Id).Any();
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Ошибка. CheckBirthdaysAtDay.", ex);
            }
            return false;
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
                              where m.ExpirationDate == null

                              join departmentState in BaseModel.DepartmentStates on m.DepartmentId equals departmentState.Id into
                                  depState

                              from ds in depState.DefaultIfEmpty()
                              where ds.DivisionId == division && ds.ExpirationDate == null
                              select employee.Id)
                    .ToList();

                return
                    result.Distinct().Select(t => new EmployeeModel(t, true, false, true, false, false))
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

        public static IQueryable<Employee> QueryAdvancedSearch(Guid? divisionId, Guid? placeId, bool? isMemberOfHeadquarter,
            string phoneNumber, Guid? departmentId, string dateStart, string dateEnd, string job, string employeeName)
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
                if (string.IsNullOrEmpty(phoneNumber))
                    query = (from employee in query
                        join employeePlace in BaseModel.EmployeePlaces on employee.Id equals
                            employeePlace.EmployeeId into ep
                        from empPlace in ep.DefaultIfEmpty()
                        where empPlace.Location != null && empPlace.Location.Locality.Id == placeId.Value
                        select employee);
                else
                    query = (from employee in query
                        join employeePlace in BaseModel.EmployeePlaces on employee.Id equals
                            employeePlace.EmployeeId into ep
                        from empPlace in ep.DefaultIfEmpty()
                        where empPlace.Location != null && empPlace.Location.Locality.Id == placeId.Value &&
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
//                         where empPlace.PhoneNumber.Contains(phoneNumber) || !ep.Any()
                         select employee);
            }

            if (isMemberOfHeadquarter.HasValue)
                query =
                    query.Where(
                        e =>
                            e.IsMemberOfHeadquarter == null
                                ? !isMemberOfHeadquarter.Value
                                : e.IsMemberOfHeadquarter.Value == isMemberOfHeadquarter.Value);


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

        public static List<EmployeeModel> SearchAdvanced(Guid? divisionId, Guid? placeId, bool? isMemberOfHeadquarter,
            string phoneNumber, Guid? departmentId, string dateStart, string dateEnd, string job, string employeeName, int pageSize, Guid currentUser, int startIndex = 0)
        {
            try
            {
                if (divisionId == Guid.Empty)
                    divisionId = null;
                if (placeId == Guid.Empty)
                    placeId = null;
                if (departmentId == Guid.Empty)
                    departmentId = null;

                var query = QueryAdvancedSearch(divisionId, placeId, isMemberOfHeadquarter, phoneNumber, departmentId,
                    dateStart, dateEnd, job, employeeName);

                List<Employee> employees = (from employee in query
                                            join staff in BaseModel.Staffs on employee.Id equals staff.Id into employeeStaff

                                            from m in employeeStaff.DefaultIfEmpty()

                                            where
                                                m.ExpirationDate == null

                                            orderby m.Position.Ranking
                                            select employee).DistinctBy(i => i.Id).Skip(pageSize * startIndex)
                    .Take(pageSize).ToList();

                var t = employees.Select(i => new EmployeeModel(i.Id, currentUser)).ToList();
                return t;
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Ошибка. SearchAdvanced.", ex);
            }

            return new List<EmployeeModel>();
        }


        public static int GetAdvancedSearchResultCount(Guid? divisionId, Guid? placeId, bool? isMemberOfHeadquarter,
            string phoneNumber, Guid? departmentId, string dateStart, string dateEnd, string job, string employeeName)
        {
            try
            {
                if (divisionId == Guid.Empty)
                    divisionId = null;
                if (placeId == Guid.Empty)
                    placeId = null;
                if (departmentId == Guid.Empty)
                    departmentId = null;

                var query = QueryAdvancedSearch(divisionId, placeId, isMemberOfHeadquarter, phoneNumber, departmentId,
                    dateStart, dateEnd, job, employeeName);
                return query.DistinctBy(t => t.Id).Count();
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
        /// Поиск регионов по дивизиону
        /// </summary>
        /// <param name="divisionId"></param>
        /// <returns></returns>
        public static List<string> GetRegions(Guid? divisionId)
        {
            try
            {
                if (divisionId.HasValue && divisionId.Value != Guid.Empty)
                {
                    return BaseModel.Locations
                        .DistinctBy(t => t.LocalityId)
                        .Select(t => t.Locality.Region)
                        .Distinct()
                        .OrderBy(t => t).ToList();

                    //                    return BaseModel.Locations.Where(t => t.DivisionId == divisionId)
                    //                        .DistinctBy(t => t.LocalityId)
                    //                        .Select(t => t.Locality.Region)
                    //                        .Distinct()
                    //                        .OrderBy(t => t).ToList();
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Ошибка. GetRegions.", ex);
            }

            return new List<string>();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="division"></param>
        /// <param name="territory"></param>
        /// <returns></returns>
        public static List<string> GetRegions(Guid division, Guid? territory)
        {
            try
            {
                var query = BaseModel.Locations.Where(t => t.DivisionId == division);


                query = territory.HasValue
                    ? query.Where(t => t.TerritoryId == territory)
                    : query.Where(t => t.TerritoryId == null);

                return query.Select(t => t.Locality.Region).DistinctBy(t => t).OrderBy(t => t).ToList();

            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Ошибка. GetRegions(Guid division, Guid? territory).", ex);
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
        public static void UpdateMemberOfHeadquarter(Guid employee, bool isMember)
        {
            try
            {
                lock (_lockObject)
                {
                    var user = GetEmployee(employee);
                    if (user != null)
                    {
                        user.IsMemberOfHeadquarter = isMember;
                        BaseModel.SaveChanges();
                    }
                }

            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Ошибка. UpdateMemberOfHeadquarter.", ex);
            }

        }

        public static void UpdateEmployeePlaces(Guid employee, Guid city, Guid edifice, Guid? pavillion, string office,
            Guid? territoryGuid)
        {
            try
            {
                lock (_lockObject)
                {
                    LogHelper.WriteLog("UpdateEmployeePlaces. " +
                                       string.Format("{0},{1},{2},{3},{4}", employee, city, edifice, pavillion, office));

                    //Получаем список плейсов
                    var places = GetEmployeePlaces(employee);

                    //ищем локейшен здания или корпуса

                    Location location = null;
                    if (pavillion.HasValue)
                        //по корпусу
                        location = BaseModel.Locations.First(t => t.Id.Equals(pavillion.Value));
                    else
                        //по зданию
                        location = BaseModel.Locations.First(t => t.Id.Equals(edifice));


                    Location resultLocation = null;
                    if (pavillion.HasValue)
                    {
                        //ищем локейшен на основе значений
                        resultLocation =
                            BaseModel.Locations.FirstOrDefault(t => t.DivisionId == location.DivisionId &&
                                                                    t.TerritoryId == territoryGuid &&
                                                                    t.Street == location.Street &&
                                                                    t.Edifice == location.Edifice &&
                                                                    t.Building == location.Building);
                    }
                    else
                    {
                        //ищем локейшен на основе значений
                        resultLocation =
                            BaseModel.Locations.FirstOrDefault(t => t.DivisionId == location.DivisionId &&
                                                                    t.TerritoryId == territoryGuid &&
                                                                    t.Street == location.Street &&
                                                                    t.Edifice == location.Edifice &&
                                                                    string.IsNullOrEmpty(t.Building));
                    }



                    //если такого локейшена нет, то создаем
                    if (resultLocation == null)
                    {

                        LogHelper.WriteLog("UpdateEmployeePlaces. create location");
                        var newLocation = new Location
                        {
                            Id = Guid.NewGuid(),
                            DivisionId = location.DivisionId,
                            LocalityId = location.LocalityId,
                            TerritoryId = territoryGuid,
                            Street = location.Street,
                            Edifice = location.Edifice,
                            Building = null,
                            PhoneZoneId = null
                        };

                        BaseModel.AddToLocations(newLocation);
                        location = newLocation;
                    }
                    else
                        location = resultLocation;


                    ////                    если есть корпус, то берем локейшен по корпусу
                    //                    if (pavillion.HasValue)
                    //                        location = BaseModel.Locations.First(t => t.Id.Equals(pavillion.Value));
                    //                    else
                    //                    {
                    //                        //если нет корпуса, то берем локейшен по зданию
                    //                        var innerLocation = BaseModel.Locations.First(t => t.Id.Equals(edifice));
                    //                        //при этом у найденного локейшена не должно быть корпуса
                    //                        if (string.IsNullOrEmpty(innerLocation.Building))
                    //                            location = innerLocation;
                    //                        else
                    //                        {
                    //                            //если у локейшена для здания все-таки есть корпус, то ищем такой же локейшен, но без копуса
                    //                            location =
                    //                                BaseModel.Locations.FirstOrDefault(t => t.DivisionId == innerLocation.DivisionId &&
                    //                                                                        t.LocalityId == innerLocation.LocalityId &&
                    //                                                                        t.TerritoryId == innerLocation.TerritoryId &&
                    //                                                                        t.Street == innerLocation.Street &&
                    //                                                                        t.Edifice == innerLocation.Edifice &&
                    //                                                                        string.IsNullOrEmpty(t.Building));
                    //                            //если такого локейшена нет, то создаем
                    //                            if (location == null)
                    //                            {
                    //
                    //                                LogHelper.WriteLog("UpdateEmployeePlaces. create location");
                    //                                var newLocation = new Location
                    //                                {
                    //                                    Id = Guid.NewGuid(),
                    //                                    DivisionId = innerLocation.DivisionId,
                    //                                    LocalityId = innerLocation.LocalityId,
                    //                                    TerritoryId = innerLocation.TerritoryId,
                    //                                    Street = innerLocation.Street,
                    //                                    Edifice = innerLocation.Edifice,
                    //                                    Building = null,
                    //                                    PhoneZoneId = null
                    //                                };
                    //
                    //                                BaseModel.AddToLocations(newLocation);
                    //                                location = newLocation;
                    //                            }
                    //                        }
                    //                    }

                    //если список плейсов пуст, то создаем новое. В качестве локейшена - локейшен Икс
                    //проставляем офис, номер телефона = "_"
                    if (!places.Any())
                    {
                        //create empty place
                        LogHelper.WriteLog("UpdateEmployeePlaces. create empty place");
                        var newPlace = new EmployeePlace
                        {
                            Id = Guid.NewGuid(),
                            EmployeeId = employee,
                            LocationId = location.Id,
                            PhoneTypeId = BaseModel.PhoneTypes.First().Id,
                            PhoneNumber = "_",
                            Office = office
                        };

                        BaseModel.AddToEmployeePlaces(newPlace);
                    }
                    else
                    {
                        //если список не пуст, то устанавливаем всем плейсам локейшен икс
                        //проставляем всем офис

                        LogHelper.WriteLog("UpdateEmployeePlaces. update  places");
                        foreach (EmployeePlace place in places)
                        {
                            place.Office = office;
                            place.LocationId = location.Id;
                        }
                    }
                }

                BaseModel.SaveChanges();
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Ошибка. UpdateEmployeePlaces.", ex);
            }
        }


        /// <summary>
        /// Изменение местоположения сотрудника
        /// </summary>
        /// <param name="employee">Гуид сотрудника</param>
        /// <param name="city">гуид населенного пункта</param>
        /// <param name="edifice">Здание</param>
        /// <param name="pavillion">номер корпуса</param>
        /// <param name="office">Номер офиса</param>
        /// <param name="territoryGuid"></param>
        public static void UpdateEmployeePlaces2(Guid employee, Guid city, Guid edifice, string pavillion, string office, Guid? territoryGuid)
        {
            try
            {
                lock (_lockObject)
                {


                    LogHelper.WriteLog("UpdateEmployeePlaces. " +
                   string.Format("{0},{1},{2},{3},{4}", employee, city, edifice, pavillion, office));

                    var places = GetEmployeePlaces(employee);

                    //                    var locality = BaseModel.Localities.First(t => t.Id.Equals(city));
                    var placeWithLocation = places.FirstOrDefault(t => t.Location != null);

                    var location = BaseModel.Locations.First(t => t.Id.Equals(edifice));

                    pavillion = string.IsNullOrEmpty(pavillion) ? "" : pavillion;

                    if (placeWithLocation != null)
                    {
                        LogHelper.WriteLog("UpdateEmployeePlaces. Update existed places");
                        placeWithLocation.Location.Street = location.Street;
                        placeWithLocation.Location.Edifice = location.Edifice;
                        placeWithLocation.Location.LocalityId = city;
                        placeWithLocation.Location.Building = pavillion;
                        placeWithLocation.Location.TerritoryId = territoryGuid;

                        foreach (EmployeePlace place in places)
                        {
                            place.Office = office;
                            place.LocationId = placeWithLocation.Location.Id;
                        }
                    }
                    else
                    {
                        //create location

//                        LogHelper.WriteLog("UpdateEmployeePlaces. create location places");
//                        var newLocation = new Location
//                        {
//                            Id = Guid.NewGuid(),
//                            DivisionId = location.DivisionId,
//                            LocalityId = city,
//                            Street = location.Street,
//                            Edifice = location.Edifice,
//                            Building = pavillion
//                        };

//                        BaseModel.AddToLocations(newLocation);

                        


                        if (places.Count == 0)
                        {
                            //create empty place
                            LogHelper.WriteLog("UpdateEmployeePlaces. create empty place");
                            var newPlace = new EmployeePlace
                            {
                                Id = Guid.NewGuid(),
                                EmployeeId = employee,
                                LocationId = edifice,
                                PhoneTypeId = BaseModel.PhoneTypes.First().Id,
                                PhoneNumber = "_",
                                Office = office
                            };

                            BaseModel.AddToEmployeePlaces(newPlace);
                        }
                        else
                        {
                            LogHelper.WriteLog("UpdateEmployeePlaces. update  places");
                            foreach (EmployeePlace place in places)
                            {
                                place.Office = office;
                                place.LocationId = edifice;
                            }
                        }


                    }

                    BaseModel.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _baseModel = new CompanyBaseModel();
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
                LogHelper.WriteLog("UpdateEmployeePhone. " +
                                   string.Format("{0},{1},{2},{3}", employee, place, phoneType, phone));
                lock (_lockObject)
                {
                    var places = GetEmployeePlaces(employee);

                    if (place != null)
                    {
                        //update
                        LogHelper.WriteLog("UpdateEmployeePhone.update place");
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
                        LogHelper.WriteLog("UpdateEmployeePhone.create place");
                        var placeWithLocation = places.FirstOrDefault(t => t.LocationId != null);

                        if (phoneType.HasValue)
                        {
                            var entity = new EmployeePlace
                            {
                                EmployeeId = employee,
                                PhoneTypeId = phoneType.Value,
                                PhoneNumber = phone,
                                Id = Guid.NewGuid()
                            };

                            if (placeWithLocation != null)
                            {
                                entity.LocationId = placeWithLocation.LocationId;
                                entity.Office = placeWithLocation.Office;
                            }

                            BaseModel.AddToEmployeePlaces(entity);
                            BaseModel.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _baseModel = new CompanyBaseModel();
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
                byte[] data = null;

                if (id != Guid.Empty)
                {
                    data = BaseModel.Employees.First(t => t.Id == id).Photo;
                }

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

        public static bool UpdateEmployeePhoto(Guid employee, string image)
        {
            try
            {
                lock (_lockObject)
                {
                    var user = BaseModel.Employees.FirstOrDefault(t => t.Id == employee);
                    if (user != null && !string.IsNullOrEmpty(image))
                    {
                        image = image.Substring(image.IndexOf(',') + 1);
                        byte[] imageBytes = Convert.FromBase64String(image);
                        user.Photo = imageBytes;

                        BaseModel.SaveChanges();

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Ошибка. UpdateEmployeePhoto.", ex);
            }

            return false;
        }

        public static void DeletePhone(Guid employPlaceId)
        {
            try
            {
                lock (_lockObject)
                {
                    var place = BaseModel.EmployeePlaces.FirstOrDefault(t => t.Id == employPlaceId);
                    if (place != null)
                    {
                        BaseModel.DeleteObject(place);
                        BaseModel.SaveChanges();
                    }
                }

            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Ошибка. DeletePhone.", ex);
            }
        }

        public static SpecificStaff GetSpecificStaff(Guid id)
        {
            try
            {
                return BaseModel.SpecificStaffs.FirstOrDefault(t => t.Id == id);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Ошибка. GetSpecificStaff.", ex);
            }
            return null;
        }

        /// <summary>
        /// Количество SpecificStaff с  DepartmentSpecificId == id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static int GetSpecificStaffsCount(Guid id)
        {
            try
            {
                return BaseModel.SpecificStaffs.Count(t => t.DepartmentSpecificId == id);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Ошибка. GetSpecificStaffsCount.", ex);
            }
            return 0;
        }

        public static List<SpecificStaffModel> GetSpecificStaffs(Guid id, int pageSize, int startIndex)
        {
            try
            {
                var items =
                    BaseModel.SpecificStaffs.Where(t => t.DepartmentSpecificId == id)
                        .OrderBy(t => t.Ranking).Skip(pageSize * startIndex)
                        .Take(pageSize).ToList();
                return items.Select(t => new SpecificStaffModel(t.Id)).ToList();
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Ошибка. GetSpecificStaffs.", ex);
            }
            return new List<SpecificStaffModel>();
        }

        public static List<Employee> GetPersonsInDivision(Guid divisionId)
        {
            try
            {
                var employes = (from employee in BaseModel.Employees
                                join staff in BaseModel.Staffs on employee.Id equals staff.Id into employeeStaff
                                from m in employeeStaff.DefaultIfEmpty()
                                join departmentState in BaseModel.DepartmentStates on m.DepartmentId equals departmentState.Id into
                                    depState
                                from ds in depState.DefaultIfEmpty()
                                join divisionState in BaseModel.DivisionStates on ds.DivisionId equals divisionState.Id into
                                    divState
                                from divS in divState.DefaultIfEmpty()
                                where
                                    m.ExpirationDate == null && ds.ExpirationDate == null && divS.ExpirationDate == null &&
                                    divS.Id == divisionId
                                select employee).DistinctBy(t => t.Id).OrderBy(t => t.Name).ToList();

                return employes;
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Ошибка. GetPersonsInDivision.", ex);
            }
            return new List<Employee>();
        }

        public static void SavePersonForSpecificCard(Guid id, Guid employeeId)
        {
            try
            {
                lock (_lockObject)
                {
                    var specific = BaseModel.SpecificStaffs.FirstOrDefault(t => t.Id == id);
                    if (specific != null)
                    {
                        specific.EmployeeId = employeeId == Guid.Empty ? (Guid?)null : employeeId;

                        BaseModel.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Ошибка. SavePersonForSpecificCard.", ex);
            }
        }

        public static void RemovePersonFromSpecificCard(Guid id)
        {
            try
            {
                lock (_lockObject)
                {
                    var specific = BaseModel.SpecificStaffs.FirstOrDefault(t => t.Id == id);
                    if (specific != null)
                    {
                        specific.EmployeeId = (Guid?) null;

                        BaseModel.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Ошибка. RemovePersonFromSpecificCard.", ex);
            }
        }

        public static void UpdateLocationForSpecificCard(Guid specificStaffId, Guid city, string street, string edifice, string office)
        {
            try
            {
                lock (_lockObject)
                {


                    var specificStaf = BaseModel.SpecificStaffs.FirstOrDefault(t => t.Id == specificStaffId);
                    var places = BaseModel.SpecificStaffPlaces.Where(t => t.SpecificStaffId == specificStaffId).ToList();
                    var locality = BaseModel.Localities.FirstOrDefault(t => t.Id.Equals(city));

                    if (locality != null && specificStaf != null)
                    {
                        var placeWithLocation = places.FirstOrDefault(t => t.Location != null);
                        if (placeWithLocation != null)
                        {
                            placeWithLocation.Location.Street = street;
                            placeWithLocation.Location.Edifice = edifice;
                            placeWithLocation.Location.LocalityId = city;

                            foreach (SpecificStaffPlace place in places)
                            {
                                place.Office = office;
                                place.LocationId = placeWithLocation.Location.Id;
                            }
                            BaseModel.SaveChanges();
                        }
                        else
                        {
                            //create location
                            if (specificStaf.EmployeeId == null)
                            {
                                return;
                            }

                            var personDivision = GetEmployeeDivision(specificStaf.EmployeeId.Value);

                            if (personDivision.Id != Guid.Empty)
                            {
                                var newLocation = new Location
                                {
                                    Id = Guid.NewGuid(),
                                    DivisionId = personDivision.Id,
                                    LocalityId = city,
                                    Street = street,
                                    Edifice = edifice
                                };

                                BaseModel.AddToLocations(newLocation);

                                if (places.Count == 0)
                                {
                                    //create empty place
                                    var newPlace = new SpecificStaffPlace
                                    {
                                        Id = Guid.NewGuid(),
                                        SpecificStaffId = specificStaffId,
                                        LocationId = newLocation.Id,
                                        PhoneTypeId = BaseModel.PhoneTypes.First().Id,
                                        PhoneNumber = "_",
                                        Office = office
                                    };

                                    BaseModel.AddToSpecificStaffPlaces(newPlace);
                                }
                                else
                                    foreach (SpecificStaffPlace place in places)
                                    {
                                        place.Office = office;
                                        place.LocationId = newLocation.Id;
                                    }

                                BaseModel.SaveChanges();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Ошибка. UpdateLocationForSpecificCard.", ex);
            }
        }

        public static void DeleteSpecificPhone(Guid specificStaffPlaceId)
        {
            try
            {
                lock (_lockObject)
                {
                    var place = BaseModel.SpecificStaffPlaces.FirstOrDefault(t => t.Id == specificStaffPlaceId);
                    if (place != null)
                    {
                        BaseModel.DeleteObject(place);
                        BaseModel.SaveChanges();
                    }
                }

            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Ошибка. DeleteSpecificPhone.", ex);
            }
        }

        public static void UpdateSpecificPhone(Guid specificStaffId, Guid? specificStaffPlaceId, Guid? phoneType, string phone)
        {
            try
            {
                lock (_lockObject)
                {
                    var places = BaseModel.SpecificStaffPlaces.Where(t => t.SpecificStaffId == specificStaffId).ToList();

                    if (specificStaffPlaceId.HasValue)
                    {
                        //update
                        var specificPlace = places.FirstOrDefault(t => t.Id == specificStaffPlaceId.Value);

                        if (specificPlace != null)
                        {
                            specificPlace.PhoneNumber = phone;
                            BaseModel.SaveChanges();
                        }
                    }
                    else
                    {
                        //create

                        var placeWithLocation = places.FirstOrDefault(t => t.LocationId != null);

                        if (phoneType.HasValue)
                        {

                            var entity = new SpecificStaffPlace
                            {
                                Id = Guid.NewGuid(),
                                SpecificStaffId = specificStaffId,
                                PhoneTypeId = phoneType.Value,
                                PhoneNumber = phone,
                            };

                            if (placeWithLocation != null)
                            {
                                entity.LocationId = placeWithLocation.LocationId;
                                entity.Office = placeWithLocation.Office;
                            }

                            BaseModel.AddToSpecificStaffPlaces(entity);
                            BaseModel.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Ошибка. UpdateSpecificPhone.", ex);
            }
        }

        /// <summary>
        /// Create new Location/localy
        /// </summary>
        /// <param name="newDivisionName"></param>
        /// <param name="parentDivisionGuid"></param>
        /// <param name="existedDivision"></param>
        /// <param name="newRegion"></param>
        /// <param name="existedRegion"></param>
        /// <param name="existedTerritory"></param>
        /// <param name="parentTerritory"></param>
        /// <param name="city"></param>
        /// <param name="existedCity"></param>
        /// <param name="innerPhoneCode"></param>
        /// <param name="outerPhoneCode"></param>
        /// <param name="street"></param>
        /// <param name="existedStreet"></param>
        /// <param name="house"></param>
        /// <param name="existedHouse"></param>
        /// <param name="pavilion"></param>
        /// <param name="newTerritory"></param>
        public static void CreateNewLocation(string newDivisionName, Guid? parentDivisionGuid,
            Guid? existedDivision, string newRegion, string existedRegion, string newTerritory, Guid? existedTerritory,  Guid? parentTerritory,
            string city, Guid? existedCity, string innerPhoneCode, string outerPhoneCode, string street,
            Guid? existedStreet, string house, Guid? existedHouse, string pavilion)
        {
            try
            {

                lock (_lockObject)
                {

                    LogHelper.WriteLog("CreateNewLocation. " +
                                       string.Format(
                                           "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15}",
                                           newDivisionName, parentDivisionGuid,
                                           existedDivision, newRegion, existedRegion, newTerritory, existedTerritory,
                                           city, existedCity, innerPhoneCode, outerPhoneCode, street,
                                           existedStreet, house, existedHouse, pavilion));



                    //division
                    var divisionId = Guid.Empty;
                    if (!existedDivision.HasValue)
                    {
                        var division = new Division
                        {
                            Id = Guid.NewGuid(),
                            Code = "000000000",
                            Ranking = "000001",
                            Essential = true
                        };

                        BaseModel.AddToDivisions(division);

                        var divisionState = new DivisionState { Id = division.Id };
                        if (parentDivisionGuid.HasValue)
                            divisionState.ParentId = parentDivisionGuid;
                        divisionState.Division = newDivisionName;
                        divisionState.ValidationDate = DateTime.Today;

                        BaseModel.AddToDivisionStates(divisionState);

                        divisionId = division.Id;
                    }
                    else
                        divisionId = existedDivision.Value;

                    //territory
                    Guid territoryId;
                    if (!existedTerritory.HasValue)
                    {
                        var territory = new Territory { Id = Guid.NewGuid() };

                        BaseModel.AddToTerritories(territory);

                        var territoryState = new TerritoryState
                        {
                            Id = territory.Id,
                            DivisionId = divisionId,
                            Territory = newTerritory,
                            ValidationDate = DateTime.Now,
                        };
                        if (parentTerritory.HasValue)
                            territoryState.ParentId = parentTerritory;

                        int phonePrefix;
                        if (!string.IsNullOrEmpty(innerPhoneCode) && Int32.TryParse(innerPhoneCode, out phonePrefix))
                            territoryState.PhonePrefix = phonePrefix;

                        BaseModel.AddToTerritoryStates(territoryState);

                        territoryId = territory.Id;
                    }
                    else
                        territoryId = existedTerritory.Value;

                    //city
                    Guid localityId;
                    if (!existedCity.HasValue)
                    {
                        var locality = new Locality
                        {
                            Id = Guid.NewGuid(),
                            Locality1 = city,
                            Region = string.IsNullOrEmpty(existedRegion) ? newRegion : existedRegion,
                            Country = "Россия",
                            CityPhoneCode = outerPhoneCode
                        };
                        BaseModel.AddToLocalities(locality);

                        localityId = locality.Id;
                    }
                    else
                        localityId = existedCity.Value;

                    //street
                    string streetName = existedStreet.HasValue ? BaseModel.Locations.First(t => t.Id == existedStreet.Value).Street : street;


                    //house
                    string houseNumber = existedHouse.HasValue
                        ? BaseModel.Locations.First(t => t.Id == existedHouse.Value).Edifice
                        : house;


                    LogHelper.WriteLog("CreateNewLocation. Create location");
                    var location = new Location
                    {
                        Id = Guid.NewGuid(),
                        DivisionId = divisionId,
                        LocalityId = localityId,
                        TerritoryId = territoryId,
                        Street = streetName,
                        Edifice = houseNumber,
                        Building = pavilion
                    };

                    BaseModel.AddToLocations(location);

                    BaseModel.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _baseModel = new CompanyBaseModel();
                LogHelper.WriteLog("Ошибка. CreateNewLocation.", ex);
            }
        }

        public static List<TerritoryState> GetTerritories(Guid division)
        {
            try
            {
                return
                    BaseModel.TerritoryStates.Where(t => t.DivisionId == division && t.ExpirationDate == null)
                        .OrderBy(t => t.Territory)
                        .ToList();
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Ошибка. GetTerritories.", ex);
            }
            return new List<TerritoryState>();
        }

        public static List<KeyValuePair<Guid, string>> GetStreets(Guid locality)
        {
            try
            {
                return
                    BaseModel.Locations.Where(t => t.LocalityId == locality).
                    Where(t => !string.IsNullOrEmpty(t.Street))
                        .DistinctBy(t => t.Street).
                        OrderBy(t => t.Street)
                        .Select(t => new KeyValuePair<Guid, string>(t.Id, t.Street))
                        .ToList();
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Ошибка. GetStreets.", ex);
            }
            return new List<KeyValuePair<Guid, string>>();
        }

        public static List<KeyValuePair<Guid, string>> GetHouses(Guid? locality, Guid? street)
        {
            try
            {
                IQueryable<Location> query = BaseModel.Locations;

                if (locality.HasValue)
                    query = query.Where(t => t.LocalityId == locality.Value);

                if (street.HasValue)
                {
                    var streetName = BaseModel.Locations.First(t => t.Id == street.Value);
                    query = query.Where(t => t.Street == streetName.Street);
                }

                return query.Where(t => !string.IsNullOrEmpty(t.Edifice)).DistinctBy(t => t.Edifice)
                    .OrderBy(t => t.Edifice)
                    .Select(t => new KeyValuePair<Guid, string>(t.Id, t.Edifice))
                    .ToList();
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Ошибка. GetHouses.", ex);
            }
            return new List<KeyValuePair<Guid, string>>();
        }


        public static List<string> GetPavillions(Guid? locality, Guid edifice)
        {
            try
            {
                IQueryable<Location> query = BaseModel.Locations;

                if (locality.HasValue)
                    query = query.Where(t => t.LocalityId == locality.Value);

                var location = BaseModel.Locations.First(t => t.Id == edifice);
                query = query.Where(t => t.Street == location.Street && t.Edifice == location.Edifice);

                return
                    query.Where(t => !string.IsNullOrEmpty(t.Building))
                        .Select(t => t.Building)
                        .Distinct()
                        .OrderBy(t => t)
                        .ToList();
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Ошибка. GetPavillions.", ex);
            }
            return new List<string>();
        }

        public static List<KeyValuePair<Guid,string>> GetPavillions2(Guid? locality, Guid edifice)
        {
            try
            {
                IQueryable<Location> query = BaseModel.Locations;

                if (locality.HasValue)
                    query = query.Where(t => t.LocalityId == locality.Value);

                var location = BaseModel.Locations.First(t => t.Id == edifice);
                query = query.Where(t => t.Street == location.Street && t.Edifice == location.Edifice);

                var list =
                    query.Where(t => !string.IsNullOrEmpty(t.Building)).DistinctBy(t=>t.Building).OrderBy(t=>t.Building).ToList();
                return list.Select(t => new KeyValuePair<Guid, string>(t.Id, t.Building)).ToList();
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Ошибка. GetPavillions2.", ex);
            }
            return new List<KeyValuePair<Guid, string>>();
        }

        private static List<string> GetInnerPhoneCodeByTerritoryId(Guid territoryId)
        {
            try
            {
                var result = new List<string>();
                var state = BaseModel.TerritoryStates.FirstOrDefault(t => t.Id == territoryId);
                if (state != null)
                {
                    if (state.PhonePrefix.HasValue)
                        result.Add(state.PhonePrefix.ToString());

                    if (state.ParentId.HasValue)
                    {
                        var code = GetInnerPhoneCodeByTerritoryId(state.ParentId.Value);
                        if (code.Any())
                            result.AddRange(code);
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Ошибка. GetInnerPhoneCodeByTerritoryId. Последний искомый гуид территории: " + territoryId, ex);                
            }
            return new List<string>();
        }

        public static string GetInnerPhoneCode(Guid territoryId)
        {
            try
            {
                var items = GetInnerPhoneCodeByTerritoryId(territoryId);

                if (items.Any())
                {
                    var result = "";
                    for (int i = items.Count - 1; i > -1; i--)
                    {
                        result += items[i];
                        if (i != 0)
                            result += "-";
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Ошибка. GetInnerPhoneCode.", ex);
            }
            return null;
        }

        /// <summary>
        /// Метод парсит строку и удаляет все символы кроме цифр
        /// </summary>
        /// <param name="number">Входная строка</param>
        /// <returns>Строка из символов</returns>
        public static string ParseNumber(string number)
        {
            if (string.IsNullOrEmpty(number))
                return "";

            var result = number.Where(Char.IsDigit);
            string f = result.Aggregate("", (s, c) => s + c);
            return f;
        }


        #region SpecificStaff

        public static Dictionary<Guid, string> GetDepartmentSpecificStates(Guid divisionId)
        {
            try
            {
                var items =
                    BaseModel.DepartmentSpecificStates.Where(t => t.DivisionId == divisionId)
                        .ToDictionary(t => t.Id, t => t.Name);

                return items;
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Ошибка. GetDepartmentSpecificStates.", ex);
            }
            return new Dictionary<Guid, string>();
        }


        public static void CreateNewDepartmentSpecificStaff(Guid division, Guid? parent, string name)
        {
            try
            {
                var item = new DepartmentSpecificState
                {
                    Id = Guid.NewGuid(),
                    DivisionId = division,
                    ParentId = parent,
                    Name = name,
                    ValidationDate = DateTime.Now,
                    ExpirationDate = null
                };

                BaseModel.AddToDepartmentSpecificStates(item);
                BaseModel.SaveChanges();
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Ошибка. CreateNewDepartmentSpecificStaff.", ex);
            }
        }

        public static void UpdateDepartmentSpecificStaff(Guid specificStateId, string name)
        {
            try
            {
                var item = BaseModel.DepartmentSpecificStates.FirstOrDefault(t => t.Id == specificStateId);
                if (item != null)
                {
                    item.Name = name;
                    BaseModel.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Ошибка. UpdateDepartmentSpecificStaff.", ex);
            }
        }

        public static void RemoveDepartmentSpecificStaff(Guid specificStateId)
        {
            try
            {
                var item = BaseModel.DepartmentSpecificStates.FirstOrDefault(t => t.Id == specificStateId);
                if (item != null)
                {
                    BaseModel.DeleteObject(item);
                    BaseModel.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Ошибка. RemoveDepartmentSpecificStaff.", ex);
            }
        }


        public static void CreateNewSpecificStaff(Guid department, Guid? employee,
            Guid departmentSpecificStaff, string position, string ranking)
        {
            try
            {
                var item = new SpecificStaff
                {
                    Id = Guid.NewGuid(),
                    DepartmentId = department,
                    EmployeeId = employee,
                    Position = position,
                    Ranking = ranking,
                    DepartmentSpecificId = departmentSpecificStaff
                };

                BaseModel.AddToSpecificStaffs(item);
                BaseModel.SaveChanges();
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Ошибка. CreateNewSpecificStaff.", ex);
            }            
        }

        public static void DeleteSpecificStaff(Guid id)
        {
            try
            {
                lock (_lockObject)
                {
                    var item = BaseModel.SpecificStaffs.First(t => t.Id == id);

                    var places = BaseModel.SpecificStaffPlaces.Where(t => t.SpecificStaffId == id).ToList();

                    foreach (var place in places)
                    {
                        BaseModel.DeleteObject(place);
                    }

                    BaseModel.DeleteObject(item);
                    BaseModel.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Ошибка. DeleteSpecificStaff.", ex);
            }     
        }
        #endregion
    }
}