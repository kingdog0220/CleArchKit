using System.Diagnostics;
using BlazorWasmTemplate.Presentation.Web.Components;

// マイグレーションが全部適用済みかチェックする
// マイグレーションを使用しない、適用済みかチェックしなくてもいい場合はこのコードは削除して問題ない
var migrationsProjectRelativePath = "../BlazorWasmTemplate.MigrationChecker";
var migrationsProjectFullPath = Path.GetFullPath(migrationsProjectRelativePath);

var migrationChecker = Process.Start(new ProcessStartInfo
{
    FileName = "dotnet",
    Arguments = $"run --project \"{migrationsProjectFullPath}\"",
    RedirectStandardOutput = true,
    RedirectStandardError = true,
    UseShellExecute = false,
    CreateNoWindow = true
});
migrationChecker!.WaitForExit();
if (migrationChecker.ExitCode != 0)
{
    Console.Error.WriteLine("未適用のマイグレーションがあります。アプリケーションを終了します。");
    Environment.Exit(1);
}


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();


var app = builder.Build();

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
