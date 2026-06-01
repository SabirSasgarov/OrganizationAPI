using System.ComponentModel.DataAnnotations;

namespace Organization.MVC.Models.AccountVMs
{
	public class RegisterViewModel
	{
		[Required]
		public string UserName { get; set; } = null!;
		public string? FullName { get; set; }
		[Required]
		[EmailAddress]
		public string Email { get; set; } = null!;
		[Required]
		[DataType(DataType.Password)]
		public string Password { get; set; } = null!;
		[Required]
		[DataType(DataType.Password)]
		[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
		public string ConfirmPassword { get; set; } = null!;
	}
}
