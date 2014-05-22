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

        public void AddToFavorites(Guid id)
        {
            
        }
    }
}