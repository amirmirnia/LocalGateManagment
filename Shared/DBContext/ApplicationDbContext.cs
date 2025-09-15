using Microsoft.EntityFrameworkCore;
using ServicesGateManagment.Shared.Common;
using ServicesGateManagment.Shared.Models.Vehicles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesGateManagment.Shared.DBContext
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSet برای مدل‌های شما
        public DbSet<VehicleInquireRequestJson> VehicleInquireRequestJson { get; set; }

        //public DbSet<VehicleInquire> VehicleInquire { get; set; }
        //public DbSet<VehicleInquireResult> VehicleInquireResult { get; set; }

        public override int SaveChanges()
        {
            UpdateAuditFields();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateAuditFields();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateAuditFields()
        {
            var entries = ChangeTracker.Entries<BaseAuditableEntity>();

            foreach (var entry in entries)
            {
                var now = DateTime.UtcNow;

                if (entry.State == EntityState.Added)
                {
                    // هنگام ایجاد
                    entry.Entity.CreatedUtc = now;
                    entry.Entity.LastModifiedUtc = now;
                    entry.Entity.CreatedBy =  "SystemLocal"; // تنظیم کاربر (اختیاری)
                    entry.Entity.LastModifiedBy = "SystemLocal";
                }
                else if (entry.State == EntityState.Modified)
                {
                    // هنگام به‌روزرسانی
                    entry.Entity.LastModifiedUtc = now;
                    entry.Entity.LastModifiedBy = "SystemLocal";
                }
            }
        }


    }


}
