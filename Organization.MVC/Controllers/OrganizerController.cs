using Microsoft.AspNetCore.Mvc;
using Organization.MVC.Models;

namespace Organization.MVC.Controllers
{
	public class OrganizerController : Controller
	{
		private readonly HttpClient _httpClient;

		public OrganizerController(IHttpClientFactory factory)
		{
			_httpClient = factory.CreateClient();
		}

		public async Task<IActionResult> Index()
		{
			var token = Request.Cookies["AuthToken"];
			if (!string.IsNullOrWhiteSpace(token))
			{
				_httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
			}
			else
			{
				return RedirectToAction("Login","Account");
			}
			var events = await _httpClient.GetFromJsonAsync<List<OrganizerViewModel>>("http://localhost:5195/api/Organizer");

			return View(events);
		}
		public async Task<IActionResult> OrganizerEvents(int id)
		{
			var token = Request.Cookies["AuthToken"];
			if (!string.IsNullOrWhiteSpace(token))
			{
				_httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
			}
			else
			{
				return RedirectToAction("Login", "Account");
			}
			var organizer = await _httpClient.GetFromJsonAsync<OrganizerViewModel>($"http://localhost:5195/api/Organizer/{id}/events");

			if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
			{
				// Return JSON for AJAX requests
				return Json(organizer);
			}

			return View(organizer);
		}


	}
}
