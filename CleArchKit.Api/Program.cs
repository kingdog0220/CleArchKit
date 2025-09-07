using CleArchKit.Application.Events;
using CleArchKit.Application.Expands;
using CleArchKit.Application.Persistence;
using CleArchKit.Application.Users.Cache;
using CleArchKit.Application.Users.Services;
using CleArchKit.Application.Users.UseCases.Command;
using CleArchKit.Application.Users.UseCases.Query;
using CleArchKit.Domain.Events;
using CleArchKit.Domain.Users.Repositories;
using CleArchKit.Infrastructure.Events;
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
builder.Services.AddScoped<IUseCaseExecutor, UseCaseExecutor>();

// Transaction
builder.Services.AddScoped<IUnitOfWork, EfUnitOfWork>();

// Repository
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Cache
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<IUserCache, UserCache>();

// Dispatcher
builder.Services.AddSingleton<IDomainEventDispatcher, InMemoryDomainEventDispatcher>();

// Event
builder.Services.AddScoped<IDomainEventBuffer, DomainEventBuffer>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
