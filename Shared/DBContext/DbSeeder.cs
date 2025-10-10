using Microsoft.EntityFrameworkCore;
using ServicesGateManagment.Shared.Models.Enum;
using ServicesGateManagment.Shared.Models.Users;


namespace ServicesGateManagment.Shared.DBContext
{
    public static class DbSeeder
    {
        public static void SeedInitialData(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    FirstName = "ادمین",
                    LastName = "اصلی",
                    Email = "admin@example.com",
                    Password = PasswordHelper.HashPassword("Admin123!"), // هش شود در عمل
                    Role = UserRole.Admin,
                    CreatedUtc = DateTime.UtcNow,
                    LastModifiedUtc = DateTime.UtcNow
                }
            );
        }
    }
}




