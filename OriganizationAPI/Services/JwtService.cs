namespace OriganizationAPI.Services
{
	public class JwtService(IOptions<JwtSetting> jwtOptions)
	{
		public string GenerateToken(AppUser user, IList<string> roles)
		{
			var claims = new List<Claim>
			{
				new (ClaimTypes.NameIdentifier, user.Id.ToString()),
				new (ClaimTypes.Name, user.UserName!),
				new ("FullName", user.FullName!)
			};


			claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
			var jwtSetting = jwtOptions.Value;
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSetting.Key));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var tokenDescriptor = new JwtSecurityToken(
				issuer: jwtSetting.Issuer,
				audience: jwtSetting.Audience,
				claims: claims,
				expires: DateTime.Now.AddMinutes(jwtSetting.Expire),
				signingCredentials: creds
				);

			var tokenHandler = new JwtSecurityTokenHandler();
			var jwtToken = tokenHandler.WriteToken(tokenDescriptor);

			return jwtToken;
		}
	}
}
