using ReportingCourseManagement.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Add services to the container.
builder.Services.AddControllersWithViews();

// Fetch the clean API string from appsettings.json
var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7072";

// 2. Register HttpClients with safe BaseAddress registration
builder.Services.AddHttpClient<AuthApiService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

builder.Services.AddHttpClient<ReportApiService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

// 3. Register Session Dependencies
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache(); // Explicitly backs up Session storage
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// 4. Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// --- CRITICAL ORDER FOR JWT SESSION ASSIGNMENT ---
app.UseSession(); // Must run AFTER Routing and BEFORE Authorization
app.UseAuthorization();

app.MapStaticAssets();

// 5. Default app behavior to hit your new Account Login portal automatically
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}")
    .WithStaticAssets();

app.Run();