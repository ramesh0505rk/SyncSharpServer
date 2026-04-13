using SyncSharpServer.Interfaces;
using SyncSharpServer.Repository;
using SyncSharpServer.Services;

namespace SyncSharpServer.Extensions
{
	public static class ApiServiceExtension
	{
		public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddAuthentication(configuration);
			services.AddCorsExtension(configuration);

			//Register repository interfaces
			services.AddScoped<IUserRepository, UserRepository>();

			//Register service interfaces
			services.AddScoped<IUserService, UserService>();

			return services;
		}
	}
}
