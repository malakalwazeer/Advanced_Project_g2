using Microsoft.AspNetCore.Identity;
using CourseManagementAPI.Models;


namespace CourseManagementAPI.Data
{
    public class IdentitySeeder
    {
        public const string AdminRole = "Admin";
        public const string UserRole = "User";

        public static async Task InitializeAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            // Seed roles 
            foreach (var roleName in new[] { AdminRole, UserRole })
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Seed default admin user 
            const string adminEmail = "admin@CourseManagementAPI.local";
            var admin = await userManager.FindByEmailAsync(adminEmail);
            if (admin == null)
            {
                admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    DisplayName = "System Administrator"
                };

                var result = await userManager.CreateAsync(admin, "Admin#12345");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, AdminRole);
                }
            }
        }
    }
}
