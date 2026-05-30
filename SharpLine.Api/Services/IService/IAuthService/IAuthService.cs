using SharpLine.Api.Models.Dtos;

namespace SharpLine.Api.Services.IService.IAuthService
{
    public interface IAuthService
    {
        Task<string> Register(RegistrationRequestDto registrationRequestDto);
        Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto);
        Task<bool> AssignRole(string email, string roleName);
        Task<IList<string>> GetUserRoles(string emailOrUserName);
        Task<bool> RemoveRole(string email, string roleName);
        
    }
}
