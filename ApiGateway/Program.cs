using BuildingBlocks.Observability.Logging;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add Serilog
builder.AddSerilog("ApiGateway");

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = builder.Configuration["Keycloak:Authority"];
        options.Audience = builder.Configuration["Keycloak:Audience"];
        options.RequireHttpsMetadata = false;

        // Accept tokens issued by localhost (from browser/Postman)
        // even though we validate against keycloak container or host.docker.internal
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidIssuer = "http://localhost:8080/realms/shopverse",
            ValidateIssuer = true
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

// Add Serilog request logging
app.UseSerilogLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Docker")
{
    app.MapOpenApi().AllowAnonymous();
    //swagger/index.html
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    });
    //api-docs/index.html
    app.UseReDoc(options =>
    {
        options.SpecUrl("/openapi/v1.json");
    });
    //scalar/
    app.MapScalarApiReference();
}

// Skip HTTPS redirection in Docker (HTTP only internally)
if (!app.Environment.IsEnvironment("Docker"))
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapReverseProxy().RequireAuthorization();

app.MapControllers();

app.Run();
