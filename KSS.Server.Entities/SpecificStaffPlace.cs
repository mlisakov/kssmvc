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
    
    public partial class SpecificStaffPlace
    {
        public System.Guid Id { get; set; }
        public System.Guid SpecificStaffId { get; set; }
        public Nullable<System.Guid> LocationId { get; set; }
        public System.Guid PhoneTypeId { get; set; }
        public string PhoneNumber { get; set; }
        public string Office { get; set; }
    
        public virtual Location Location { get; set; }
        public virtual PhoneType PhoneType { get; set; }
        public virtual SpecificStaff SpecificStaff { get; set; }
    }
}
