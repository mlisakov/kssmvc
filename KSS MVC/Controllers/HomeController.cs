using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.qrcode;
using KSS.Helpers;
using KSS.Models;

namespace KSS.Controllers
{
    public class HomeController : Controller
    {
        private const int PageSize = 5;

        public ActionResult Index()
        {
            var homeViewModel = new HomeViewModel(Session);

            ViewBag.UserName = Session["UserName"];
            ViewBag.UserID = Session["CurrentUser"];

            return View(homeViewModel);
        }

        public ActionResult SearchView(Guid id)
        {
            SearchViewModel model = id == Guid.Empty ? new SearchViewModel(Session) : new SearchViewModel(Session, id);

            ViewResult view= View(model);

            view.ViewBag.StartIndex = 0;
            view.ViewBag.IsAdvanced = true;
            view.ViewBag.Search = "";            
            view.ViewBag.SearchPlace = null;
            view.ViewBag.SearchIsMember = false;

            view.ViewBag.SearchPhoneNumber = "";            
            view.ViewBag.SearchDateStart = "";
            view.ViewBag.SearchDateEnd = "";
            view.ViewBag.SearchJob = "";            

            Session["BackLink"] = Url.Action("SearchView", "Home", new { id = id });
            return view;
        }

        public ActionResult SpecificSearchView(Guid id, int startIndex = 0 )
        {
            var model = new SpecificSearchViewModel(Session, id);
            model.StartIndex = startIndex;

            Session["BackLink"] = Url.Action("SpecificSearchView", "Home", new {id = id, startIndex = startIndex});

            ViewResult view = View(model);
            view.ViewBag.StartIndex = startIndex;                        
            return view;
        }

        public ActionResult Favorites(int startIndex)
        {
            var guid = new Guid(Session["CurrentUser"].ToString());


            var favoritesCount = DBHelper.GetFavoritesCount(guid);

            var employees = new List<EmployeeModel>();
            int pagesCount = 0;
            if (favoritesCount > 0)
            {
                DBHelper.CheckAndOrderFavorites(guid);

                employees = DBHelper.GetFavorites(guid, PageSize, startIndex);

                pagesCount = favoritesCount / PageSize;
                if ((favoritesCount % PageSize) != 0)
                    pagesCount++;
            }

            ViewResult view = View("FavoritesPage", employees);
            view.ViewBag.StartIndex = startIndex;
            view.ViewBag.PageCount = pagesCount;


            Session["BackLink"] = Url.Action("Favorites", "Home", new {startIndex = startIndex});
            
            return view;
        }

        public ActionResult FavoritesWithReplace(int startIndex, Guid userGuid, int delta)
        {
            var guid = new Guid(Session["CurrentUser"].ToString());
            DBHelper.UpdateFavoritePosition(guid, userGuid, delta);

            return Favorites(startIndex);
        }

        public bool RemoveFavorite(Guid id)
        {
            HomeViewModel favoritesViewModel = new HomeViewModel(Session);
            return favoritesViewModel.RemoveFromFavorite(id);
        }

        public bool AddFavorite(Guid id)
        {
            HomeViewModel favoritesViewModel = new HomeViewModel(Session);
            return favoritesViewModel.AddToFavorite(id);
        }

        public ActionResult Help()
        {
            ViewResult view = View("Help");
            Session["BackLink"] = Url.Action("Help", "Home");
            return view;
        }

        public ActionResult SearchEmployees(string employeeName, int startIndex = 0)
        {
            var guid = new Guid(Session["CurrentUser"].ToString());

            var employees = DBHelper.Search(guid, employeeName, PageSize, startIndex);
            ViewResult view = View("SearchEmployeeResult", employees);

            var itemsCount = DBHelper.GetSearchResultCount(employeeName);
            var pagesCount = itemsCount / PageSize;

            if ((itemsCount % PageSize) != 0)
                pagesCount++;

            view.ViewBag.StartIndex = startIndex;
            view.ViewBag.PageCount = pagesCount;
            view.ViewBag.IsAdvanced = false;
            view.ViewBag.Search = employeeName;


            var action = Url.Action("SearchEmployees", "Home",
                new {employeeName = employeeName, startIndex = startIndex});


            //создаем вручную ссылку, т.к. иначе мы сохраним кириллицу в виде символов вида %0D
            action = "/Home/SearchEmployees?employeeName=" + employeeName + "&startIndex=" + startIndex;
            Session["BackLink"] = action;
            return view;
        }

