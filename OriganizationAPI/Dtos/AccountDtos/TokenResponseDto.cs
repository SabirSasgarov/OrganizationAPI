namespace OriganizationAPI.Dtos.AccountDtos
{
	public class TokenResponseDto
	{
		public string AccessToken { get; set; } = null!;
		public string RefreshToken { get; set; } = null!;
	}
}
