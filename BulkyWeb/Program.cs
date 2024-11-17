using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository;
using BulkyBook.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Identity.UI.Services;
using Stripe;

internal class Program
{
	private static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		// Add services to the container.
		builder.Services.AddControllersWithViews();
		builder.Services.AddDbContext<ApplicationDbContext>(options =>
			options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

		builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));

		builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
		builder.Services.ConfigureApplicationCookie(options =>
		{
			options.LoginPath = $"/identity/Account/Login";
			options.LogoutPath = $"/identity/Account/Logout";
			options.AccessDeniedPath = $"/identity/Account/AccessDenied";
		});

		builder.Services.AddAuthentication().AddFacebook(options =>
		{
			options.AppId = "1350148716359419";
			options.AppSecret = "b4c672fae07f1b9e28de238e77ad3ecd";
		});

		builder.Services.AddDistributedMemoryCache();
		builder.Services.AddSession(options =>
		{
			options.IdleTimeout = TimeSpan.FromMinutes(100);
			options.Cookie.HttpOnly = true;
			options.Cookie.IsEssential = true;
		});

		builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
		builder.Services.AddScoped<IEmailSender, EmailSender>();
		builder.Services.AddRazorPages();

		var app = builder.Build();

		// Configure the HTTP request pipeline.
		if (!app.Environment.IsDevelopment())
		{
			app.UseExceptionHandler("/Home/Error");
			// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
			app.UseHsts();
		}

		app.UseHttpsRedirection();
		app.UseStaticFiles();
		StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();

		app.UseRouting();

		app.UseAuthentication();
		app.UseAuthorization();
		app.UseSession();
		app.MapRazorPages();

		app.MapControllerRoute(
			name: "default",
			pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

		app.Run();
	}
}