namespace OriganizationAPI.Dtos.OrganizerDtos
{
	public class OrganizerCreateDto
	{
		public string Name { get; set; } = null!;
		public string Email { get; set; } = null!;
		public string? Phone { get; set; }
		public IFormFile? File { get; set; }
	}
}
