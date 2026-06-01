using Microsoft.AspNetCore.Mvc;
using Organization.MVC.Models;

namespace Organization.MVC.Controllers
{
	public class EventController : Controller
	{
		private readonly HttpClient _httpClient;

		public EventController(IHttpClientFactory factory)
		{
			_httpClient = factory.CreateClient();
		}

		public async Task<IActionResult> Index()
		{
			var events = await _httpClient.GetFromJsonAsync<List<EventViewModel>>("http://localhost:5195/api/Events");

			return View(events);
		}
	}
}
