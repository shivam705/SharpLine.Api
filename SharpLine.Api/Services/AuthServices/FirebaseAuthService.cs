using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SharpLine.Api.Models;
using SharpLine.Api.Models.Dtos.FirebaseDTOs;
using SharpLine.Api.Services.IService.IFirebaseService;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;



namespace SharpLine.Api.Services.AuthServices
{
    public class FirebaseAuthService : IFirebaseAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;

        public FirebaseAuthService(
            UserManager<ApplicationUser> userManager,
            IConfiguration config)
        {
            _userManager = userManager;
            _config = config;
        }

        public async Task<AuthResponse> LoginWithFirebaseAsync(string idToken)
        {
            // 1. Verify Firebase token
            var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken);

            var phoneNumber = decodedToken.Claims["phone_number"]?.ToString();

            if (string.IsNullOrEmpty(phoneNumber))
                throw new Exception("Invalid Firebase token");

            // 2. Check if user exists
            var user = await _userManager.Users
                .FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);

            // 3. Create user if not exists
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = phoneNumber,
                    PhoneNumber = phoneNumber,
                    PhoneNumberConfirmed = true
                };

                var result = await _userManager.CreateAsync(user);

                if (!result.Succeeded)
                    throw new Exception("User creation failed");
            }

            // 4. Generate JWT
            var token = GenerateJwtToken(user);

            return new AuthResponse
            {
                Token = token,
                PhoneNumber = phoneNumber
            };
        }

        private string GenerateJwtToken(ApplicationUser user)
        {
            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName)
        };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiry = DateTime.UtcNow.AddDays(7);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: expiry,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
