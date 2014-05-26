using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KSS.Helpers;

namespace KSS.Models
{
    public class FavoritesModel
    {
        private HttpSessionStateBase _session;
        public FavoritesModel(HttpSessionStateBase session)
        {
            _session = session;
        }

        public IEnumerable<EmployeeModel> GetFavorites()
        {
            return DBHelper.GetFavorites(new Guid(_session["CurrentUser"].ToString()));
        }

        public bool AddToFavorites(Guid id)
        {
            return DBHelper.AddToFavorites(new Guid(_session["CurrentUser"].ToString()), id);
        }

        public bool RemoveFromFavorites(Guid id)
        {
            return DBHelper.RemoveFromFavorites(new Guid(_session["CurrentUser"].ToString()),id);
        }
    }
}