using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WhatsAppConsume.Models;
using WhatsAppConsume.Services;

namespace WhatsAppConsume.Controllers
{
    public class HomeController : Controller
    {
        private readonly IApiService _apiService;

        public HomeController(IApiService apiService)
        {
            _apiService = apiService;
        }

        public IActionResult Index()
        {
            var token = _apiService.GetStoredToken();

            if (!string.IsNullOrEmpty(token) && _apiService.IsTokenValid(token))
            {
                return RedirectToAction("Profile");
            }

            return View();
        }

        public IActionResult Login()
        {
            var token = _apiService.GetStoredToken();

            if (!string.IsNullOrEmpty(token) && _apiService.IsTokenValid(token))
            {
                return RedirectToAction("Profile");
            }

            return View();
        }

        public IActionResult Verify()
        {
            var token = _apiService.GetStoredToken();

            if (!string.IsNullOrEmpty(token) && _apiService.IsTokenValid(token))
            {
                return RedirectToAction("Profile");
            }

            return View();
        }

        public async Task<IActionResult> Profile()
        {
            var token = _apiService.GetStoredToken();

            if (string.IsNullOrEmpty(token) || !_apiService.IsTokenValid(token))
            {
                _apiService.ClearToken();
                return RedirectToAction("Login");
            }

            var profile = await _apiService.GetUserProfileAsync(token);

            if (profile == null)
            {
                _apiService.ClearToken();
                return RedirectToAction("Login");
            }

            return View(profile);
        }

        public IActionResult Logout()
        {
            _apiService.ClearToken();
            return RedirectToAction("Index");
        }
    }
}
