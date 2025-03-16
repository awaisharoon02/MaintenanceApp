using System.Data.Entity;
using DevExpress.AspNetCore;
using DevExpress.DashboardAspNetCore;
using DevExpress.DashboardWeb;
using DevExpress.DashboardWeb.Native;
using DevExpress.Utils;
using DevExpress.XtraCharts;
using DXApplication1;
using MaintenanceApp.Controllers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

static async Task CreateRoles(IServiceProvider serviceProvider)
{
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    string[] roleNames = { "Admin", "Initiator", "Maintainer", "Acknowledge" };

    foreach (var role in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();


// Register DevExpress services
IFileProvider? fileProvider = builder.Environment.ContentRootFileProvider;
IConfiguration? configuration = builder.Configuration;
// ...
builder.Services.AddDevExpressControls();
//builder.Services.AddScoped<DashboardConfigurator>((IServiceProvider serviceProvider) => {
//    DashboardConfigurator configurator = new DashboardConfigurator();
//    configurator.SetDashboardStorage(new DashboardFileStorage(fileProvider.GetFileInfo("Data/Dashboards").PhysicalPath));
//    configurator.SetConnectionStringsProvider(new DashboardConnectionStringsProvider(configuration));
//    return configurator;
//});
builder.Services.AddScoped<DashboardConfigurator>((IServiceProvider serviceProvider) => {
    return DashboardUtils.CreateDashboardConfigurator(configuration, fileProvider);
});

DeserializationSettings.RegisterTrustedAssembly(typeof(object).Assembly);
DeserializationSettings.RegisterTrustedClass(typeof(object));


builder.Services.AddDbContext<MaintenanceDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<MaintenanceDbContext>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
    options.AddPolicy("Initiator", policy => policy.RequireRole("Initiator"));
    options.AddPolicy("Maintainer", policy => policy.RequireRole("Maintainer"));
    options.AddPolicy("Acknowledge", policy => policy.RequireRole("Acknowledge"));
});

builder.Services.AddDefaultIdentity<ApplicationUser>()
    .AddRoles<IdentityRole>()  // Make sure roles are added
    .AddEntityFrameworkStores<MaintenanceDbContext>();

//builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
//    .AddEntityFrameworkStores<MaintenanceDbContext>()
//    .AddDefaultTokenProviders();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await CreateRoles(services);
}


app.UseDevExpressControls();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.MapControllerRoute(
//    name: "MaintenanceRequest",
//    pattern: "MaintenanceRequest/{action=Index}/{id?}",
//    defaults: new { controller = "MaintenanceRequest" });



app.MapDashboardRoute("dashboardControl", "DefaultDashboard");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Add a default route to prevent all URLs from redirecting to MaintenanceRequest


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
