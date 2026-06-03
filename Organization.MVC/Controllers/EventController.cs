using Microsoft.AspNetCore.Mvc;
using Organization.MVC.Models.EventViewModels;
using Organization.MVC.Models.OrganizerViewModels;
using Organization.MVC.Models.TicketViewModels;
using System.Net.Http.Headers;

namespace Organization.MVC.Controllers
{
	public class EventController : Controller
	{
		private readonly HttpClient _httpClient;

		public EventController(IHttpClientFactory factory)
		{
			_httpClient = factory.CreateClient("ApiClient");
		}

		public async Task<IActionResult> Index()
		{
			var events = await _httpClient.GetFromJsonAsync<List<EventViewModel>>("http://localhost:5195/api/Events");

			return View(events);
		}
		[HttpGet]
		public IActionResult Create()
		{
			return View(new EventCreateVM());
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
			var client = _httpClient;
			var eventt = await client.GetFromJsonAsync<EventViewModel>($"http://localhost:5195/api/Events/{id}");
			var tickets = await client.GetFromJsonAsync<List<TicketViewModel>>($"http://localhost:5195/api/Events/{id}/tickets");
			//var organizer = await client.GetFromJsonAsync<OrganizerViewModel>($"http://localhost:5195/api/Events/{id}/organizer");
			ViewBag.EventTitle = eventt?.Title ?? "Unknown Event";
			ViewBag.EventDescription = eventt?.Description ?? "No Description";
			ViewBag.EventDate = eventt?.Date.ToString("f") ?? "No Date";
			ViewBag.BannerImage = eventt?.BannerImage ?? "/images/default-banner.jpg";
			ViewBag.EventLocation = eventt?.Location ?? "No Location";
			ViewBag.OrganizerName = eventt?.Organizer.Name ?? "Unknown Organizer";
			ViewBag.OrganizerEmail = eventt?.Organizer.Email ?? "No Email";
			ViewBag.OrganizerPhone = eventt?.Organizer.Phone ?? "No Phone";
			if (tickets == null)
			{
				return NotFound();
			}
			return View(tickets);
		}
		[HttpGet]
		public async Task<IActionResult> Update(int id)
		{
			var client = _httpClient;
			var events = await client.GetFromJsonAsync<List<EventViewModel>>("http://localhost:5195/api/Events");
			var eventItem = events?.FirstOrDefault(e => e.Id == id);

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
			var events = await client.GetFromJsonAsync<List<EventViewModel>>("http://localhost:5195/api/Events");
			var eventItem = events?.FirstOrDefault(e => e.Id == id);

			if (eventItem == null)
			{
				return NotFound();
			}

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
