using BlazorWasmTemplate.Infrastructure;
using BlazorWasmTemplate.Presentation.Web.Components;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// DbContext登録（設定はInfrastructureのものを使う）
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
    npgsqlOptions => npgsqlOptions.MigrationsAssembly("BlazorWasmTemplate.Migrations")));

var app = builder.Build();

// マイグレーションが全部適用済みかチェック
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var pending = db.Database.GetPendingMigrations();

    if (pending.Any())
    {
        Console.Error.WriteLine("ERROR: There are pending migrations.");
        Environment.Exit(-1);  // 強制終了
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
