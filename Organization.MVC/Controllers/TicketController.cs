using Microsoft.AspNetCore.Mvc;
using Organization.MVC.Models;

namespace Organization.MVC.Controllers
{
	public class TicketController : Controller
	{
		private readonly HttpClient _httpClient;

		public TicketController(IHttpClientFactory factory)
		{
			_httpClient = factory.CreateClient();
		}

		public async Task<IActionResult> Index()
		{
			var events = await _httpClient.GetFromJsonAsync<List<TicketViewModel>>("http://localhost:5195/api/Ticket");

			return View(events);
		}
	}
}
