namespace Hospital_Management_System;

public static class Dependency_Injection 
{
    public static IServiceCollection AddDependencies(this IServiceCollection services , IConfiguration configuration)
    {
        services.AddControllers();

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        services.AddOpenApi();
        // Add Maspter , FluentValidation Method DI :
        services
            .AddMaspterConfig()
            .AddFluentValidationConfig();

        // Add Hybrid Cacahing :
        services.AddHybridCache();

        // Read From MailSettings in appsetting :
        services.Configure<MailSettings>(configuration.GetSection(nameof(MailSettings)));

        /*CROS:
        var config = configuration.GetSection("AllowedOrigin").Get<string[]>();
        services.AddCors(options =>
            options.AddDefaultPolicy(builder =>
            builder
                .WithOrigins(config!)
                .AllowAnyMethod()
                .AllowAnyHeader()
            )
        );*/

        // Add Connection String :
        var connectionString = configuration.GetConnectionString("DefaultConnection") ??
              throw new InvalidOperationException("Connect String 'DefaultConnection' Has Not Found");
        // Add My Service with has's Interfaceing :
        services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<IDoctorService, DoctorService>();
        services.AddScoped<IAuthorService, AuthorService>();
        services.AddScoped<IPatientsServices, PatientsService>();
        services.AddScoped<IDepartmentService, DepartmentService>();
        services.AddScoped<IAppointmentService, AppointmentService>();
        services.AddScoped<IRoomService, RoomService>();
        // Tell it to use the old Microsoft UI interface
        services.AddScoped<IEmailSender, EmailService>();

        // Add Global Exception Handler:
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
        // Add Author Token Config Method :
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

    public static IServiceCollection AddAuthorTokenConfig(this IServiceCollection services ,IConfiguration configuration)
    {
       
        services
          .AddIdentity<ApplicationUser, IdentityRole>()
          .AddEntityFrameworkStores<ApplicationDbContext>()
          .AddDefaultTokenProviders();

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

        // Confirmation on identityOptions for Regiter :
        services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequiredLength = 8;
            options.SignIn.RequireConfirmedEmail = true;
            //options.User.AllowedUserNameCharacters ="abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
            options.User.RequireUniqueEmail = true;
            options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
        }

      );

        return services;
    }

}
