using GrpcSandbox.Core;
using GrpcSandbox.Server.Repositories;
using GrpcSandbox.Server.Services;

var builder = WebApplication.CreateBuilder(args);
var issuer = builder.Configuration["Jwt:Issuer"] ?? string.Empty;
var audience = builder.Configuration["Jwt:Audience"] ?? string.Empty;
var key = builder.Configuration["Jwt:Key"] ?? string.Empty;

// Add services to the container.
builder.Services
    .AddAuthentication()
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new DefaultTokenValidationParameters(issuer, audience, key);
    });

var validator = new JwtTokenValidator(audience, issuer, key);

builder.Services.AddSingleton(validator);
builder.Services.AddAuthorization();
builder.Services.AddGrpc();
builder.Services.AddSingleton<CustomerRepository>();
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("AllowAll", pb =>
    {
        pb.AllowAnyOrigin();
        pb.AllowAnyHeader();
        pb.AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapGrpcService<CustomerService>();
app.MapGrpcService<DummyService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
