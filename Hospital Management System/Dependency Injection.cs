using FluentValidation.AspNetCore;
using Hospital_Management_System.Authentication;
using MapsterMapper;
using Microsoft.Extensions.Options;
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
        services.AddScoped<IDoctorService, DoctorService>();
        services.AddScoped<IAuthorService, AuthorService>();

        // Add Author Token Config
        services.AddAuthorTokenConfig(configuration);



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

    public static IServiceCollection AddAuthorTokenConfig(this IServiceCollection services ,
        IConfiguration configuration)
    {
       
        services
          .AddIdentity<ApplicationUser, IdentityRole>()
          .AddEntityFrameworkStores<ApplicationDbContext>();
        services.AddSingleton<IJwtProvider, JwtProvider>();
        
       services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.OptionName));
        var JwtSetting = configuration.GetSection(JwtOptions.OptionName).Get<JwtOptions>();
        services.AddAuthentication(options =>
        {

            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })

            .AddJwtBearer(options => 
            {
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = JwtSetting?.issuer ,
                    ValidAudience = JwtSetting?.audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSetting?.key!))
                };
            });

        return services;
    }

}
