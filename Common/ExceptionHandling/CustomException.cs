namespace SyncSharpServer.Common.ExceptionHandling
{
	public class BadRequestException : Exception
	{
		public List<string>? Errors { get; set; }
		public BadRequestException(List<string> errors) : base("Bad Request")
		{
			Errors = errors;
		}
	}

	public class UnauthorizedException : Exception
	{
		public List<string>? Errors { get; set; }
		public UnauthorizedException(List<string> errors) : base("Unauthorized")
		{
			Errors = errors;
		}
	}
}
