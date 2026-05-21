namespace OriganizationAPI.Models
{
	public class AppUser : IdentityUser
	{
		public string? FullName { get; set; }
	}
}
