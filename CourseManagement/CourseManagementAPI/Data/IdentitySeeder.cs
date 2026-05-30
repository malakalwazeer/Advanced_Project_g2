using Microsoft.AspNetCore.Identity;
using CourseManagementAPI.Models;

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

            var rolesToSeed = new[] { TrainingCoordinatorRole, InstructorRole, TraineeRole };
            foreach (var roleName in rolesToSeed)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            const string coordinatorEmail = "admin@CourseManagementAPI.local";
            var coordinatorUser = await userManager.FindByEmailAsync(coordinatorEmail);

            if (coordinatorUser == null)
            {
                coordinatorUser = new ApplicationUser
                {
                    UserName = coordinatorEmail,
                    Email = coordinatorEmail,
                    EmailConfirmed = true,
                    DisplayName = "Training Coordinator Admin"
                };

                var result = await userManager.CreateAsync(coordinatorUser, "Admin#12345");
                if (result.Succeeded)
                {
                    await AssignRoleAsync(userManager, coordinatorUser, TrainingCoordinatorRole);
                }
            }
            else
            {
                await AssignRoleAsync(userManager, coordinatorUser, TrainingCoordinatorRole);
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