using Microsoft.AspNetCore.Mvc;
using WhatsAppConsume.Services;

namespace WhatsAppConsume.Controllers
{
    public class AuthController : Controller
    {
        private readonly IApiService _apiService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IApiService apiService, ILogger<AuthController> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> RequestOtp(string phoneNumber)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(phoneNumber))
                {
                    return Json(new { success = false, message = "Phone number is required" });
                }

                // Store phone number in session for verification step
                HttpContext.Session.SetString("LoginPhoneNumber", phoneNumber);

                var result = await _apiService.RequestOtpAsync(phoneNumber);

                return Json(new
                {
                    success = result.Success,
                    message = result.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error requesting OTP");
                return Json(new
                {
                    success = false,
                    message = "An error occurred while sending OTP"
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> VerifyOtp(string code)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(code))
                {
                    return Json(new { success = false, message = "OTP code is required" });
                }

                var phoneNumber = HttpContext.Session.GetString("LoginPhoneNumber");

                if (string.IsNullOrEmpty(phoneNumber))
                {
                    return Json(new
                    {
                        success = false,
                        message = "Session expired. Please start over."
                    });
                }

                var result = await _apiService.VerifyOtpAsync(phoneNumber, code);

                if (result.Success)
                {
                    // Clear the stored phone number
                    HttpContext.Session.Remove("LoginPhoneNumber");
                }

                return Json(new
                {
                    success = result.Success,
                    message = result.Message,
                    redirectUrl = result.Success ? Url.Action("Profile", "Home") : null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying OTP");
                return Json(new
                {
                    success = false,
                    message = "An error occurred while verifying OTP"
                });
            }
        }
    }
}
