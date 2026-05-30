using CourseManagementAPI.Data;
using CourseManagementAPI.Models;
using CourseManagementAPI.Services;
using CourseManagementAPI.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CourseManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _config;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ITokenService tokenService,
            IConfiguration config,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _config = config;
            _roleManager = roleManager;
        }

        [HttpPost("register-trainee")]
        public async Task<ActionResult<AuthResponseDto>> RegisterTrainee(RegisterDto dto)
        {
            var existing = await _userManager.FindByEmailAsync(dto.Email);
            if (existing != null)
            {
                return Conflict(new { message = "An account with this email already exists." });
            }

            await EnsureRoleExistsAsync("Trainee");

            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                DisplayName = dto.DisplayName
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
            }

            await _userManager.AddToRoleAsync(user, "Trainee");

            return await BuildAuthResponseAsync(user);
        }

        [HttpPost("register-instructor")]
        public async Task<ActionResult<AuthResponseDto>> RegisterInstructor(RegisterDto dto)
        {
            var existing = await _userManager.FindByEmailAsync(dto.Email);
            if (existing != null)
            {
                return Conflict(new { message = "An account with this email already exists." });
            }

            await EnsureRoleExistsAsync("Instructor");

            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                DisplayName = dto.DisplayName
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
            }

            await _userManager.AddToRoleAsync(user, "Instructor");

            return await BuildAuthResponseAsync(user);
        }

        [HttpPost("register-coordinator")]
        public async Task<ActionResult<AuthResponseDto>> RegisterCoordinator(RegisterDto dto)
        {
            var existing = await _userManager.FindByEmailAsync(dto.Email);
            if (existing != null)
            {
                return Conflict(new { message = "An account with this email already exists." });
            }

            await EnsureRoleExistsAsync("TrainingCoordinator");

            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                DisplayName = dto.DisplayName
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
            }

            await _userManager.AddToRoleAsync(user, "TrainingCoordinator");

            return await BuildAuthResponseAsync(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }

            return await BuildAuthResponseAsync(user);
        }

        private async Task EnsureRoleExistsAsync(string roleName)
        {
            var roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        private async Task<AuthResponseDto> BuildAuthResponseAsync(ApplicationUser user)
        {
            var token = await _tokenService.CreateTokenAsync(user);
            var expiryMinutes = int.Parse(_config["Jwt:ExpiryMinutes"]!);
            var roles = await _userManager.GetRolesAsync(user);

            return new AuthResponseDto
            {
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes),
                Email = user.Email!,
                DisplayName = user.DisplayName ?? user.UserName!,
                Roles = roles
            };
        }
    }
}