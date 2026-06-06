using Microsoft.AspNetCore.WebUtilities;
using OriganizationAPI.Handler;
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
		SignInManager<AppUser> signInManager,
		JwtService jwtService)
		: ControllerBase
	{
		[HttpPost("register")]
		public async Task<IActionResult> Register([FromForm] RegisterDto registerDto)
		{
			var validationResult = await registerValidationRules.ValidateAsync(registerDto);
			if (!validationResult.IsValid)
				return BadRequest(ResponseHandler<RegisterDto>.FailureResponse([.. validationResult.Errors.Select(e => e.ErrorMessage)]));

			var user = new AppUser
			{
				FullName = registerDto.FullName,
				UserName = registerDto.UserName,
				Email = registerDto.Email
			};
			var createResult = await userManager.CreateAsync(user, registerDto.Password);
			if (!createResult.Succeeded)
				return BadRequest(ResponseHandler<RegisterDto>.FailureResponse([.. createResult.Errors.Select(e => e.Description)]));

			var confirmEmailToken = await userManager.GenerateEmailConfirmationTokenAsync(user);
			confirmEmailToken= WebUtility.UrlEncode(confirmEmailToken);

			await userManager.AddToRoleAsync(user, "Member");

			return Ok(ResponseHandler<RegisterDto>.SuccessResponse(registerDto));
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromForm] LoginDto loginDto)
		{
			var validationResult = await loginValidationRules.ValidateAsync(loginDto);
			if (!validationResult.IsValid)
				return BadRequest(ResponseHandler<LoginDto>.FailureResponse([.. validationResult.Errors.Select(e => e.ErrorMessage)]));

			var user = await userManager.FindByNameAsync(loginDto.Username);
			if (user == null)
				return BadRequest(ResponseHandler<LoginDto>.FailureResponse(["Invalid username or password."]));

			//to add email confirmation use this code =>

			//if (!user.EmailConfirmed)
			//	return BadRequest("Verify email first!");

			var passwordValid = await userManager.CheckPasswordAsync(user, loginDto.Password);
			if (!passwordValid)
				return BadRequest(ResponseHandler<LoginDto>.FailureResponse(["Invalid username or password."]));

			string tokenString = jwtService.GenerateToken(user, await userManager.GetRolesAsync(user));
			user.RefreshToken = refreshTokenService.GenerateRefreshToken();
			user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(5);

			await userManager.AddToRoleAsync(user, "Member");
			await userManager.UpdateAsync(user);
			await signInManager.SignInAsync(user, passwordValid);
			//new { Token = tokenString, Refresh = user.RefreshToken }
			return Ok(ResponseHandler<TokenResponseDto>.SuccessResponse(new TokenResponseDto
			{
				AccessToken = tokenString,
				RefreshToken = user.RefreshToken
			}));
		}
		[Authorize]
		[HttpPost("reset_password")]
		public async Task<IActionResult> ResetPassword([FromForm] ResetPasswordDto resetPasswordDto)
		{
			if(resetPasswordDto == null)
				return BadRequest("Invalid data.");
			var validationResult = await resetPasswordValidationRules.ValidateAsync(resetPasswordDto);
			if (!validationResult.IsValid)
				return BadRequest(ResponseHandler<ResetPasswordDto>.FailureResponse([.. validationResult.Errors.Select(e => e.ErrorMessage)]));

			string id = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value;
			var user = await userManager.FindByIdAsync(id);
			if (user == null)
				return BadRequest(ResponseHandler<ResetPasswordDto>.FailureResponse(["User not found."]));
			var changePasswordResult = await userManager.ChangePasswordAsync(user, resetPasswordDto.Password, resetPasswordDto.NewPassword);
			if (!changePasswordResult.Succeeded)
				return BadRequest(ResponseHandler<ResetPasswordDto>.FailureResponse([.. changePasswordResult.Errors.Select(e => e.Description)]));

			return Ok(ResponseHandler<ResetPasswordDto>.SuccessResponse(resetPasswordDto));
		}
		[HttpPost("forget_password")]
		public async Task<IActionResult> ForgetPassword([FromForm] ForgetPasswordDto forgetPasswordDto)
		{
			var validationResult = await forgetPasswordValidationRules.ValidateAsync(forgetPasswordDto);
			if (!validationResult.IsValid)
				return BadRequest(ResponseHandler<ForgetPasswordDto>.FailureResponse([.. validationResult.Errors.Select(e => e.ErrorMessage)]));

			var user = await userManager.FindByEmailAsync(forgetPasswordDto.Email);
			if (user == null)
				return BadRequest(ResponseHandler<ForgetPasswordDto>.FailureResponse(["User not found."]));

			var token = await userManager.GeneratePasswordResetTokenAsync(user);
			var edncodedToken = WebUtility.UrlEncode(token);
			
			// send email with token

			return Ok(ResponseHandler<ForgetPasswordDto>.SuccessResponse(forgetPasswordDto));
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
				return Unauthorized(ResponseHandler<TokenRequestDto>.FailureResponse(["Invalid refresh token."]));

			if (user.RefreshTokenExpiryTime <= DateTime.UtcNow)
				return Unauthorized(ResponseHandler<TokenRequestDto>.FailureResponse(["Refresh token has expired."]));

			var newAccessToken = jwtService.GenerateToken(user, await userManager.GetRolesAsync(user));
			var newRefreshToken = refreshTokenService.GenerateRefreshToken();

			user.RefreshToken = newRefreshToken;

			await userManager.UpdateAsync(user);

			return Ok(ResponseHandler<TokenResponseDto>.SuccessResponse(new TokenResponseDto
			{
				AccessToken = newAccessToken,
				RefreshToken = newRefreshToken
			}));
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
