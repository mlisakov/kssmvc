//#define RELEASE
#define DEBUG
using System;
using System.Collections.Generic;
using System.Linq;
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
                if (Context.User.Identity.IsAuthenticated)
                {
                    string userLogin = Context.User.Identity.Name;
                    Tuple<Guid,string> userInfo = DBHelper.GetUserFullName(userLogin);
                    Guid userDivision = DBHelper.GetUserDivision(userInfo.Item1).Item1;
                    Guid userDepartment = DBHelper.GetUserDepartment(userInfo.Item1);
                    if (!string.IsNullOrEmpty(userInfo.Item2))
                    {
                        Session["UserName"] = userInfo.Item2;
                        Session["CurrentUser"] = userInfo.Item1;
                        Session["CurrentUserDepartment"] = userDepartment;
                        Session["CurrentUserDivision"] = userDivision;
                    }
                    else
                    {
                        Session["UserName"] = "Нераспознанное имя:" + Context.User.Identity.Name;
                        Session["CurrentUser"] = Guid.Empty;
                        Session["CurrentUserDepartment"] = Guid.Empty;
                        Session["CurrentUserDivision"] = Guid.Empty;
                        Session["UserName"] += "Пустой департамент:";
                    }
                }
                else
                {
                    Session["CurrentUser"] = Guid.Empty;
                    Session["CurrentUserDepartment"] = Guid.Empty;
                    Session["CurrentUserDivision"] = Guid.Empty;
                    Session["UserName"] = "Неавторизованный пользователь:" + Context.User.Identity.Name;
                }
#else
            Session["CurrentUser"] = "B88F6C02-77F2-41B7-9C66-098A7262EE12";
            Session["CurrentUserDepartment"] =
                DBHelper.GetUserDepartment(new Guid("B88F6C02-77F2-41B7-9C66-098A7262EE12"));
            Session["CurrentUserDivision"] =
                DBHelper.GetUserDivision(new Guid("B88F6C02-77F2-41B7-9C66-098A7262EE12")).Item1;
            Session["UserName"] = "Петрович Василий Пупкин";
   

#endif
        }
    }
}