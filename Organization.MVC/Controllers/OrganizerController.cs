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
	}
}
