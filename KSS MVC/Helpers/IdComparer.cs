using System;
using System.Collections.Generic;
using KSS.Server.Entities;

namespace KSS.Helpers
{
    public class IdComparer : IEqualityComparer<Employee>
    {
        public bool Equals(Employee x, Employee y)
        {
            if (x == null)
            {
                return y == null;
            }

            return x.Id == y.Id;
        }

        public int GetHashCode(Employee obj)
        {
            return obj.GetHashCode();
        }
    }
}