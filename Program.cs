using ITAssetHelpdesk.Data;
using ITAssetHelpdesk.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddRazorPages();
builder.Services.AddDbContext<AppDbContext>(o =>
    o.UseSqlite("Data Source=ITAssetHelpdesk.db"));
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<DashboardService>();
builder.Services.AddScoped<AssetService>();
builder.Services.AddScoped<EmployeeService>();
builder.Services.AddScoped<TicketService>();

// Session for auth
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(o => { o.IdleTimeout = TimeSpan.FromHours(4); o.Cookie.HttpOnly = true; o.Cookie.IsEssential = true; });

var app = builder.Build();

// Seed DB
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DbSeeder.SeedAsync(db);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseSession();

app.MapStaticAssets();
app.MapRazorPages().WithStaticAssets();

// Redirect root to dashboard or login
app.MapGet("/", ctx => { ctx.Response.Redirect("/Login"); return Task.CompletedTask; });

app.Run();
