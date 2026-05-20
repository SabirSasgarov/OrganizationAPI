using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OriganizationAPI.Dtos.AccountDtos;
using OriganizationAPI.Models;
using OriganizationAPI.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OriganizationAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AccountController(
		IValidator<RegisterDto> registerValidationRules,
		IValidator<LoginDto> loginValidationRules,
		UserManager<AppUser> userManager,
		RoleManager<IdentityRole> roleManager,
		JwtService jwtService)
		: ControllerBase
	{
		[HttpGet]
		public async Task<IActionResult> Get()
		{
			var users = await userManager.Users.ToListAsync();
			return Ok(users);
		}

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

			
			//dsajduhajisdauisdjajsd

			string tokenString = jwtService.GenerateToken(user, await userManager.GetRolesAsync(user));

			return Ok(new { Token = tokenString });
		}
		[Authorize]
		[HttpGet("profile")]
		public async Task<IActionResult> GetProfile()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if(userId == null)
				return Unauthorized("User ID not found in token.");
			var user = await userManager.FindByIdAsync(userId);
			if(user == null)
				return NotFound("User not found.");
			return Ok(new { FullName = user.FullName, UserName = user.UserName, Email = user.Email });
		}


		[HttpPost("roles")]
		public async Task<IActionResult> AddRoles()
		{	
			await roleManager.CreateAsync(new IdentityRole { Name = "Admin" });
			await roleManager.CreateAsync(new IdentityRole { Name = "Member" });

			return Ok("Roles created successfully.");
		}

	}
}
