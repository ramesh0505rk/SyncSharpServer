namespace SyncSharpServer.Common.Response
{
	public class ApiErrorResponse
	{
		public ApiError? Errors { get; set; }
	}

	public class ApiError
	{
		public string? Message { get; set; }
		public string? Details { get; set; }
}
}
