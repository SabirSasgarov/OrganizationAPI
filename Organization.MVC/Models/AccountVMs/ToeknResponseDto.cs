namespace Organization.MVC.Models.AccountVMs
{
	public class TokenResponseDto
	{
		public string accessToken { get; set; } = null!;
		public string refreshToken {  get; set; } = null!;
	}
}
