namespace OriganizationAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AccountController(
		IValidator<RegisterDto> registerValidationRules,
		IValidator<LoginDto> loginValidationRules,
		UserManager<AppUser> userManager,
		//RoleManager<IdentityRole> roleManager,
		JwtService jwtService)
		: ControllerBase
	{
		[HttpPost("register")]
		public async Task<IActionResult> Register([FromForm] RegisterDto registerDto)
		{
			var validationResult = await registerValidationRules.ValidateAsync(registerDto);
			if (!validationResult.IsValid)
				return BadRequest(validationResult.Errors);

			var user = new AppUser
			{
				FullName = registerDto.FullName,
				UserName = registerDto.UserName,
				Email = registerDto.Email
			};
			var createResult = await userManager.CreateAsync(user, registerDto.Password);
			if (!createResult.Succeeded)
				return BadRequest(createResult.Errors);

			await userManager.AddToRoleAsync(user, "Member");
			return Ok("Account created successfully.");
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromForm] LoginDto loginDto)
		{
			var validationResult = await loginValidationRules.ValidateAsync(loginDto);
			if (!validationResult.IsValid)
				return BadRequest(validationResult.Errors);

			var user = await userManager.FindByNameAsync(loginDto.Username);
			if (user == null)
				return BadRequest("Invalid username or password.");

			var passwordValid = await userManager.CheckPasswordAsync(user, loginDto.Password);
			if (!passwordValid)
				return BadRequest("Invalid username or password.");

			string tokenString = jwtService.GenerateToken(user, await userManager.GetRolesAsync(user));
			await userManager.AddToRoleAsync(user, "Member");

			return Ok(new { Token = tokenString });
		}
		
		#region add static roles
		//[Authorize(Roles = "Admin")]
		//[HttpPost("roles")]
		//public async Task<IActionResult> AddRoles()
		//{	
		//	await roleManager.CreateAsync(new IdentityRole { Name = "Admin" });
		//	await roleManager.CreateAsync(new IdentityRole { Name = "Member" });

		//	return Ok("Roles created successfully.");
		//}
		#endregion


		// reset password
		// confirmation email
		// forget password
		// refresh token
	}
}