        public ActionResult SearchEmployeesAdvanced(Guid? divisionId, Guid? placeId, bool? isMemberOfHeadquarter,
            string phoneNumber, Guid? departmentId, string dateStart, string dateEnd, string job, string employeeName, int startIndex = 0)
        {
            var guid = new Guid(Session["CurrentUser"].ToString());

            phoneNumber = DBHelper.ParseNumber(phoneNumber);

            var employees = DBHelper.SearchAdvanced(divisionId, placeId, isMemberOfHeadquarter, phoneNumber,
                departmentId,
                dateStart, dateEnd, job, employeeName, PageSize, guid, startIndex);

            ViewResult view = View("SearchEmployeeResult", employees);

            var itemsCount = DBHelper.GetAdvancedSearchResultCount(divisionId, placeId, isMemberOfHeadquarter, phoneNumber,
                departmentId,
                dateStart, dateEnd, job, employeeName);
            var pagesCount = itemsCount/PageSize;

            if ((itemsCount % PageSize) != 0)
                pagesCount++;

            view.ViewBag.StartIndex = startIndex;
            view.ViewBag.PageCount = pagesCount;
            view.ViewBag.IsAdvanced = true;
            view.ViewBag.Search = employeeName;
            view.ViewBag.SearchDivision = divisionId;
            view.ViewBag.SearchPlace = placeId;
            view.ViewBag.SearchIsMember = isMemberOfHeadquarter;

            view.ViewBag.SearchPhoneNumber = phoneNumber;
            view.ViewBag.SearchDepartment = departmentId;
            view.ViewBag.SearchDateStart = dateStart;
            view.ViewBag.SearchDateEnd = dateEnd;
            view.ViewBag.SearchJob = job;


            var action = "/Home/SearchEmployeesAdvanced?divisionId=" + divisionId +
                         "&startIndex=" + startIndex +
                         "&placeId=" + placeId +
                         "&isMemberOfHeadquarter=" + isMemberOfHeadquarter +
                         "&phoneNumber=" + phoneNumber +
                         "&departmentId=" + departmentId +
                         "&dateStart=" + dateStart +
                         "&dateEnd=" + dateEnd +
                         "&job=" + job +
                         "&employeeName=" + employeeName +
                         "&startIndex=" + startIndex;

            Session["BackLink"] = action;
            return view;
        }

        public async Task<ActionResult> GetBirthdays()
        {
            List<EmployeeModel> people =
                await Task.Run(() => DBHelper.GetBirthdayPeople(Session["CurrentUserDivision"].ToString()));

            ViewResult view = View("BirthDayResult", people);            
            return view;
        }

