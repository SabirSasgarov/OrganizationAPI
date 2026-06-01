using Microsoft.AspNetCore.WebUtilities;
using System.Net;

namespace OriganizationAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AccountController(
		IValidator<RegisterDto> registerValidationRules,
		IValidator<LoginDto> loginValidationRules,
		IValidator<ResetPasswordDto> resetPasswordValidationRules,
		IValidator<ForgetPasswordDto> forgetPasswordValidationRules,
		UserManager<AppUser> userManager,
		RefreshTokenService refreshTokenService,
		RoleManager<IdentityRole> roleManager,
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

			var confirmEmailToken = await userManager.GenerateEmailConfirmationTokenAsync(user);
			confirmEmailToken= WebUtility.UrlEncode(confirmEmailToken);

			await userManager.AddToRoleAsync(user, "Member");

			return Ok($"{confirmEmailToken}\nAccount created successfully.");
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

			if (!user.EmailConfirmed)
				return BadRequest("Verify email first!");

			string tokenString = jwtService.GenerateToken(user, await userManager.GetRolesAsync(user));
			user.RefreshToken = refreshTokenService.GenerateRefreshToken();
			user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(5);

			await userManager.AddToRoleAsync(user, "Member");
			await userManager.UpdateAsync(user);
			 
			return Ok(new { Token = tokenString, Refresh = user.RefreshToken });
		}
		[Authorize]
		[HttpPost("reset_password")]
		public async Task<IActionResult> ResetPassword([FromForm] ResetPasswordDto resetPasswordDto)
		{
			if(resetPasswordDto == null)
				return BadRequest("Invalid data.");
			var validationResult = await resetPasswordValidationRules.ValidateAsync(resetPasswordDto);
			if (!validationResult.IsValid)
				return BadRequest(validationResult.Errors);

			string id = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value;
			var user = await userManager.FindByIdAsync(id);
			if (user == null)
				return BadRequest("User not found.");
			var changePasswordResult = await userManager.ChangePasswordAsync(user, resetPasswordDto.Password, resetPasswordDto.NewPassword);
			if (!changePasswordResult.Succeeded)
				return BadRequest(changePasswordResult.Errors);

			return Ok("Password reset successfully.");
		}
		[HttpPost("forget_password")]
		public async Task<IActionResult> ForgetPassword([FromForm] ForgetPasswordDto forgetPasswordDto)
		{
			var validationResult = await forgetPasswordValidationRules.ValidateAsync(forgetPasswordDto);
			if (!validationResult.IsValid)
				return BadRequest(validationResult.Errors);

			var user = await userManager.FindByEmailAsync(forgetPasswordDto.Email);
			if (user == null)
				return BadRequest("User not found.");

			var token = await userManager.GeneratePasswordResetTokenAsync(user);
			var edncodedToken = WebUtility.UrlEncode(token);
			
			// send email with token

			return Ok($"Password reset token generated ---> {edncodedToken}. Please check your email.");
		}


		[HttpPost("confirm_email")]
		public async Task<IActionResult> ConfirmEmail(string email, string token)
		{
			var user = await userManager.FindByEmailAsync(email);
			if (user == null)
				return BadRequest("User not found.");

			var decodedToken = WebUtility.UrlDecode(token);


			var result = await userManager.ConfirmEmailAsync(user, decodedToken);
			if (!result.Succeeded)
				return BadRequest("Email confirmation failed.");
			return Ok("Email confirmed successfully.");
		}

		[HttpPost("refresh-token")]
		public async Task<IActionResult> RefreshToken(TokenRequestDto dto)
		{
			var user = await userManager.Users
				.FirstOrDefaultAsync(x =>
					x.RefreshToken == dto.RefreshToken);

			if (user == null)
				return Unauthorized();

			if (user.RefreshTokenExpiryTime <= DateTime.UtcNow)
				return Unauthorized();

			var newAccessToken = jwtService.GenerateToken(user, await userManager.GetRolesAsync(user));
			var newRefreshToken = refreshTokenService.GenerateRefreshToken();

			user.RefreshToken = newRefreshToken;

			await userManager.UpdateAsync(user);

			return Ok(new
			{
				AccessToken = newAccessToken,
				RefreshToken = newRefreshToken
			});
		}



		#region add static roles
		[Authorize(Roles = "Admin")]
		[HttpPost("roles")]
		public async Task<IActionResult> AddRoles()
		{
			await roleManager.CreateAsync(new IdentityRole { Name = "Admin" });
			await roleManager.CreateAsync(new IdentityRole { Name = "Member" });

			return Ok("Roles created successfully.");
		}
		#endregion

	}
}
