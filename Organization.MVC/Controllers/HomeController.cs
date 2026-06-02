using Microsoft.AspNetCore.Mvc;

namespace Organization.MVC.Controllers
{
	public class HomeController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
