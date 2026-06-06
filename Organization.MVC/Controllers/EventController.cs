using Microsoft.AspNetCore.Mvc;
using Organization.MVC.Handlers;
using Organization.MVC.Models.EventViewModels;
using Organization.MVC.Models.OrganizerViewModels;
using Organization.MVC.Models.TicketViewModels;
using System.Net.Http.Headers;

namespace Organization.MVC.Controllers
{
	public class EventController : Controller
	{
		private readonly HttpClient _httpClient;

		public EventController(IHttpClientFactory factory) => _httpClient = factory.CreateClient("ApiClient");

		public async Task<IActionResult> Index()
		{
			var response = await _httpClient.GetFromJsonAsync<ResponseModel<List<EventViewModel>>>("http://localhost:5195/api/Events");
			if (response == null || !response.Success)
			{
				ViewBag.ErrorMessage = response?.Errors?.FirstOrDefault() ?? "An error occurred while fetching events.";
				return View(new List<EventViewModel>());
			}

			return View(response.Data);
		}
		[HttpGet]
		public async Task<IActionResult> Create()
		{
			var organizers = await _httpClient.GetFromJsonAsync<ResponseModel<List<OrganizerViewModel>>>("http://localhost:5195/api/Organizer");
			var model = new EventCreateVM
			{
				Organizers = organizers?.Data ?? []
			};
			return View(model);
		}
		[HttpPost]
		public async Task<IActionResult> Create(EventCreateVM model)
		{
			var client = _httpClient;
			if (!ModelState.IsValid)
			{
				return View(model);
			}
			using var content = new MultipartFormDataContent();
			content.Add(new StringContent(model.Title), nameof(model.Title));
			content.Add(new StringContent(model.Description ?? string.Empty), nameof(model.Description));
			content.Add(new StringContent(model.Date.ToString("o")), nameof(model.Date));
			content.Add(new StringContent(model.Location), nameof(model.Location));
			content.Add(new StringContent(model.OrganizerId.ToString()), nameof(model.OrganizerId));
			if (model.File is { Length: > 0 })
			{
				var fileContent = new StreamContent(model.File.OpenReadStream());
				fileContent.Headers.ContentType = new MediaTypeHeaderValue(model.File.ContentType);
				content.Add(fileContent, nameof(model.File), model.File.FileName);
			}
			var response = await client.PostAsync("http://localhost:5195/api/Events", content);
			if (response.IsSuccessStatusCode)
			{
				return RedirectToAction(nameof(Index));
			}
			var errorContent = await response.Content.ReadAsStringAsync();
			ModelState.AddModelError(string.Empty, string.IsNullOrWhiteSpace(errorContent)
				? "An error occurred while creating the event."
				: errorContent);
			return View(model);
		}
		//[HttpPost]
		//public async Task<IActionResult> Delete(int id)
		//{
		//	var response = await _httpClient.DeleteAsync($"http://localhost:5195/api/Events/{id}");
		//	if (response.IsSuccessStatusCode)
		//	{
		//		return RedirectToAction(nameof(Index));
		//	}
		//	var errorContent = await response.Content.ReadAsStringAsync();
		//	ModelState.AddModelError(string.Empty, string.IsNullOrWhiteSpace(errorContent)
		//		? "An error occurred while deleting the event."
		//		: errorContent);
		//	return RedirectToAction(nameof(Index));
		//}
		public async Task<IActionResult> EventDetails(int id)
		{
			var eventt = await _httpClient.GetFromJsonAsync<ResponseModel<EventViewModel>>($"http://localhost:5195/api/Events/{id}");
			var tickets = await _httpClient.GetFromJsonAsync<ResponseModel<List<TicketViewModel>>>($"http://localhost:5195/api/Events/{id}/tickets");
			if (eventt == null || !eventt.Success)
			{
				ViewBag.EventErrorMessage = eventt?.Errors?.FirstOrDefault() ?? "An error occurred while fetching event details.";
				return NotFound("Event not found.");
			}
			if(tickets == null || !tickets.Success)
			{
				ViewBag.TicketsErrorMessage = tickets?.Errors?.FirstOrDefault() ?? "An error occurred while fetching tickets.";
				return View(new List<TicketViewModel>());
			}
			ViewBag.EventTitle = eventt?.Data?.Title ?? "Unknown Event";
			ViewBag.EventDescription = eventt?.Data?.Description ?? "No Description";
			ViewBag.EventDate = eventt?.Data?.Date.ToString("f") ?? "No Date";
			ViewBag.BannerImage = eventt?.Data?.BannerImage ?? "/images/default-banner.jpg";
			ViewBag.EventLocation = eventt?.Data?.Location ?? "No Location";
			ViewBag.OrganizerName = eventt?.Data?.Organizer.Name ?? "Unknown Organizer";
			ViewBag.OrganizerEmail = eventt?.Data?.Organizer.Email ?? "No Email";
			ViewBag.OrganizerPhone = eventt?.Data?.Organizer.Phone ?? "No Phone";
			if (tickets == null)
			{
				return NotFound("Tickets not found.");
			}
			return View(tickets?.Data ?? []);
		}
		[HttpGet]
		public async Task<IActionResult> Update(int id)
		{
			var client = _httpClient;
			var events = await client.GetFromJsonAsync<ResponseModel<List<EventViewModel>>>("http://localhost:5195/api/Events");
			if(events == null)
				return NotFound("Events not found.");
			var eventItem = events?.Data?.FirstOrDefault(e => e.Id == id);

			if (eventItem == null)
			{
				return NotFound();
			}

			return View(eventItem);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Update(int id, IFormFile? file)
		{
			var client = _httpClient;
			var events = await client.GetFromJsonAsync<ResponseModel<List<EventViewModel>>>("http://localhost:5195/api/Events");
			var eventItem = events?.Data?.FirstOrDefault(e => e.Id == id);
			if(eventItem == null)
				return NotFound("Event not found.");
			if (eventItem == null)
				return NotFound();
			

			if (file == null || file.Length == 0)
			{
				ModelState.AddModelError(string.Empty, "Please select an image file to upload.");
				return View(eventItem);
			}

			if (!ModelState.IsValid)
			{
				return View(eventItem);
			}
			using var content = new MultipartFormDataContent();
			var fileContent = new StreamContent(file.OpenReadStream());
			fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
			content.Add(fileContent, "banner", file.FileName);

			var response = await client.PatchAsync($"http://localhost:5195/api/Events/{id}/banner", content);
			if (response.IsSuccessStatusCode)
			{
				return RedirectToAction(nameof(Index));
			}
			var errorContent = await response.Content.ReadAsStringAsync();
			ModelState.AddModelError(string.Empty, string.IsNullOrWhiteSpace(errorContent)
				? "An error occurred while updating the event."
				: errorContent);
			return View(eventItem);
		}
		
	}
}
