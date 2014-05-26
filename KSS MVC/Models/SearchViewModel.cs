using System;
using System.Linq;
using KSS.Server.Entities;

namespace KSS.Models
{
    public class SearchViewModel
    {
        private Guid _id;

        private static CompanyBaseModel _baseModel;

        public SearchViewModel(Guid id)
        {
            if (_baseModel == null)
                _baseModel = new CompanyBaseModel();

            _id = id;
        }

        public string GetName()
        {
            var result = "Без наименования";
             var item = _baseModel.DepartmentStates.FirstOrDefault(t => t.Id == _id);
            if (item == null)
            {
                var innerItem = _baseModel.DivisionStates.FirstOrDefault(t => t.Id ==_id);
                if (innerItem != null)
                    result = innerItem.Division;
            }
            else
            {
                result = item.Department;
            }
            return result;
        }
    }
}