using AspNetCoreIdentityApp.Core.OptionsModel;
using AspNetCoreIdentityApp.Core.PermissionsRoot;
using AspNetCoreIdentityApp.Repository.Models;
using AspNetCoreIdentityApp.Repository.Seeds;
using AspNetCoreIdentityApp.Web.ClaimProviders;
using AspNetCoreIdentityApp.Web.Extensions;
using AspNetCoreIdentityApp.Web.Requirements;
using AspNetCoreIdentityApp.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlConn"), options =>
    {
        options.MigrationsAssembly("AspNetCoreIdentityApp.Repository");
    });
});

builder.Services.AddIdentityWithExtension();

builder.Services.ConfigureApplicationCookie(options =>
{
    var cookieBuilder = new CookieBuilder();
    cookieBuilder.Name = "IdentityCookie";

    options.LoginPath = new PathString("/Home/SignIn");
    options.LogoutPath = new PathString("/Member/LogOut");
    options.AccessDeniedPath = new PathString("/Member/AccessDenied");

    options.Cookie = cookieBuilder;
    options.ExpireTimeSpan = TimeSpan.FromDays(60);
    options.SlidingExpiration = true;
});

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IClaimsTransformation, UserClaimProvider>();
builder.Services.AddScoped<IAuthorizationHandler, ExchangeExpireRequirementHandler>();
builder.Services.AddScoped<IAuthorizationHandler, ViolenceRequirementHandler>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AnkaraPolicy", policy =>
    {
        policy.RequireClaim("City", "Ankara");
        //policy.RequireClaim("City", "Ankara","Kirklareli"); //bu şekilde birsürü value ile güncellenebilir
        policy.RequireRole("Admin");
    });

    options.AddPolicy("ExchangePolicy", policy =>
    {
        policy.AddRequirements(new ExchangeExpireRequirement());
    });

    options.AddPolicy("ViolencePolicy", policy =>
    {
        policy.AddRequirements(new ViolenceRequirement() { ThresholdAge = 18 });
    });

    options.AddPolicy("OrderPermissionReadAndDelete", policy =>
    {
        policy.RequireClaim("Permission", Permission.Order.Read);
        policy.RequireClaim("Permission", Permission.Order.Delete);
        policy.RequireClaim("Permission", Permission.Stock.Delete);
    });

    options.AddPolicy("Permission.Order.Read", policy =>
    {
        policy.RequireClaim("Permission", Permission.Order.Read);
    });

    options.AddPolicy("Permission.Order.Delete", policy =>
    {
        policy.RequireClaim("Permission", Permission.Order.Delete);
    });

    options.AddPolicy("Permission.Stock.Delete", policy =>
    {
        policy.RequireClaim("Permission", Permission.Stock.Delete);
    });

});

builder.Services.Configure<SecurityStampValidatorOptions>(options =>
{
    options.ValidationInterval = TimeSpan.FromMinutes(30);
});

builder.Services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Directory.GetCurrentDirectory()));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();

    await PermissionSeed.Seed(roleManager);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
   pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
