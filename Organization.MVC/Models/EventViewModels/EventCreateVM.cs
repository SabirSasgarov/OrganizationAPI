using System.ComponentModel.DataAnnotations;
using Organization.MVC.Models.OrganizerViewModels;

namespace Organization.MVC.Models.EventViewModels
{
	public class EventCreateVM
	{
		[Required]
		public string Title { get; set; } = null!;
		public string? Description { get; set; }
		[Required]
		[DataType(DataType.DateTime)]
		public DateTime Date { get; set; }
		[Required]
		public string Location { get; set; } = null!;
		public IFormFile? File { get; set; }
		[Required]
		public int OrganizerId { get; set; }
		public List<OrganizerViewModel>? Organizers { get; set; }
	}
}
