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
        private TreeViewModel _treeViewModel;
        private HttpSessionStateBase _session;
        public TreeViewModel TreeViewModel
        {
            get { return _treeViewModel; }
            private set { _treeViewModel = value; }
        }

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
    }
}