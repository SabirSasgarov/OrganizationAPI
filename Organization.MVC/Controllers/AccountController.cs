using Microsoft.AspNetCore.Mvc;
using Organization.MVC.Models.AccountVMs;

namespace Organization.MVC.Controllers
{
	public class AccountController : Controller
	{
		private readonly IHttpClientFactory _httpClientFactory;

		public AccountController(IHttpClientFactory httpClientFactory)
		{
			_httpClientFactory = httpClientFactory;
		}

		public IActionResult Register()
		{
			return View(new RegisterViewModel());
		}

		[HttpPost]
		public async Task<IActionResult> Register(RegisterViewModel model)
		{
			if (!ModelState.IsValid) return View(model);

			HttpClient client = _httpClientFactory.CreateClient();
			var content = new FormUrlEncodedContent(new Dictionary<string, string>
			{
				["UserName"] = model.UserName,
				["FullName"] = model.FullName ?? string.Empty,
				["Email"] = model.Email,
				["Password"] = model.Password,
				["ConfirmPassword"] = model.ConfirmPassword
			});

			var response = await client.PostAsync("http://localhost:5195/api/Account/register", content);
			if (response.IsSuccessStatusCode)
			{
				TempData["SuccessMessage"] = "Registration successful! Please log in.";
				return RedirectToAction("Login");
			}

			var errorContent = await response.Content.ReadAsStringAsync();
			ModelState.AddModelError(string.Empty, $"Registration failed: {errorContent}");
			return View(model);
		}

		public IActionResult Login()
		{
			return View(new LoginViewModel());
		}

		[HttpPost]
		public async Task<IActionResult> Login(LoginViewModel model)
		{
			if (!ModelState.IsValid) return View(model);

			HttpClient client = _httpClientFactory.CreateClient();
			var content = new FormUrlEncodedContent(new Dictionary<string, string>
			{
				["Username"] = model.UserName,
				["Password"] = model.Password
			});

			var response = await client.PostAsync("http://localhost:5195/api/Account/login", content);
			if (response.IsSuccessStatusCode)
			{
				TempData["SuccessMessage"] = "Login successful!";
				return RedirectToAction("Index", "Home");
			}

			var errorContent = await response.Content.ReadAsStringAsync();
			ModelState.AddModelError(string.Empty, $"Login failed: {errorContent}");
			return View(model);
		}
	}
}
