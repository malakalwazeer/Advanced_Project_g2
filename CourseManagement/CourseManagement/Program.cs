using CourseManagement.Hubs;
using CourseManagement.Services;
using CourseManagementAPI.Data;
using CourseManagementAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CourseManagementAPI.Services.Validation;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<CourseManagementDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<CourseManagementDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

builder.Services.AddScoped<CourseValidationService>();
builder.Services.AddScoped<CourseSessionValidationService>();
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<EnrollmentValidationService>();
builder.Services.AddScoped<PaymentValidationService>();
builder.Services.AddScoped<AssessmentValidationService>();
builder.Services.AddScoped<CertificationProgressService>();

builder.Services.AddSignalR();
builder.Services.AddScoped<EnrollmentBroadcastService>();

builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"]!);
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.MapStaticAssets();

app.MapHub<EnrollmentHub>("/hubs/enrollment");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
