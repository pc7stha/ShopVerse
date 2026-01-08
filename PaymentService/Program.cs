using BuildingBlocks.Messaging.Kafka;
using BuildingBlocks.Observability.Logging;
using PaymentService.Consumers;

var builder = WebApplication.CreateBuilder(args);

// Add Serilog
builder.AddSerilog("PaymentService");

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

        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidIssuer = "http://localhost:8080/realms/shopverse",
            ValidateIssuer = true
        };
    });

builder.Services.AddAuthorization();

// Add Kafka publisher and consumer
builder.Services.AddKafkaPublisher(builder.Configuration);
builder.Services.AddHostedService<OrderCreatedConsumer>();

var app = builder.Build();

// Add Serilog request logging
app.UseSerilogLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Docker"))
{
    app.MapOpenApi();
}

// Skip HTTPS redirection in Docker (HTTP only internally)
if (!app.Environment.IsEnvironment("Docker"))
{
    app.UseHttpsRedirection();
}

//app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
