using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OriganizationAPI.Models;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OriganizationAPI.Services
{
	public class JwtService(IOptions<JwtSetting> jwtOptions)
	{
		public string GenerateToken(AppUser user, IList<string> roles)
		{
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
				new Claim(ClaimTypes.Name, user.UserName!),
				new Claim("FullName", user.FullName!)
			};


			claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
			var jwtSetting = jwtOptions.Value;
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSetting.Key));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				issuer: jwtSetting.Issuer,
				audience: jwtSetting.Audience,
				claims: claims,
				expires: DateTime.Now.AddSeconds(jwtSetting.Expire),
				signingCredentials: creds
				);

			var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

			return tokenString;
		}
	}
}
