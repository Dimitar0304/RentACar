using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using RentACar.Core.Services;
using RentACar.Core.Services.CarDto;
using RentACar.Core.Services.Chat;
using RentACar.Core.Services.Contracts;
using RentACar.Infrastructure.Data;
using RentACar.Infrastructure.Data.Models.User;
using RentACar.Infrastructure.Data.Seed;
using RentACar.Middleware;

var builder = WebApplication.CreateBuilder(args);

var dbUrl = Environment.GetEnvironmentVariable("DATABASE_URL")
        ?? "postgres://postgres_rentcar_user:o1FygMsWeSanMEDM0JOnXXqE6tNRiQ4f@dpg-d1cjdm6r433s73fspnlg-a.oregon-postgres.render.com:5432/postgres_rentcar";

if (string.IsNullOrEmpty(dbUrl))
    throw new Exception("DATABASE_URL is not set");

var dbUri = new Uri(dbUrl);
var userInfo = dbUri.UserInfo.Split(':');

var connectionString = new Npgsql.NpgsqlConnectionStringBuilder
{
    Host = dbUri.Host,
    Port = dbUri.Port,
    Username = userInfo[0],
    Password = userInfo[1],
    Database = dbUri.AbsolutePath.TrimStart('/'),
    SslMode = SslMode.Prefer,
    TrustServerCertificate = true
}.ToString();

builder.Services.AddDbContext<RentCarDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(opt=>
{
    opt.SignIn.RequireConfirmedAccount = false;
    opt.User.RequireUniqueEmail = true;
    opt.Password.RequireDigit = false;
    opt.Password.RequireNonAlphanumeric = false;
    opt.Password.RequireUppercase = false;
    opt.Password.RequireLowercase = false;
    
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<RentCarDbContext>();

builder.Services.AddControllersWithViews();

builder.Services.AddSignalR();

builder.Services.AddScoped<ISeeder, CategorySeeder>();
builder.Services.AddScoped<ISeeder, RoleSeeder>();
builder.Services.AddScoped<ISeeder, CarSeeder>();
builder.Services.AddScoped<ISeeder, RentBillSeeder>();
builder.Services.AddScoped<ApplicationSeeder>();
builder.Services.AddTransient<ICarService, CarService>();
builder.Services.AddTransient<IChatService, ChatHubModel>();
builder.Services.AddTransient<IRentBillService, RentBillService>();
builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        ["application/octet-stream"]);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseRouting();

app.UseAuthorization();

app.UseResponseCompression();

app.MapControllerRoute(
    name: "areaRoute",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapHub<ChatHubModel>("/chathub");
app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<ApplicationSeeder>();
    await seeder.SeedAsync();
}

app.Run();
