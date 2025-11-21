// Program.cs - Full, Corrected Code

using Contract_Monthly_Claim_System.Data;
using Contract_Monthly_Claim_System.services;
using Contract_Monthly_Claim_System.services.interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// 1. Connection String Setup
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                        throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// 2. Database Context Setup
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});

// 3. Identity Services Configuration
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>();

// 4. Application Services
builder.Services.AddScoped<IClaimService, ClaimService>();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages(); // Ensure this is added for Identity UI

var app = builder.Build();

// ==========================================================
// *** FIX START: Ensure Database is Created/Migrated on Startup ***
// This is critical to resolve the "Cannot open database" error 
// if the database does not yet exist.
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate();
        // This will create the database and apply all pending migrations.
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred creating or migrating the database.");
        // Consider re-throwing the exception in a development environment.
    }
}
// *** FIX END ***
// ==========================================================


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) app.UseExceptionHandler("/Home/Error");

app.UseStaticFiles();

app.UseRouting();

// 5. Enable Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// 6. Map Razor Pages (Required for Identity UI)
app.MapRazorPages();

app.Run();
