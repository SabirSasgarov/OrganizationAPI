using OriganizationAPI.Models.Common;

namespace OriganizationAPI.Models
{
	public class Organizer : BaseEntity
	{
		public string Name { get; set; } = null!;
		public string Email { get; set; } = null!;
		public string? Phone { get; set; }
		public string? LogoUrl { get; set; }
		public List<Event> Events { get; set; } = new List<Event>();
	}
}
