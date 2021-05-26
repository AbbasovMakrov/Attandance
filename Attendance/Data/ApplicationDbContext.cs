using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Attendance.Data.Entities;
using Attendance.Data.Entities.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Attendance.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Budget> EmployeesBudgets { get; set; }
        public DbSet<ExcelFile> ExcelFiles { get; set; }
        public DbSet<MoneyTransaction> MoneyTransactions { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            SavingChanges += GenerateTimeStampsValues;
        }

       
        private void GenerateTimeStampsValues(object sender , SaveChangesEventArgs args)
        {
            ChangeTracker.Entries<ITimestampableEntity>()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
                .ToImmutableList()
                .ForEach(e =>
                {
                    switch (e.State)
                    {
                        case EntityState.Added:
                            e.Entity.CreatedAt = DateTime.Now;
                            e.Entity.UpdatedAt = DateTime.Now;
                            break;
                        case EntityState.Modified:
                            e.Entity.UpdatedAt = DateTime.Now;
                           // e.Entity.CreatedAt = e.Entity.CreatedAt;
                            break;
                    }
                });
        }
    }
}
