using Microsoft.AspNetCore.Mvc;
using Organization.MVC.Handlers;
using Organization.MVC.Models.OrganizerViewModels;
using System.Net.Http.Headers;

namespace Organization.MVC.Controllers
{
	public class OrganizerController : Controller
	{
		private readonly HttpClient _httpClient;

		public OrganizerController(IHttpClientFactory factory)
		{
			_httpClient = factory.CreateClient("ApiClient");
		}

		public async Task<IActionResult> Index()
		{
			var client = _httpClient;
			HttpContext.Request.Cookies.TryGetValue("AuthToken", out var authHeader);
			if (authHeader != null && authHeader != "")
			{
				var organizers = await client.GetFromJsonAsync<ResponseModel<List<OrganizerViewModel>>>("http://localhost:5195/api/Organizer");
				return View(organizers?.Data ?? new List<OrganizerViewModel>());
			}
			return RedirectToAction("Login", "Account");
		}

		public IActionResult Create()
		{
			return View(new OrganizerCreateVM());
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(OrganizerCreateVM model)
		{
			var client = _httpClient;

			if (!ModelState.IsValid)
			{
				return View(model);
			}

			using var content = new MultipartFormDataContent();
			content.Add(new StringContent(model.Name), nameof(model.Name));
			content.Add(new StringContent(model.Email), nameof(model.Email));

			if (!string.IsNullOrWhiteSpace(model.Phone))
			{
				content.Add(new StringContent(model.Phone), nameof(model.Phone));
			}

			if (model.File is { Length: > 0 })
			{
				var fileContent = new StreamContent(model.File.OpenReadStream());
				fileContent.Headers.ContentType = new MediaTypeHeaderValue(model.File.ContentType);
				content.Add(fileContent, nameof(model.File), model.File.FileName);
			}

			var response = await client.PostAsync("http://localhost:5195/api/Organizer", content);
			if (response.IsSuccessStatusCode)
			{
				TempData["SuccessMessage"] = "Organizer created successfully.";
				return RedirectToAction(nameof(Index));
			}

			var errorContent = await response.Content.ReadAsStringAsync();
			ModelState.AddModelError(string.Empty, string.IsNullOrWhiteSpace(errorContent)
				? "Could not create organizer."
				: errorContent);

			return View(model);
		}

		public async Task<IActionResult> OrganizerEvents(int id)
		{
			var client = _httpClient;

			var organizer = await client.GetFromJsonAsync<ResponseModel<OrganizerViewModel>>($"http://localhost:5195/api/Organizer/{id}/events");

			if (Request.Headers.XRequestedWith == "XMLHttpRequest")
			{
				return Json(organizer);
			}

			return View(organizer?.Data);
		}
		[HttpGet]
		public async Task<IActionResult> Update(int id)
		{
			var client = _httpClient;
			var organizer = await client.GetFromJsonAsync<ResponseModel<OrganizerViewModel>>($"http://localhost:5195/api/Organizer/{id}");

			if (organizer == null)
			{
				return NotFound();
			}

			return View(organizer?.Data);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Update(int id, IFormFile? file)
		{
			var client = _httpClient;
			var organizer = await client.GetFromJsonAsync<ResponseModel<OrganizerViewModel>>($"http://localhost:5195/api/Organizer/{id}");

			if (organizer == null)
			{
				return NotFound();
			}

			if (file == null || file.Length == 0)
			{
				ModelState.AddModelError(string.Empty, "Please select an image file to upload.");
				return View(organizer);
			}

			if (!ModelState.IsValid)
			{
				return View(organizer);
			}

			using var content = new MultipartFormDataContent();
			var fileContent = new StreamContent(file.OpenReadStream());
			fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
			content.Add(fileContent, "logo", file.FileName);

			var response = await client.PatchAsync($"http://localhost:5195/api/Organizer/{id}/logo", content);
			if (response.IsSuccessStatusCode)
			{
				return RedirectToAction(nameof(Index));
			}

			var errorContent = await response.Content.ReadAsStringAsync();
			ModelState.AddModelError(string.Empty, string.IsNullOrWhiteSpace(errorContent)
				? "An error occurred while updating the organizer."
				: errorContent);
			return View(organizer?.Data);
		}
	}
}
