using FluentValidation;
using FluentValidation.AspNetCore;
using SyncSharpServer.Interfaces;
using SyncSharpServer.Presistence;
using SyncSharpServer.Repository;
using SyncSharpServer.Services;
using SyncSharpServer.Validators;

namespace SyncSharpServer.Extensions
{
    public static class ApiServiceExtension
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(configuration);
            services.AddCorsExtension(configuration);

            //Register FluentValidation
            services.AddValidatorsFromAssemblyContaining<SignInValidator>();

            services.AddFluentValidationAutoValidation(options =>
            {
                //Disable data annotations validation
                options.DisableDataAnnotationsValidation = true;
            });

            //Register repository interfaces
            services.AddScoped<IUserRepository, UserRepository>();

            //Register service interfaces
            services.AddScoped<IUserService, UserService>();
            services.AddSingleton<IPasswordHasher, PasswordHasher>();

            //Register DB connection factory
            services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();

            return services;
        }
    }
}
