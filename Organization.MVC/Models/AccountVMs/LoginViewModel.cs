using System.ComponentModel.DataAnnotations;

namespace Organization.MVC.Models.AccountVMs
{
	public class LoginViewModel
	{
		[Required(ErrorMessage = "Username is required.")]
		public string UserName { get; set; } = null!;
		[Required(ErrorMessage = "Password is required.")]
		public string Password { get; set; } = null!;
	}
}
