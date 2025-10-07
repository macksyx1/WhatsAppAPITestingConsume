using WhatsAppConsume.Models;

namespace WhatsAppConsume.Services
{
    public interface IApiService
    {
        Task<AuthResponse> RequestOtpAsync(string phoneNumber);
        Task<AuthResponse> VerifyOtpAsync(string phoneNumber, string code);
        Task<UserProfile> GetUserProfileAsync(string token);
        bool IsTokenValid(string token);
        string GetStoredToken();
        void StoreToken(string token);
        void ClearToken();
    }
}
