using Microsoft.AspNetCore.Identity;
using CourseManagementAPI.Models;
using Microsoft.Extensions.DependencyInjection;

namespace CourseManagementAPI.Data
{
    public class IdentitySeeder
    {
        public const string TrainingCoordinatorRole = "TrainingCoordinator";
        public const string InstructorRole = "Instructor";
        public const string TraineeRole = "Trainee";

        public static async Task InitializeAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            // 1. Seed Roles
            var rolesToSeed = new[] { TrainingCoordinatorRole, InstructorRole, TraineeRole };
            foreach (var roleName in rolesToSeed)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // 2. Seed Training Coordinator (Admin)
            await EnsureUserExistsAsync(
                userManager,
                "admin@CourseManagementAPI.local",
                "Admin#12345",
                "Training Coordinator Admin",
                TrainingCoordinatorRole
            );

            // 3. Seed Instructor
            await EnsureUserExistsAsync(
                userManager,
                "ahmed.ali@example.com",
                "Temp123!",
                "Ahmed Ali",
                InstructorRole
            );

            await EnsureUserExistsAsync(
                userManager,
                "sara.hassan@example.com",
                "Temp123!",
                "Sara Hassan",
                InstructorRole
            );

            // 4. Seed Trainee
            await EnsureUserExistsAsync(
                userManager,
                "noor@example.com",
                "Temp123!",
                "Noor Mohammed",
                TraineeRole
            );

            await EnsureUserExistsAsync(
                userManager,
                "ali@example.com",
                "Temp123!",
                "Ali Yusuf",
                TraineeRole
            );
        }

        // Helper method to look up, create, and assign roles to users cleanly
        private static async Task EnsureUserExistsAsync(
            UserManager<ApplicationUser> userManager,
            string email,
            string password,
            string displayName,
            string role)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    DisplayName = displayName
                };

                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await AssignRoleAsync(userManager, user, role);
                }
            }
            else
            {
                await AssignRoleAsync(userManager, user, role);
            }
        }

        private static async Task AssignRoleAsync(
            UserManager<ApplicationUser> userManager,
            ApplicationUser user,
            string role)
        {
            if (!await userManager.IsInRoleAsync(user, role))
            {
                await userManager.AddToRoleAsync(user, role);
            }
        }
    }
}