using SharpLine.Api.Models.Dtos.FirebaseDTOs;

namespace SharpLine.Api.Services.IService.IFirebaseService
{
    public interface IFirebaseAuthService
    {
        Task<AuthResponse> LoginWithFirebaseAsync(string idToken);
    }
}
