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
    
    public partial class DivisionState
    {
        public System.Guid Id { get; set; }
        public Nullable<System.Guid> ParentId { get; set; }
        public string Division { get; set; }
        public string DivisionBrief { get; set; }
        public System.DateTime ValidationDate { get; set; }
        public Nullable<System.DateTime> ExpirationDate { get; set; }
    }
}