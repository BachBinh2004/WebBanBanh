using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebBanBanh.Data;
using WebBanBanh.Models;
using WebBanBanh.Services;
using WebBanBanh.Services.VNPAY;

var builder = WebApplication.CreateBuilder(args);
//Connect VNPay API
builder.Services.AddScoped<IVnPayService, VnPayService>();

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
//    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();
//Add session để lưu số lượng bánh thay static
builder.Services.AddSession();
//cái này để pahn6 quyền theo bài thầy 
builder.Services.AddDefaultIdentity<IdentityUser>
(options =>
options.SignIn.RequireConfirmedAccount = true)
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddDbContext<WebBanBanhContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// Thêm dịch vụ FAQService vào DI container
builder.Services.AddScoped<FAQService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
name: "areas",
pattern: "{area:exists}/{controller=Banhs}/{action=Index}/{id?}");
app.MapRazorPages()
   .WithStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Banhs}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();
app.UseCors(policy => policy
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader()
);
//phân quyền
using (var scope = app.Services.CreateScope())
{
    var roleManager =
    scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    string[] roles = { "Admin", "User", "Manager" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}
app.Run();
