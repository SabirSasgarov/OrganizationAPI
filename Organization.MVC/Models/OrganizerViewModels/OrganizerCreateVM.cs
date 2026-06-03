using System.ComponentModel.DataAnnotations;

namespace Organization.MVC.Models.OrganizerViewModels
{
	public class OrganizerCreateVM
	{
		[Required]
		[StringLength(100, MinimumLength = 2)]
		public string Name { get; set; } = null!;
		[Required]
		[EmailAddress]
		public string Email { get; set; } = null!;
		[Phone]
		public string? Phone { get; set; }
		public IFormFile? File { get; set; }
	}
}
