using Microsoft.AspNetCore.Identity;
using SharpLine.Api.Data;
using Microsoft.EntityFrameworkCore;
using SharpLine.Api.Interfaces;
using SharpLine.Api.Models;
using SharpLine.Api.Models.Dtos;
using SharpLine.Api.Services.IService.IAuthService;

namespace SharpLine.Api.Services.AuthServices
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthService(ApplicationDbContext db, IJwtTokenGenerator jwtTokenGenerator,
            UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _jwtTokenGenerator = jwtTokenGenerator;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<bool> AssignRole(string email, string roleName)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(roleName))
                return false;

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return false;

            // Ensure role exists
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                var createRoleResult = await _roleManager.CreateAsync(new IdentityRole(roleName));
                if (!createRoleResult.Succeeded)
                    return false;
            }

            // If already in role, nothing to do
            if (await _userManager.IsInRoleAsync(user, roleName))
                return true;

            var addResult = await _userManager.AddToRoleAsync(user, roleName);
            return addResult.Succeeded;
        }

        public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
        {
            // Prefer using UserManager to locate the user (handles normalization and stores correctly)
            var user = await _userManager.FindByNameAsync(loginRequestDto.UserName);
            if (user == null)
            {
                // fallback to email lookup
                user = await _userManager.FindByEmailAsync(loginRequestDto.UserName);
            }

            if (user == null)
            {
                return new LoginResponseDto() { User = null, Token = "" };
            }

            var isValid = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);

            if (!isValid)
            {
                return new LoginResponseDto() { User = null, Token = "" };
            }

            //if user was found , Generate JWT Token
            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtTokenGenerator.GenerateToken(user, roles);

            UserDto userDTO = new()
            {
                Email = user.Email,
                ID = user.Id,
                Name = user.Name,
                PhoneNumber = user.PhoneNumber
            };

            LoginResponseDto loginResponseDto = new LoginResponseDto()
            {
                User = userDTO,
                Token = token
            };

            return loginResponseDto;
        }

        public async Task<string> Register(RegistrationRequestDto registrationRequestDto)
        {
            ApplicationUser user = new()
            {
                UserName = registrationRequestDto.Email,
                Email = registrationRequestDto.Email,
                NormalizedEmail = registrationRequestDto.Email.ToUpper(),
                Name = registrationRequestDto.Name,
                PhoneNumber = registrationRequestDto.PhoneNumber
            };

            try
            {
                // prevent duplicate registration
                var existing = await _userManager.FindByEmailAsync(registrationRequestDto.Email);
                if (existing != null)
                {
                    return "User with this email already exists";
                }

                // create user
                var result = await _userManager.CreateAsync(user, registrationRequestDto.Password);
                if (!result.Succeeded)
                {
                    return result.Errors.FirstOrDefault()?.Description ?? "Registration failed";
                }

                // enforce default role for newly registered users
                var defaultRole = "User";
                if (!await _roleManager.RoleExistsAsync(defaultRole))
                {
                    var createRoleResult = await _roleManager.CreateAsync(new IdentityRole(defaultRole));
                    if (!createRoleResult.Succeeded)
                    {
                        // rollback user creation
                        await _userManager.DeleteAsync(user);
                        return createRoleResult.Errors.FirstOrDefault()?.Description ?? "Failed to create default role";
                    }
                }

                var addToRoleResult = await _userManager.AddToRoleAsync(user, defaultRole);
                if (!addToRoleResult.Succeeded)
                {
                    // rollback user creation
                    await _userManager.DeleteAsync(user);
                    return addToRoleResult.Errors.FirstOrDefault()?.Description ?? "Failed to assign default role to user";
                }

                return string.Empty;

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public async Task<IList<string>> GetUserRoles(string emailOrUserName)
        {
            if (string.IsNullOrWhiteSpace(emailOrUserName))
                return new List<string>();

            var user = await _userManager.FindByNameAsync(emailOrUserName);
            if (user == null)
                user = await _userManager.FindByEmailAsync(emailOrUserName);

            if (user == null)
                return new List<string>();

            var roles = await _userManager.GetRolesAsync(user);
            return roles;
        }

        public async Task<bool> RemoveRole(string email, string roleName)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(roleName))
                return false;

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return false;

            if (!await _roleManager.RoleExistsAsync(roleName))
                return false;

            if (!await _userManager.IsInRoleAsync(user, roleName))
                return true; // already not in role

            var removeResult = await _userManager.RemoveFromRoleAsync(user, roleName);
            return removeResult.Succeeded;
        }


    }
}
