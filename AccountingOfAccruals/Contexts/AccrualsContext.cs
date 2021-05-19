using AccountingOfAccruals.Entities;
using System;
using System.Data.Entity;

namespace AccountingOfAccruals.Contexts
{
    public class AccrualsContext : DbContext
    {
        static AccrualsContext()
        {
            Database.SetInitializer<AccrualsContext>(new AccrualsContextInitializer());
        }

        public AccrualsContext() : base("DbConnection")
        { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Properties<DateTime>().Configure(c => c.HasColumnType("datetime2"));

            modelBuilder.Entity<AbsenceFromWork>().HasKey(e => new { e.EmployeeId, e.Date });
            modelBuilder.Entity<AbsenceFromWork>().HasIndex(e => e.EmployeeId);

            modelBuilder.Entity<Employee>().HasKey(e => e.Id);
            modelBuilder.Entity<Employee>().HasIndex(e => e.Id).IsUnique();

            modelBuilder.Entity<PayRate>().HasKey(e => e.Date);
            modelBuilder.Entity<PayRate>().HasIndex(e => e.Date).IsUnique();


            modelBuilder.Entity<WorkingCalendar>().HasKey(e => e.Date);
            modelBuilder.Entity<WorkingCalendar>().HasIndex(e => e.Date).IsUnique();

            modelBuilder.Entity<WorkingPeriodsOfEmployees>().HasKey(e => new { e.EmployeeId, e.EmploymentDate });
            modelBuilder.Entity<WorkingPeriodsOfEmployees>().HasIndex(e => e.EmployeeId);

            modelBuilder.Entity<CompanySettings>().HasKey(e => e.Date);
            modelBuilder.Entity<CompanySettings>().HasIndex(e => e.Date).IsUnique();
        }

        public DbSet<Employee> Employees { get; set; }

        public DbSet<WorkingPeriodsOfEmployees> WorkingPeriodsOfEmployees { get; set; }

        public DbSet<AbsenceFromWork> AbsenceFromWork { get; set; }

        public DbSet<WorkingCalendar> WorkingCalendar { get; set; }

        public DbSet<PayRate> PayRates { get; set; }

        public DbSet<CompanySettings> CompanySettings { get; set; }
    }
}
