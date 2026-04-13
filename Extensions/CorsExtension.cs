using System.Runtime.CompilerServices;

namespace SyncSharpServer.Extensions
{
	public static class CorsExtension
	{
		public static IServiceCollection AddCorsExtension(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddCors(options =>
			{
				options.AddPolicy("AllowedOrigins",
					builder =>
					{
						var origins = configuration.GetSection("CORS:AllowedOrigins").Get<string[]>();
						builder.WithOrigins(origins: origins)
						.AllowAnyHeader().AllowAnyMethod();
					});
			});
			return services;
		}
	}
}
