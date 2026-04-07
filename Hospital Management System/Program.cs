using Hospital_Management_System;
using Serilog;
var builder = WebApplication.CreateBuilder(args);



builder.Services.AddDependencies(builder.Configuration);
//builder.Services.AddIdentityApiEndpoints<ApplicationUser>()
//    .AddEntityFrameworkStores<ApplicationDbContext>()
//    .AddDefaultTokenProviders();

// Logging:
builder.Host.UseSerilog((context, loggerConfiguration) =>
{
    loggerConfiguration.ReadFrom.Configuration(context.Configuration);
});

// Add this at the very beginning of your Program.cs
QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(option => option.SwaggerEndpoint("/openapi/v1.json", "v1"));
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseExceptionHandler();

app.Run();

/*using Hospital_Management_System;
using Serilog;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDependencies(builder.Configuration);

builder.Host.UseSerilog((context, loggerConfiguration) =>
    loggerConfiguration.ReadFrom.Configuration(context.Configuration)
);

builder.Services.AddCors(options => {
    options.AddPolicy("AllowAll", builder =>
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader());
});

var app = builder.Build();

app.UseDeveloperExceptionPage();
app.UseExceptionHandler();
app.UseSerilogRequestLogging();

app.MapOpenApi();
app.UseSwaggerUI(option => option.SwaggerEndpoint("./openapi/v1.json", "v1"));

app.UseHttpsRedirection();
app.UseCors("AllowAll");       
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();*/
