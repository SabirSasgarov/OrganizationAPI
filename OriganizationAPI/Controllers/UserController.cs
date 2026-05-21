namespace OriganizationAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UserController(UserManager<AppUser> userManager) : ControllerBase
	{
		[Authorize(Roles = "Admin")]
		[HttpGet("all_users")]
		public async Task<IActionResult> Get()
		{
			var users = await userManager.Users.ToListAsync();
			return Ok(users);
		}
		[Authorize]
		[HttpGet("profile")]
		public async Task<IActionResult> GetProfile()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId == null)
				return Unauthorized("User ID not found in token.");
			var user = await userManager.FindByIdAsync(userId);
			if (user == null)
				return NotFound("User not found.");
			return Ok(new { user.FullName, user.UserName, user.Email });
		}
	}
}
