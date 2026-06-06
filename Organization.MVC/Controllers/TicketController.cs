using Microsoft.AspNetCore.Mvc;
using Organization.MVC.Handlers;
using Organization.MVC.Models.EventViewModels;
using Organization.MVC.Models.TicketViewModels;

namespace Organization.MVC.Controllers
{
	public class TicketController : Controller
	{
		private readonly HttpClient _httpClient;

		public TicketController(IHttpClientFactory factory)
		{
			_httpClient = factory.CreateClient("ApiClient");
		}

		public async Task<IActionResult> Index()
		{
			var events = await _httpClient.GetFromJsonAsync<ResponseModel<List<TicketViewModel>>>("http://localhost:5195/api/Ticket");

			return View(events?.Data ?? new List<TicketViewModel>());
		}
		[HttpGet]
		public async Task<IActionResult> Create()
		{
			var events = await _httpClient.GetFromJsonAsync<ResponseModel<List<EventViewModel>>>("http://localhost:5195/api/Events");
			var model = new TicketCreateVM
			{
				Events = events?.Data ?? new List<EventViewModel>()
			};
			return View(model);
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(TicketCreateVM model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}
			var client = _httpClient;
			using var content = new MultipartFormDataContent();
			content.Add(new StringContent(model.Type), nameof(model.Type));
			content.Add(new StringContent(model.EventId.ToString()), nameof(model.EventId));
			content.Add(new StringContent(model.QuantityAvailable.ToString()), nameof(model.QuantityAvailable));
			content.Add(new StringContent(model.Price.ToString()), nameof(model.Price));

			var response = await client.PostAsync("http://localhost:5195/api/Ticket", content);
			if (response.IsSuccessStatusCode)
			{
				return RedirectToAction(nameof(Index));
			}
			var errorContent = await response.Content.ReadAsStringAsync();
			ModelState.AddModelError(string.Empty, string.IsNullOrWhiteSpace(errorContent)
				? "An error occurred while creating the ticket."
				: errorContent);

			return View(model);
		}
	}
}
