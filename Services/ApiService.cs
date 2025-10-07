using System.Net.Http.Headers;
using WhatsAppConsume.Models;
using System.Net.Http.Json;
using System.Text.Json;
using System.IdentityModel.Tokens.Jwt;

namespace WhatsAppConsume.Services
{
    public class ApiService: IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public ApiService(
            HttpClient httpClient,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;

            // Set base address from configuration
            _httpClient.BaseAddress = new Uri(_configuration["ApiSettings:BaseUrl"]);
        }

        public async Task<AuthResponse> RequestOtpAsync(string phoneNumber)
        {
            try
            {
                var request = new LoginRequest { PhoneNumber = phoneNumber };
                var response = await _httpClient.PostAsJsonAsync("api/auth/login", request);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<AuthResponse>();
                }
                else
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "Failed to send OTP. Please try again."
                    };
                }
            }
            catch (Exception ex)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = $"Error: {ex.Message}"
                };
            }
        }

        public async Task<AuthResponse> VerifyOtpAsync(string phoneNumber, string code)
        {
            try
            {
                var request = new VerifyOtpRequest
                {
                    PhoneNumber = phoneNumber,
                    Code = code
                };

                var response = await _httpClient.PostAsJsonAsync("api/auth/verify-otp", request);

                if (response.IsSuccessStatusCode)
                {
                    var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();

                    if (authResponse?.Success == true && !string.IsNullOrEmpty(authResponse.Token))
                    {
                        StoreToken(authResponse.Token);
                    }

                    return authResponse;
                }
                else
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "Invalid OTP. Please try again."
                    };
                }
            }
            catch (Exception ex)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = $"Error: {ex.Message}"
                };
            }
        }

        public async Task<UserProfile> GetUserProfileAsync(string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.GetAsync("api/user/profile");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<UserProfile>();
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        public bool IsTokenValid(string token)
        {
            if (string.IsNullOrEmpty(token))
                return false;

            try
            {
                var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token);

                return jwtToken.ValidTo > DateTime.UtcNow;
            }
            catch
            {
                return false;
            }
        }

        public string GetStoredToken()
        {
            return _httpContextAccessor.HttpContext?.Session.GetString("JwtToken");
        }

        public void StoreToken(string token)
        {
            _httpContextAccessor.HttpContext?.Session.SetString("JwtToken", token);
        }

        public void ClearToken()
        {
            _httpContextAccessor.HttpContext?.Session.Remove("JwtToken");
        }
    }
}
