namespace OriganizationAPI.Handler
{
	public class ResponseHandler<T>
	{
		public bool Success { get; set; }
		public List<string> Errors { get; set; } = null!;
		public T? Data { get; set; }

		public static ResponseHandler<T> SuccessResponse(T data)
		{
			return new ResponseHandler<T>
			{
				Success = true,
				Errors = [],
				Data = data
			};
		}

		public static ResponseHandler<T> FailureResponse(List<string> errors)
		{
			return new ResponseHandler<T>
			{
				Success = false,
				Errors = errors,
				Data = default
			};
		}
	}
}
