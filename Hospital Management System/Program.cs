using Hospital_Management_System;
using Serilog;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDependencies(builder.Configuration);
/*builder.Services.AddIdentityApiEndpoints<ApplicationUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();*/

// Logging
builder.Host.UseSerilog((context, loggerConfiguration) =>
{
    loggerConfiguration.ReadFrom.Configuration(context.Configuration);
});



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

/*var builder = WebApplication.CreateBuilder(args);
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

// ✅ ترتيب صحيح
app.UseDeveloperExceptionPage();
app.UseExceptionHandler();
app.UseSerilogRequestLogging();

// ✅ Swagger بدون شرط
app.MapOpenApi();
app.UseSwaggerUI(option => option.SwaggerEndpoint("/openapi/v1.json", "v1"));

app.UseHttpsRedirection();
app.UseCors("AllowAll");        // ✅ مرة واحدة فقط
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();*/
