//#define RELEASE
#define DEBUG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using KSS.Helpers;

namespace KSS
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        public void Session_Start()
        {
#if RELEASE
            LogHelper.WriteLog("Autorisation. RELEASE mode.");
            LogHelper.WriteLog("Autorisation. UserName= " + Context.User.Identity.Name);

                if (Context.User.Identity.IsAuthenticated)
                {
                    LogHelper.WriteLog("Autorisation. IsAuthenticated= " + Context.User.Identity.IsAuthenticated);
            
                    string userLogin = Context.User.Identity.Name;
                    Tuple<Guid, string, bool> userInfo = DBHelper.GetLoginingUser(userLogin);
                    Guid userDivision = DBHelper.GetEmployeeDivision(userInfo.Item1).Id;
                    Guid userDepartment = DBHelper.GetEmployeeDepartment(userInfo.Item1).Id;
                    if (!string.IsNullOrEmpty(userInfo.Item2))
                    {
                        Session["UserName"] = userInfo.Item2;
                        Session["CurrentUser"] = userInfo.Item1;
                        Session["CurrentUserDepartment"] = userDepartment;
                        Session["CurrentUserDivision"] = userDivision;
                        Session["IsAdministrator"] = userInfo.Item3;
                        Session["BackLink"] = "";
                    }
                    else
                    {
                        Session["UserName"] = "Нераспознанное имя:" + Context.User.Identity.Name;
                        Session["CurrentUser"] = Guid.Empty;
                        Session["CurrentUserDepartment"] = Guid.Empty;
                        Session["CurrentUserDivision"] = Guid.Empty;
                        Session["UserName"] += "Пустой департамент:";
                        Session["IsAdministrator"] = false;
                        Session["BackLink"] = "";
                    }
                }
                else
                {
                    LogHelper.WriteLog("Autorisation. IsAuthenticated= " + Context.User.Identity.IsAuthenticated);
                    Session["CurrentUser"] = Guid.Empty;
                    Session["CurrentUserDepartment"] = Guid.Empty;
                    Session["CurrentUserDivision"] = Guid.Empty;
                    Session["UserName"] = "Неавторизованный пользователь:" + Context.User.Identity.Name;
                    Session["IsAdministrator"] = false;
                    Session["BackLink"] = "";
                }
#else

            LogHelper.WriteLog("Autorisation. Debug mode.");
            Session["CurrentUser"] = "B88F6C02-77F2-41B7-9C66-098A7262EE12";
            Session["CurrentUserDepartment"] =
                DBHelper.GetEmployeeDepartment(new Guid("B88F6C02-77F2-41B7-9C66-098A7262EE12")).Id;
            Session["CurrentUserDivision"] =
                DBHelper.GetEmployeeDivision(new Guid("B88F6C02-77F2-41B7-9C66-098A7262EE12")).Id;
            Session["UserName"] = "Неопознанный пользователь";
            Session["IsAdministrator"] = true;
            Session["BackLink"] = "";
#endif
        }
    }
}