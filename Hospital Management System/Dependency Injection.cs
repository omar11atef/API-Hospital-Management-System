using FluentValidation.AspNetCore;
using MapsterMapper;
using System.Reflection;

namespace Hospital_Management_System;

public static class Dependency_Injection 
{
    public static IServiceCollection AddDependencies(this IServiceCollection services , IConfiguration configuration)
    {
        services.AddControllers();

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        services.AddOpenApi();

        services
            .AddMaspterConfig()
            .AddFluentValidationConfig();

        // Add Connection String :
        var connectionString = configuration.GetConnectionString("DefaultConnection") ??
              throw new InvalidOperationException("Connect String 'DefaultConnection' Has Not Found");
        services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<IDoctorServices, DoctorServices>();



        return services;
    }
    public static IServiceCollection AddMaspterConfig(this IServiceCollection services)
    {
        // Add Mapster
        var MappingConfig = TypeAdapterConfig.GlobalSettings;
        MappingConfig.Scan(Assembly.GetExecutingAssembly());
        services.AddSingleton<IMapper>(new Mapper());
        return services;
    }

    public static IServiceCollection AddFluentValidationConfig(this IServiceCollection services)
    {
        // Add FluentValidation
        services
            .AddFluentValidationAutoValidation()
            .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        return services;
    }

}
