using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KSS.Helpers;
using KSS.Server.Entities;

namespace KSS.Models
{
    public class HomeViewModel
    {
        private HttpSessionStateBase _session;

        public TreeViewModel TreeViewModel { get; private set; }

        public HomeViewModel(HttpSessionStateBase session)
        {
            _session = session;
            if (session["Tree"] == null)
                TreeViewModel = new TreeViewModel();
            else
                TreeViewModel = (TreeViewModel)session["Tree"];
        }

        public IEnumerable<Employee> SearchEmployees()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<EmployeeModel> GetFavorites()
        {
            return DBHelper.GetFavorites(new Guid(_session["CurrentUser"].ToString()));
        }

        public bool AddToFavorite(Guid id)
        {
            return DBHelper.AddToFavorites(new Guid(_session["CurrentUser"].ToString()), id);
        }

        public bool RemoveFromFavorite(Guid id)
        {
            return DBHelper.RemoveFromFavorites(new Guid(_session["CurrentUser"].ToString()), id);
        }
    }
}