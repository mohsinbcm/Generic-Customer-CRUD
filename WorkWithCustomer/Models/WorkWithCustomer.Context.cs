﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WorkWithCustomer.Models
{
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;

    public class WorkWithCustomerEntities : DbContext
    {
        public WorkWithCustomerEntities()
            : base("name=WorkWithCustomerEntities")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }

        public DbSet<Cusf> Cusf { get; set; }
        public DbSet<CustomerGroups> CustomerGroups { get; set; }
        public DbSet<Purchases> Purchases { get; set; }
        public DbSet<Distributors> Distributors { get; set; }
        public DbSet<Salespersons> Salesreps { get; set; }
        
    }
}
