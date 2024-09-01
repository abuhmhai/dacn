using FontWeb.Data;
using FontWeb.Service;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Python.Runtime;
using static FontWeb.Pages.DashBoard;
using Syncfusion.Blazor;
using Syncfusion.Licensing;

var builder = WebApplication.CreateBuilder(args);
// Đăng ký HttpClient với URL API chính xác
//builder.Services.AddHttpClient<StockService>(client =>
//{
//    client.BaseAddress = new Uri("https://localhost:7030/");
//});

// Thêm dịch vụ CORS
builder.Services.AddCors(options =>
{
	options.AddDefaultPolicy(policy =>
	{
		policy.AllowAnyOrigin()
			  .AllowAnyHeader()
			  .AllowAnyMethod();
	});
});

// Đăng ký mã giấy phép Syncfusion ở đây
SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NCaF1cWWhAYVJyWmFZfVpgdVdMYFRbRXJPIiBoS35RckVrWHpccnVcRGVeVU10");

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSyncfusionBlazor();
builder.Services.AddSingleton<WeatherForecastService>();
//builder.Services.AddSingleton<StockService>();
builder.Services.AddSingleton<StockServices>();
builder.Services.AddScoped<CryptoService>();
//builder.Services.AddScoped<Crypto>();
//builder.Services.AddScoped<Test>();
//builder.Services.AddScoped<ApiService>();
//builder.Services.AddScoped<StockService>();
builder.Services.AddScoped<StockDataServiceCsv>();
//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://127.0.0.1:5000/") });



// Register HttpClient
builder.Services.AddHttpClient();

builder.Services.AddBlazorBootstrap();
var app = builder.Build();

app.UseCors();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseStaticFiles();

app.UseRouting();


app.UseHttpsRedirection();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
