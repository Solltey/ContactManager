using ContactManager.Application.Interfaces;
using ContactManager.Application.Models;
using ContactManager.Application.Services;
using ContactManager.Persistence.Context;
using ContactManager.Persistence.Entities;
using ContactManager.WebUI.Interfaces;
using ContactManager.WebUI.Managers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Events;
using System.Net;
using System.Text;
using TelegramSink;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

//Bot Url: t.me/hermes_exceptions_bot
//Replace ChatID value in appsettings.json with your chat's ID to test it
Log.Logger = new LoggerConfiguration().MinimumLevel.Override("Microsoft", LogEventLevel.Error)
            .MinimumLevel.Override("System", LogEventLevel.Error)
            .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Error)
            .WriteTo.TeleSink(configuration["TelegramBot:BotToken"],
            configuration["TelegramBot:ChatID"])
            .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog();

builder.Services.AddHttpClient();

builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<IEmailService>(provider =>
{
    var dropboxCredentials = new DropboxCredentials
    {
        ClientId = configuration["Dropbox:ClientId"],
        ClientSecret = configuration["Dropbox:ClientSecret"],
        RefreshToken = configuration["Dropbox:RefreshToken"]
    };
    return new EmailService(dropboxCredentials, provider.GetRequiredService<ILogger<EmailService>>());
});

builder.Services.AddScoped<IJwtService>(provider =>
{
    return new JwtService(
        provider.GetRequiredService<UserManager<ApplicationUser>>(),
        provider.GetRequiredService<ILogger<JwtService>>());
});

builder.Services.AddScoped<IAccountManager, AccountManager>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("ContactManagerDatabase"));
});

//Config Sign Up Settings
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.User.AllowedUserNameCharacters += " ";
    options.SignIn.RequireConfirmedEmail = Boolean.Parse(configuration["RegistrationSettings:RequireConfirmedEmail"]);
    options.Password.RequireDigit = Boolean.Parse(configuration["RegistrationSettings:RequireDigit"]);
    options.Password.RequireLowercase = Boolean.Parse(configuration["RegistrationSettings:RequireLowercase"]);
    options.Password.RequireUppercase = Boolean.Parse(configuration["RegistrationSettings:RequireUppercase"]);
    options.Password.RequiredLength = int.Parse(configuration["RegistrationSettings:RequiredLength"]);
    options.Password.RequireNonAlphanumeric = Boolean.Parse(configuration["RegistrationSettings:RequireNonAlphanumeric"]);
}).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    //Congif JWT Settings
    options.TokenValidationParameters = new TokenValidationParameters
    {
        RequireExpirationTime = Boolean.Parse(configuration["JwtSettings:RequireExpirationTime"]),
        ValidateIssuer = Boolean.Parse(configuration["JwtSettings:ValidateIssuer"]),
        ValidateAudience = Boolean.Parse(configuration["JwtSettings:ValidateAudience"]),
        ValidateLifetime = Boolean.Parse(configuration["JwtSettings:ValidateLifetime"]),
        ValidateIssuerSigningKey = Boolean.Parse(configuration["JwtSettings:ValidateIssuerSigningKey"]),
        ValidIssuer = configuration["JwtSettings:Issuer"],
        ValidAudience = configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:SecurityKey"]))
    };
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.Strict,
    HttpOnly = HttpOnlyPolicy.Always,
    Secure = CookieSecurePolicy.Always
});

app.UseStatusCodePages(async context => {
    var request = context.HttpContext.Request;
    var response = context.HttpContext.Response;

    if (response.StatusCode == (int)HttpStatusCode.Unauthorized)
    {
        response.Redirect("/Account/SignIn");
    }
});

app.Use(async (context, next) =>
{
    var token = context.Request.Cookies["access_token"];
    if (!string.IsNullOrEmpty(token))
        context.Request.Headers.Add("Authorization", "Bearer " + token);

    await next();
});

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
