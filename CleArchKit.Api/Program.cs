using CleArchKit.Application.Users.Cache;
using CleArchKit.Application.Users.Services;
using CleArchKit.Application.Users.UseCases.Query;
using CleArchKit.Domain.Users.Repositories;
using CleArchKit.Infrastructure.Persistence.Postgresql;
using CleArchKit.Infrastructure.Persistence.Users.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// DBContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Controller
builder.Services.AddControllers();

// Service
builder.Services.AddScoped<IUserService, UserService>();

// UseCase
builder.Services.AddScoped<IUserQueryUseCase, UserQueryUseCase>();

// Repository
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Cache
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<IUserCache, UserCache>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
