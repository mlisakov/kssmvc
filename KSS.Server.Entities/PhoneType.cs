//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace KSS.Server.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class PhoneType
    {
        public PhoneType()
        {
            this.EmployeePlaces = new HashSet<EmployeePlace>();
            this.SpecificStaffPlaces = new HashSet<SpecificStaffPlace>();
        }
    
        public System.Guid Id { get; set; }
        public string Type { get; set; }
        public string Category { get; set; }
        public int Sequence { get; set; }
    
        public virtual ICollection<EmployeePlace> EmployeePlaces { get; set; }
        public virtual ICollection<SpecificStaffPlace> SpecificStaffPlaces { get; set; }
    }
}
