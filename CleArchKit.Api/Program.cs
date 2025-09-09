using CleArchKit.Application.Persistence;
using CleArchKit.Application.Users.Services;
using CleArchKit.Application.Users.UseCases.Command;
using CleArchKit.Application.Users.UseCases.Query;
using CleArchKit.Domain.Users.Repositories;
using CleArchKit.Infrastructure.Persistence;
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
builder.Services.AddScoped<IUserCommandUseCase, UserCommandUseCase>();

// Transaction
builder.Services.AddScoped<IUnitOfWork, EfUnitOfWork>();

// Repository
builder.Services.AddScoped<IUserRepository, UserRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