        public ActionResult GetDepartments(Guid divisionId)
        {
            return Json(new SelectList(DBHelper.GetDepartmentStatesByDivision(divisionId), "Id", "Department"),
                JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPositions(Guid departmentId)
        {
            return Json(new SelectList(DBHelper.GetPositionStatesByDepartment(departmentId), "Id", "Title"),
                JsonRequestBehavior.AllowGet);
        }

        public ActionResult Export()
        {

            var searchLink = Session["BackLink"].ToString();
            var employees = new List<EmployeeModel>();
            var guid = new Guid(Session["CurrentUser"].ToString());


            if (searchLink.Contains("SearchEmployeesAdvanced"))
            {
                var parameters = searchLink.Substring(searchLink.IndexOf('?') + 1);
                var paramArra = parameters.Split('&');

                Guid? divisionId = null;
                Guid? placeId = null;
                bool? isMemberOfHeadquarter = null;
                string phoneNumber = string.Empty;
                Guid? departmentId = null;
                string dateStart = string.Empty;
                string dateEnd = string.Empty;
                string job = string.Empty;
                string employeeName = string.Empty;
                const int startIndex = 0;

                foreach (var item in paramArra)
                {
                    var split = item.Split('=');
                    if (split.Length == 1)
                        continue;

                    if (item.Contains("divisionId"))
                    {
                        Guid temp;
                        Guid.TryParse(split[1], out temp);
                        divisionId = temp;
                    }
                    else if (item.Contains("placeId"))
                    {
                        Guid temp;
                        Guid.TryParse(split[1], out temp);
                        placeId = temp;
                    }
                    else if (item.Contains("isMemberOfHeadquarter"))
                    {
                        bool t;
                        if (bool.TryParse(split[1], out t))
                            isMemberOfHeadquarter = t;
                    }
                    else if (item.Contains("phoneNumber"))
                    {
                        phoneNumber = split[1];
                    }
                    else if (item.Contains("departmentId"))
                    {
                        Guid temp;
                        Guid.TryParse(split[1], out temp);
                        departmentId = temp;
                    }
                    else if (item.Contains("dateStart"))
                    {
                        dateStart = split[1];
                    }
                    else if (item.Contains("dateEnd"))
                    {
                        dateEnd = split[1];
                    }
                    else if (item.Contains("job"))
                    {
                        job = split[1];
                    }
                    else if (item.Contains("employeeName"))
                    {
                        employeeName = split[1];
                    }
                }


                var itemsCount = DBHelper.GetAdvancedSearchResultCount(divisionId, placeId, isMemberOfHeadquarter,
                    phoneNumber,
                    departmentId,
                    dateStart, dateEnd, job, employeeName);

                itemsCount = itemsCount > 500 ? 500 : itemsCount;

                employees = DBHelper.SearchAdvanced(divisionId, placeId, isMemberOfHeadquarter, phoneNumber,
                    departmentId,
                    dateStart, dateEnd, job, employeeName, itemsCount, guid);
            }
            else
            {
                if (searchLink.Contains("SearchEmployees"))
                {
                    var parameters = searchLink.Substring(searchLink.IndexOf('?') + 1);
                    var paramArra = parameters.Split('&');

                    var employeeName = "";
                    if (parameters.Length > 1)
                    {
                        employeeName = paramArra[0].Split('=')[1];
                    }

                    var itemsCount = DBHelper.GetSearchResultCount(employeeName);

                    itemsCount = itemsCount > 500 ? 500 : itemsCount;
                    employees = DBHelper.Search(guid, employeeName, itemsCount, 0);
                }
            }


            var dataTable = new DataTable("teste");
            dataTable.Columns.Add("ФИО", typeof (string));
            dataTable.Columns.Add("Подразделение", typeof(string));
            dataTable.Columns.Add("Должность", typeof(string));
            dataTable.Columns.Add("Рабочий телефон", typeof(string));
            dataTable.Columns.Add("Специальный телефон", typeof(string));
            dataTable.Columns.Add("Почта", typeof(string));

            foreach (var employee in employees)
            {
                string phone =
                    employee.EmployeePlaces.Where(
                        place => !string.IsNullOrEmpty(place.PhoneNumber) && place.PhoneNumber != "_")
                        .Aggregate("", (current, place) => current + (place.PhoneNumber + ";    "));

                string specificPhone =
                    employee.SpecificStaffPlaces.Where(
                        t => !string.IsNullOrEmpty(t.PhoneNumber) && t.PhoneNumber != "_")
                        .Aggregate("", (current, place) => current + (place.PhoneNumber + ";    "));

                dataTable.Rows.Add(employee.Employee.Name,
                    employee.DepartmentState.Department,
                    employee.PositionState.Title,
                    phone, specificPhone, employee.Employee.EMail);
            }

            if (employees.Count == 0)
            {
                string empt = "<пусто>";
                dataTable.Rows.Add(empt, empt, empt, empt, empt, empt);
            }

            var grid = new GridView();
            grid.DataSource = dataTable;
            grid.DataBind();

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=Report_KCC.xls");
            Response.ContentType = "application/ms-excel";

            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            grid.RenderControl(htw);

            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            return View("Help");

        }

        public ActionResult Print()
        {

            var searchLink = Session["BackLink"].ToString();
            var employees = new List<EmployeeModel>();
            var guid = new Guid(Session["CurrentUser"].ToString());


            if (searchLink.Contains("SearchEmployeesAdvanced"))
            {
                var parameters = searchLink.Substring(searchLink.IndexOf('?') + 1);
                var paramArra = parameters.Split('&');

                Guid? divisionId = null;
                Guid? placeId = null;
                bool? isMemberOfHeadquarter = null;
                string phoneNumber = string.Empty;
                Guid? departmentId = null;
                string dateStart = string.Empty;
                string dateEnd = string.Empty;
                string job = string.Empty;
                string employeeName = string.Empty;
                const int startIndex = 0;

                foreach (var item in paramArra)
                {
                    var split = item.Split('=');
                    if (split.Length == 1)
                        continue;

                    if (item.Contains("divisionId"))
                    {
                        Guid temp;
                        Guid.TryParse(split[1], out temp);
                        divisionId = temp;
                    }
                    else if (item.Contains("placeId"))
                    {
                        Guid temp;
                        Guid.TryParse(split[1], out temp);
                        placeId = temp;
                    }
                    else if (item.Contains("isMemberOfHeadquarter"))
                    {
                        bool t;
                        if (bool.TryParse(split[1], out t))
                            isMemberOfHeadquarter = t;
                    }
                    else if (item.Contains("phoneNumber"))
                    {
                        phoneNumber = split[1];
                    }
                    else if (item.Contains("departmentId"))
                    {
                        Guid temp;
                        Guid.TryParse(split[1], out temp);
                        departmentId = temp;
                    }
                    else if (item.Contains("dateStart"))
                    {
                        dateStart = split[1];
                    }
                    else if (item.Contains("dateEnd"))
                    {
                        dateEnd = split[1];
                    }
                    else if (item.Contains("job"))
                    {
                        job = split[1];
                    }
                    else if (item.Contains("employeeName"))
                    {
                        employeeName = split[1];
                    }
                }


                var itemsCount = DBHelper.GetAdvancedSearchResultCount(divisionId, placeId, isMemberOfHeadquarter,
                    phoneNumber,
                    departmentId,
                    dateStart, dateEnd, job, employeeName);

                itemsCount = itemsCount > 500 ? 500 : itemsCount;

                employees = DBHelper.SearchAdvanced(divisionId, placeId, isMemberOfHeadquarter, phoneNumber,
                    departmentId,
                    dateStart, dateEnd, job, employeeName, itemsCount, guid);
            }
            else
            {
                if (searchLink.Contains("SearchEmployees"))
                {
                    var parameters = searchLink.Substring(searchLink.IndexOf('?') + 1);
                    var paramArra = parameters.Split('&');

                    var employeeName = "";
                    if (parameters.Length > 1)
                    {
                        employeeName = paramArra[0].Split('=')[1];
                        //                    employeeName = Encoding.Default.GetString(Encoding.Default.GetBytes(employeeName));
                    }

                    var itemsCount = DBHelper.GetSearchResultCount(employeeName);

                    itemsCount = itemsCount > 500 ? 500 : itemsCount;
                    employees = DBHelper.Search(guid, employeeName, itemsCount, 0);
                }
            }

            Response.Clear();
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=\"Report.pdf\"");
            Byte[] fileBuffer = CreatePDFDocument(employees).ToArray();
            Response.BinaryWrite(fileBuffer);
            Response.End();

            return View("Help");

        }

        private MemoryStream CreatePDFDocument(List<EmployeeModel> employees)
        {
            var pdfDoc = new Document();
            var str = new MemoryStream();
            PdfWriter.GetInstance(pdfDoc, str);
            pdfDoc.Open();
            var table = new PdfPTable(1) { HorizontalAlignment = 0 };
            string path = Server.MapPath("~/Fonts/Arial.ttf");
            BaseFont baseFont = BaseFont.CreateFont(path, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            var font = new Font(baseFont, 11, Font.NORMAL);            

            foreach (var employee in employees)
            {
                string phone =
                    employee.EmployeePlaces.Where(
                        place => !string.IsNullOrEmpty(place.PhoneNumber) && place.PhoneNumber != "_")
                        .Aggregate("", (current, place) => current + (place.PhoneNumber + ";    "));

                string specificPhone =
                    employee.SpecificStaffPlaces.Where(
                        t => !string.IsNullOrEmpty(t.PhoneNumber) && t.PhoneNumber != "_")
                        .Aggregate("", (current, place) => current + (place.PhoneNumber + ";    "));

                table.AddCell(
                    new PdfPCell(
                        new Phrase("ФИО:" + employee.Employee.Name + Environment.NewLine +
                                   "Подразделение:" + employee.DepartmentState.Department + Environment.NewLine +
                                   "Должность:" + employee.PositionState.Title + Environment.NewLine +
                                   "Личный телефон:" + phone + Environment.NewLine +
                                   "Рабочий телефон:" + specificPhone +
                                   Environment.NewLine +
                                   "Почта:" + employee.Employee.EMail, font)) { HorizontalAlignment = 0 });
            }

            if (employees.Count == 0)
            {
                table.AddCell(
                    new PdfPCell(
                        new Phrase("ФИО: -" + Environment.NewLine +
                                   "Подразделение: - " + Environment.NewLine +
                                   "Должность: -" + Environment.NewLine +
                                   "Личный телефон: -" + Environment.NewLine +
                                   "Рабочий телефон: -" + Environment.NewLine +
                                   "Почта: - ", font)) {HorizontalAlignment = 0});
            }

            pdfDoc.Add(table);
            pdfDoc.Close();
            return str;
        }



        #region SpecificStaff

        public ActionResult SpecificCard(Guid id)
        {
            bool isAdmin = Convert.ToBoolean(Session["IsAdministrator"]);

            var specificModel = new SpecificStaffModel(id);

            var view = View("SpecificCard", specificModel);
            view.ViewBag.IsAdmin = isAdmin;
            view.ViewBag.BackLink = Session["BackLink"];
            return view;
        }

        public ActionResult SavePersonForSpecificCard(Guid id, Guid employeeId)
        {
            DBHelper.SavePersonForSpecificCard(id, employeeId);

            return SpecificCard(id);
        }

        public ActionResult SaveLocationForSpecificCard(Guid specificStaffId, Guid city, string street, string edifice, string office)
        {
            DBHelper.UpdateLocationForSpecificCard(specificStaffId, city, street, edifice, office);

            return SpecificCard(specificStaffId);
        }

        public ActionResult DeleteSpecificPhone(Guid specificStaffId, Guid specificStaffPlaceId)
        {
            DBHelper.DeleteSpecificPhone(specificStaffPlaceId);

            return SpecificCard(specificStaffId);
        }

        public ActionResult SaveSpecificPhone(Guid specificStaffId, Guid? specificStaffPlaceId, Guid? phoneType, string phone)
        {
            DBHelper.UpdateSpecificPhone(specificStaffId, specificStaffPlaceId, phoneType, phone);

            return SpecificCard(specificStaffId);
        }

        public ActionResult SpecificChangableView()
        {
            var model = new SpecificChangableModel(Session);
            ViewResult view = View("SpecificChangableView", model);

            bool isAdmin = Convert.ToBoolean(Session["IsAdministrator"]);
            view.ViewBag.IsAdmin = isAdmin;

            return view;
        }

        public string GetDepartmentSpecificStaff(Guid division)
        {
            var sb = new StringBuilder();
            sb.Append("<option value=\"\" selected>не выбран</option>");
            foreach (var item in DBHelper.GetDepartmentSpecificStates(division))
            {
                sb.Append("<option value=\"");
                sb.Append(item.Key);
                sb.Append("\">");
                sb.Append(item.Value);
                sb.Append("</option>");
            }
            return sb.ToString();
        }

        public void CreateNewDepartmentSpecificStaff(Guid division, Guid? parent, string name)
        {
            DBHelper.CreateNewDepartmentSpecificStaff(division, parent, name);
        }

        #endregion

    }
}
