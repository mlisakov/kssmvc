﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class CompanyBaseModel : DbContext
    {
        public CompanyBaseModel()
            : base("name=CompanyBaseModel")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Decrease> Decreases { get; set; }
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<DepartmentState> DepartmentStates { get; set; }
        public virtual DbSet<Division> Divisions { get; set; }
        public virtual DbSet<DivisionPhoneCode> DivisionPhoneCodes { get; set; }
        public virtual DbSet<DivisionState> DivisionStates { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<EmployeePlace> EmployeePlaces { get; set; }
        public virtual DbSet<Favorite> Favorites { get; set; }
        public virtual DbSet<HistoryUser> HistoryUsers { get; set; }
        public virtual DbSet<Locality> Localities { get; set; }
        public virtual DbSet<Location> Locations { get; set; }
        public virtual DbSet<Operator> Operators { get; set; }
        public virtual DbSet<Option> Options { get; set; }
        public virtual DbSet<PhoneMatrixInt> PhoneMatrixInts { get; set; }
        public virtual DbSet<PhoneMatrixIntFull> PhoneMatrixIntFulls { get; set; }
        public virtual DbSet<PhoneType> PhoneTypes { get; set; }
        public virtual DbSet<PhoneZoneInt> PhoneZoneInts { get; set; }
        public virtual DbSet<Position> Positions { get; set; }
        public virtual DbSet<PositionState> PositionStates { get; set; }
        public virtual DbSet<SpecificStaff> SpecificStaffs { get; set; }
        public virtual DbSet<SpecificStaffPlace> SpecificStaffPlaces { get; set; }
        public virtual DbSet<Staff> Staffs { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<Territory> Territories { get; set; }
        public virtual DbSet<TerritoryState> TerritoryStates { get; set; }
    }
}
