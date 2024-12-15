using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository;
using BulkyBook.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Identity.UI.Services;
using Stripe;
using BulkyBook.DataAccess.DbInitializer;

var builder = WebApplication.CreateBuilder(args);

// Load configuration from multiple sources
builder.Configuration.AddEnvironmentVariables(); // For Azure Application Settings

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Stripe Configuration: Handle both Azure and Local appsettings.json
var stripeSecretKey = builder.Configuration["Stripe:SecretKey"]; // Get from Azure or appsettings.json
var stripePublishableKey = builder.Configuration["Stripe:PublishableKey"];

if (string.IsNullOrEmpty(stripeSecretKey) || string.IsNullOrEmpty(stripePublishableKey))
{
    Console.WriteLine("Stripe configuration keys are missing. Ensure they are set in Azure or appsettings.json.");
    throw new InvalidOperationException("Stripe keys are missing.");
}

// Register StripeSettings for DI (optional if needed in controllers)
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
StripeConfiguration.ApiKey = stripeSecretKey; // Global Stripe API key

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"/identity/Account/Login";
    options.LogoutPath = $"/identity/Account/Logout";
    options.AccessDeniedPath = $"/identity/Account/AccessDenied";
});

// Facebook Authentication
//builder.Services.AddAuthentication().AddFacebook(options =>
//{
//    IConfiguration configuration = builder.Configuration;
//    options.AppId = configuration["Authentication:Facebook:AppId"];
//    options.AppSecret = configuration["Authentication:Facebook:AppSecret"];
//});

// Session Configuration
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(100);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Scoped Services
builder.Services.AddScoped<IDbInitializer, DbInitializer>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Set the Stripe API Key globally (useful if StripeConfiguration requires runtime update)
StripeConfiguration.ApiKey = stripeSecretKey;

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

// Database seeding
SeedDatabase();

app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

app.Run();

// Method to seed database
void SeedDatabase()
{
    using (var scope = app.Services.CreateScope())
    {
        try
        {
            var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
            dbInitializer.Initialize();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Database Initialization Error: {ex.Message}");
            throw;
        }
    }
}
