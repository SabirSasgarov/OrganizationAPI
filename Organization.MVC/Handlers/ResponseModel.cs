namespace Organization.MVC.Handlers
{
	public class ResponseModel<T>
	{
		public bool Success { get; set; }
		public List<string> Errors { get; set; } = [];
		public T Data { get; set; } = default!;
	}
}
