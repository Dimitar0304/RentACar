using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using RentACar.Core.Services.CarDto;
using RentACar.Core.Services.Chat;
using RentACar.Core.Services.Contracts;
using RentACar.Infrastructure.Data;
using RentACar.Infrastructure.Data.Seed;


var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("RentCarConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<RentCarDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(opt=>
{
    opt.SignIn.RequireConfirmedAccount = false;
    opt.User.RequireUniqueEmail = true;
    opt.Password.RequireDigit = false;
    opt.Password.RequireNonAlphanumeric = false;
    opt.Password.RequireUppercase = false;
})

    .AddEntityFrameworkStores<RentCarDbContext>();
builder.Services.AddControllersWithViews();

builder.Services.AddSignalR();

builder.Services.AddScoped<ISeeder, CategorySeeder>();
builder.Services.AddScoped<ApplicationSeeder>();
builder.Services.AddTransient<ICarService, CarService>();
builder.Services.AddTransient<IChatService, ChatHubModel>();
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

app.UseHttpsRedirection().
    UseStaticFiles().
    UseRouting().
    UseAuthorization();

app.UseResponseCompression();

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
