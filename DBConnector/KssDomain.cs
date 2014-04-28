using System.Collections.Generic;
using KSS.DBConnector.Models;

namespace KSS.DBConnector
{
    public class KssDomain
    {
        private BaseRepository<DivisionState> _divisionRepository;

        public KssDomain()
        {
            _divisionRepository=new BaseRepository<DivisionState>();
        }

        public IList<DivisionState> GetDivisions()
        {
            return _divisionRepository.GetAll();
        }
    }
}
