using iCoin.Data;
using Microsoft.EntityFrameworkCore;
using Models;
using StackExchange.Redis;
using Microsoft.AspNetCore.Authentication.Cookies;
using Services;
using SignalRChat.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddSingleton<IConnectionMultiplexer>(opt =>
    ConnectionMultiplexer.Connect($"{builder.Configuration.GetConnectionString("RedisConnection")}, allowAdmin = true"));

builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SQLConnection"));
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.LoginPath = "/Login";
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("CORS", builder =>
    {
        builder.WithOrigins(new string[]
            {
                "https://127.0.0.1:7046",
                "https://localhost:7046",
                "http://127.0.0.1:7046",
                "http://localhost:7046"
            })
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddScoped<ICoinRepo, RedisCoinRepo>();
builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddHostedService<BackgroundService>();

builder.Services.AddSignalR();
builder.Services.AddHttpClient();
builder.Services.AddControllers();
builder.Services.AddRazorPages();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseAuthentication();


app.UseCors("CORS");
app.UseRouting();
app.UseHttpsRedirection();
app.UseAuthorization();
app.UseStaticFiles();
app.MapRazorPages();
app.MapControllers();
app.MapHub<CoinHub>("/coinHub");

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});


app.Run();
